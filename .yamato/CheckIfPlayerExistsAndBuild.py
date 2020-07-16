import os
import sys
import zipfile
import hashlib
import utility.ArtifactoryFileTransferManager
import utility.versionCheck
import time
import io
import utility.BuildStandalonePlayer


def CheckIfPlayerExistsAndBuild():
    # Get the latest unity version hash
    currentUnityHash = utility.versionCheck.get_latest_version(os.getenv('unityVERSION'))
    print("Branch " + os.getenv('unityVERSION') + " latest hash is: " + currentUnityHash)

    # Get the xr.xrautomatedtests revision hash
    gitRevisionHash = os.getenv("GIT_REVISION")
    print("gitRevisionHash is : " + gitRevisionHash)

    # generate a hash from the cmd line arguements. This should be unique to every test.
    cmdHash = str(hashlib.sha1(os.getenv('cmd').encode('utf-8')).hexdigest())
    print("cmd hash is: " + cmdHash)

    # Combine these 3 hashes to generate a filename that is unique to this unity version,
    # autoamtedtests version, and test parameters.
    filename = currentUnityHash + "_" + gitRevisionHash + "_" + cmdHash + ".zip"
    playerUrl = "" + os.getenv("artifactPath") + "/" + filename
    # check artifactory to see if this player has already been built.
    PlayerExists = utility.ArtifactoryFileTransferManager.does_file_already_exist(playerUrl)

    # if it hasn't trigger the process of building the player.
    if PlayerExists is False:
        print("Player didn't exist. Building Player.")
        # Install Unity with the requested version and components
        utility.BuildStandalonePlayer.install_unity(os.getenv('unityVERSION'), os.getenv('includeAndroid'),
                                                    os.getenv('includeUWP'), os.getenv('includeIL2CPP'),
                                                    os.getenv('includeIOS'))

        # Build the player with the previously installed Unity version.
        utility.BuildStandalonePlayer.build_player(os.getenv('cmd'))

        print("Player Built. Archiving Player...")
        # Archive the player in a zip file.
        outputFolderToArchive = "build"

        zipf = zipfile.ZipFile(filename, 'w', zipfile.ZIP_STORED)

        for root, dirs, files in os.walk(outputFolderToArchive):
            for file in files:
                zipf.write(os.path.join(root, file))

        zipf.close()
        print("Archive complete archive file named: " + filename)

        # upload the archive to our artifactory
        print("Uploading Player...")
        result = utility.ArtifactoryFileTransferManager.upload_file(filename, os.getenv("artifactPath"))
        if result is not False:
            playerUrl = result
    else:
        print("Player Already Exists at:" + PlayerExists)
        playerUrl = PlayerExists

    # Check to see if we want to run the tests in this player.
    if os.getenv("runTests") == "True":
        print("Artifacts URL: " + playerUrl)
        # If we have specified a label, add it to the Jenkins Rest API Params, otherwise just pass it the Artifacts URL.
        if os.getenv("jenkinsNodeLabel") is not None:
            params = {"ArtifactsURL": playerUrl, "TestNodeLabel": os.getenv("jenkinsNodeLabel")}
        else:
            params = {"ArtifactsURL": playerUrl}

        # Invoke the Jenkins Job, and wait for it to complete before returning.
        result, jobURL = utility.JenkinsJob.start_jenkins_job(os.getenv("jenkinsJob"), params, True, True)

        print("Result: " + result)
        print("JobURL: " + jobURL)

        # Download the Jenkins build artifacts
        artifactZipURL = jobURL + "artifact/RunTest/*zip*/RunTest.zip"

        r = utility.JenkinsJob.download_sbr_artifacts(artifactZipURL)

        # extract them to the location in our workspace where Yamato can collect them.
        z = zipfile.ZipFile(io.BytesIO(r.content))
        workFolder = os.getenv("YAMATO_WORK_DIR")
        outputDir = os.getenv("YAMATO_WORK_DIR") + "\\unity\\xr.xrautomatedtests\\.yamato\\testResults"
        print("Extracting Jenkins Artifacts to: " + outputDir)
        z.extractall(outputDir)

        # If the Jenkins job wasn't successful, fail this job.
        # Ideally there would be a Yamato status for cancelled, but I'm unsure how to exit with it right now.
        if result != "SUCCESS":
            exit(1);
