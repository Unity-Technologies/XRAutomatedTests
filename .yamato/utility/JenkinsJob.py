import requests
import time
import os
import re

temp_username = os.getenv('JENKINSUSER')
temp_APIKEY = os.getenv('JENKINSAPIKEY')
temp_JobToken = os.getenv('JENKINSJOBTOKEN')

branches = os.getenv('UNITYBRANCHES')


def parse_version_for_jenkins(unityVERSION):
    """Determine what type of version this is.
        This is only used when passing the value along to our pre-existing Jenkins Jobs."""

    # Is it a known branch?
    if unityVERSION in branches:
        return "unityBranchName"
    # Does it fit the version syntax? (2020.1.3f1,ect)
    if re.match("\d\d\d\d[.]\d(\d)?[.]\w+", unityVERSION):
        return "unityVersion"
    else:
        # Otherwise it must be a revision hash
        return "unityRevision"


def start_jenkins_job(jobName, params={}, waitForQueue=False, waitForJobComplete=False, userName=temp_username,
                      APIkey=temp_APIKEY, jobToken=temp_JobToken):
    """Start a Jenkins job by using the Rest API to trigger it remotely."""

    # Set the base URL with out shared username and APIKey.
    url = "http://" + userName + ":" + APIkey + "@xrtest.hq.unity3d.com:8080/job/" + jobName + "/buildWithParameters?"

    # Set the jobToken
    url = url + "token=" + jobToken

    # Set the Parameters
    for key in params:
        url = url + "&" + key + "=" + params[key]

    print("Calling URL:" + url)

    # General request form is http://<JenkinsUser>:<JenkinsUser's APIkey>@<Jenkins Server>/job/<job Name>/build?<Job Specific Secret key>
    r = requests.post(url)

    # Quick output of response data
    print("post status: " + str(r.status_code))
    print("post response: " + r.text)
    print("Location: " + r.headers['Location'])

    # Do we want to wait for the Jenkins Job to actually start?
    if waitForQueue is True:
        prettyJSON = r.headers['Location'] + "api/json?pretty=true"
        # Sleep for a safe period to get past the quite period?
        time.sleep(5)

        JobRunning = False
        jobURL = ""
        while JobRunning is False:
            getR = requests.get(prettyJSON, auth=(userName, APIkey))
            json = getR.json()
            # Did the response have the executable url we were looking for?
            if "executable" in json:
                if "url" in json["executable"]:
                    jobURL = json["executable"]["url"]
                    print("job url:" + json["executable"]["url"])
                    JobRunning = True
                    break
            # If not, go ahead and wait 60 seconds before trying again.
            print("Job not started yet, sleeping for 60 seconds...")
            time.sleep(60)
        # If we don't want to wait for the job to be complete, just return the job URL
        if waitForJobComplete is False:
            return jobURL
        else:
            # otherwise, ensure we actually have the Job's URL.
            if jobURL != "":
                # Then start the loop to wait for till the job is actually finished.
                jobRunning = True
                while jobRunning is True:
                    jobJson = jobURL + "api/json?pretty=true"
                    getJob = requests.get(jobJson, auth=(userName, APIkey))
                    jobStatusJSON = getJob.json()
                    # parse the result value of the web request response.
                    if "result" in jobStatusJSON:
                        result = str(jobStatusJSON["result"])
                        print("Job Status:" + result)
                        # if it's none, then the job is still running, and we'll sleep for 5 seconds.
                        if result == "None":
                            print("Job still running, sleep for 30 seconds and check again...")
                            time.sleep(30)
                        # If it's SUCCESS, FAILURE, or ABORTED then return those values, to be handled
                        # as appropriate outside of the job.
                        elif result == "SUCCESS":
                            print("Job Completed Successfully!")
                            jobRunning = False
                            return result, jobURL
                            # collect artifacts here.
                            # mark job as successful.
                        elif result == "FAILURE":
                            print("Job Failed!")
                            jobRunning = False
                            return result, jobURL
                            # Mark this job as a failure in Yamato.
                        elif result == "ABORTED":
                            print("Job ABORTED!")
                            jobRunning = False
                            return result, jobURL
                            # Mark this job as a failure in Yamato.


def download_sbr_artifacts(jobURL, userName=temp_username,
                           APIkey=temp_APIKEY):
    """After a Jenkins job completes, download a zip file containing the collected artifacts from the Jenkins job."""

    # If we can't get the artifacts for 10 minutes, give up and fail this job.
    timeout = 600

    # How often should we check for the artifacts?
    interval = 30

    start = time.time()
    while (time.time() - start) < timeout:
        # Add the Jenkins username and APIKeys to the URL.
        url = jobURL.replace("http://", "http://" + userName + ":" + APIkey + "@")
        print("Download SBR Artifacts from: " + url)

        # Start the Web request to retrieve these.
        r = requests.get(url, stream=True)
        print("SBR Artifact download request returned status code of:" + str(r.status_code))
        # If we didn't find the artifacts, return false. Otherwise return the result status of the web request.
        if r.status_code != 200:
            print("SBR Artifact download from " + url + " returned status code of " + str(r.status_code))
            time.sleep(interval)
        else:
            return r
    print("FAILED TO DOWNLOAD SBR JENKINS ARTIFACTS FROM: " + url)
    return False
