import sys
import subprocess
import utility.JenkinsJob
import utility.ArtifactoryFileTransferManager

default_branch = "trunk"

def check_for_new_version(branch=default_branch):
    filename = "last_unity_" + branch
    last_checked_version = utility.ArtifactoryFileTransferManager.download_hash_file(filename)
    latest_unity_version = subprocess.check_output(
        "/home/bokken/.local/bin/unity-downloader-cli -u "+branch+" -c editor --skip-download --fast", shell=True)
    if last_checked_version == latest_unity_version:
        print("No new version! Exiting!")
        return "no_new_version"
    else:
        print("New version detected: " + latest_unity_version)

        new_version_file = open(filename, "w+")
        new_version_file.write(latest_unity_version)
        new_version_file.close()
        utility.ArtifactoryFileTransferManager.upload_file(filename)

        return latest_unity_version

