import requests
import os

# This is what determines which account will be making the Yamato call.
global_apikey = os.getenv('QA-SEATTLE-YAMATO-APIKEY')

# Known projects and their Project IDs. This is used when making Yamato web requests.
known_projects = {
    "xr.xrautomatedtests": "768"
}


def start_yamato_job(project_number, yaml_file_name, branch_name, job_name, revision=os.getenv("GIT_REVISION"),
                     environmentVariables="[]",
                     apikey=global_apikey, isbearerkey=False):
    """Start a Yamato job by sending web requests to the Yamato endpoint."""

    job_definition = "/projects/" + project_number + "/revisions/" + revision \
                     + "/job-definitions/.yamato%2F" + yaml_file_name + "%23" + job_name

    # Setup the data for the web request
    data = '{"source": {"branchname": "' + branch_name + '","revision" : "' + revision + '" }, \
            "environmentVariables": ' + environmentVariables + '  ,\
            "links": {"project": "/projects/' + project_number + '","jobDefinition": "' + job_definition + '"}}'

    # Determine if the key we're using is an ApiKey, which is shared, or a Bearer key, which is user-based.
    keytype = "ApiKey"
    if isbearerkey:
        keytype = "Bearer"

    # Load the Key into the request header.
    headers = {'Content-Type': 'application/x-www-form-urlencoded',
               'Authorization': keytype + ' ' + apikey}
    # Set the URL for the jobs web request endpoint.
    url = "https://yamato-api.cds.internal.unity3d.com/jobs"

    # output the url, data, and headers we're sending.
    print("Sending Data: " + str(data))
    print("Sending Headers: " + str(headers))
    print("Sending URL: " + url)
    r = requests.post(url, data=data, headers=headers)

    # Check the response.
    print("post status: " + str(r.status_code))
    print("post response: " + r.text)

    # If the request was successful, return true, otherwise return false.
    if r.status_code == 200:
        return True
    return False


def get_job(job_number, apikey=global_apikey, isbearerkey=False):
    """Used to check the status of a job. This a utility function that isn't used yet,
        but will be needed if we need to monitor an active job."""

    keytype = "ApiKey"
    if isbearerkey:
        keytype = "Bearer"

    headers = {'Content-Type': 'application/x-www-form-urlencoded',
               'Authorization': keytype + ' ' + apikey}
    url = "https://yamato-api.cds.internal.unity3d.com/jobs/" + job_number

    print("Sending Headers: " + str(headers))
    print("Sending URL: " + url)
    r = requests.get(url, headers=headers)

    print("post status: " + str(r.status_code))
    print("post response: " + r.text)
    if r.status_code == 200:
        return True
    return False


def get_project_id(project_name, apikey=global_apikey, isbearerkey=False):
    """Returns the project ID, This is a required part of the web requests to start a job and check on its status.
        Technically this could be retrieved by iterating through every project on Yamato and check it's name against
        the requested name. However that would take much longer than this approach."""
    if project_name in known_projects:
        return known_projects[project_name]
