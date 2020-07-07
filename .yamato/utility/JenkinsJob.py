import requests
import time
import datetime
import os
import re

temp_username = os.getenv('JENKINSUSER')
temp_APIKEY = os.getenv('JENKINSAPIKEY')
temp_JobToken = os.getenv('JENKINSJOBTOKEN')

branches = ["trunk","2020.1/staging","2019.4/staging","2018.4/staging"]

#Determine what type of version this is.
#This is only used when passing the value along to our pre-existing Jenkins Jobs.
def parse_version_for_jenkins(unityVERSION):
    #Is it a known branch?
    if unityVERSION in branches:
        return "unityBranchName"
    #Does it fit the version syntax? (2020.1.3f1,ect)
    if re.match("\d\d\d\d[.]\d(\d)?[.]\w+", unityVERSION):
        return "unityVersion"
    else:
    #Otherwise it must be a revision hash
        return "unityRevision"

#Start a Jenkins job by using the Rest API to trigger it remotely.
def start_jenkins_job(jobName, params={}, waitForQueue=False, waitForJobComplete=False, userName=temp_username,
                      APIkey=temp_APIKEY, jobToken=temp_JobToken):
    #Set the base URL with out shared username and APIKey.
    url = "http://" + userName + ":" + APIkey + "@xrtest.hq.unity3d.com:8080/job/" + jobName + "/buildWithParameters?"

    #Set the jobToken
    url = url + "token=" + jobToken

    #Set the Parameters
    for key in params:
        url = url + "&" + key + "=" + params[key]

    print("Calling URL:" + url)

    # General request form is http://<JenkinsUser>:<JenkinsUser's APIkey>@<Jenkins Server>/job/<job Name>/build?<Job Specific Secret key>
    r = requests.post(url)

    #Quick output of response data
    print("post status: " + str(r.status_code))
    print("post response: " + r.text)
    print("Location: " + r.headers['Location'])

    #Do we want to wait for the Jenkins Job to actually start?
    if waitForQueue is True:
        prettyJSON = r.headers['Location'] + "api/json?pretty=true"
        # Sleep for a safe period to get past the quite period?

        #Give it 10 minutes to try and determine the job's URL, then timeout if it hasn't
        timeout = 600
        timeout_time = datetime.datetime.now() + datetime.timedelta(seconds=timeout)
        JobRunning = False
        jobURL = ""
        while JobRunning is False:
            if datetime.datetime.now() > timeout_time:
                print("TIMED OUT BEFORE WE WERE ABLE TO DETERMINE JOB!")
                break
            getR = requests.get(prettyJSON, auth=(userName, APIkey))
            json = getR.json()
            #Did the response have the executable url we were looking for?
            if "executable" in json:
                if "url" in json["executable"]:
                    jobURL = json["executable"]["url"]
                    print("job url:" + json["executable"]["url"])
                    JobRunning = True
                    break
            #If not, go ahead and wait 5 seconds before trying again.
            print("Job not started yet, sleeping for 5 seconds...")
            time.sleep(5)
        #If we don't want to wait for the job to be complete, just return the job URL
        if waitForJobComplete is False:
            return jobURL
        else:
            #otherwise, ensure we actually have the Job's URL.
            if jobURL is not "":
                #Then start the loop to wait for till the job is actually finished.
                jobRunning = True
                while jobRunning is True:
                    jobJson = jobURL + "api/json?pretty=true"
                    getJob = requests.get(jobJson, auth=(userName, APIkey))
                    jobStatusJSON = getJob.json()
                    #parse the result value of the web request response.
                    if "result" in jobStatusJSON:
                        result = str(jobStatusJSON["result"])
                        print("Job Status:" + result)
                        #if it's none, then the job is still running, and we'll sleep for 5 seconds.
                        if result == "None":
                            print("Job still running, sleep for 5 seconds and check again...")
                            time.sleep(5)
                        #If it's SUCCESS, FAILURE, or ABORTED then return those values, to be handled as appropriate outside of the job.
                        elif result == "SUCCESS":
                            print("Job Completed Successfully!")
                            jobRunning = False
                            return result
                            # collect artifacts here.
                            # mark job as successful.
                        elif result == "FAILURE":
                            print("Job Failed!")
                            jobRunning = False
                            return result
                            # Mark this job as a failure in Yamato.
                        elif result == "ABORTED":
                            print("Job ABORTED!")
                            jobRunning = False
                            return result
                            # Mark this job as a failure in Yamato.

