import requests
import hashlib
import sys
import os

# get the artifactory default login credentials from our secret group
artifactory_default_user = os.getenv("ARTIFACTORY-USERNAME")
artifactory_default_key = os.getenv("ARTIFACTORY-APIKEY")

# set the default location.
artifactory_default_url = 'https://artifactory.prd.it.unity3d.com/artifactory'
# Set the content-type
content_type = 'application/java-archive'

# set the name of the repo we're storing our files in.
artifactory_repo = 'xrautomatedtests'


def upload_file(filename, artifactoryPath="", artifactory_url=artifactory_default_url,
                artifactory_user=artifactory_default_user,
                artifactory_key=artifactory_default_key):
    """upload a file to artifactory."""

    # filename requires the file to be in the current operating folder.
    # Unfortunately this an area in which python seems to be bit wonky.

    # This part is where we want the file to be stored in artifactory.

    if artifactoryPath != "":
        url = artifactory_url + '/' + artifactory_repo + '/' + artifactoryPath + "/" + filename
    else:
        url = artifactory_url + '/' + artifactory_repo + '/' + filename

    # checksums are useful for validating that the upload was successful
    headers = {'content-type': content_type,
               'X-Checksum-Md5': hashlib.md5(open(filename, "rb").read()).hexdigest(),
               'X-Checksum-Sha1': hashlib.sha1(open(filename, "rb").read()).hexdigest()}

    # Open a file stream and upload the file to artifactory.
    with open(filename, 'rb') as f:
        r = requests.put(url, auth=(artifactory_user, artifactory_key), data=f, headers=headers)

    # check for a successful upload and return True/False
    if r.status_code != 201:
        print("Something went wrong")
        print("status code: " + str(r.status_code))
        print("text: " + r.text)
        return False
    else:
        print ("Upload Successful! File url: " + r.json()['downloadUri'])
        return r.json()['downloadUri']


def download_hash_file(filename, artifactory_url=artifactory_default_url):
    """Download the latest hash(version number) file from Artifactory"""

    # No login credentials required to download as far as I can tell.
    # this seems odd, but it may be because we're in network.
    url = artifactory_url + '/' + artifactory_repo + '/' + filename

    print("Download URL: " + url)

    r = requests.get(url)
    # If we were able to download the file return the version hash from within it.
    # otherwise let the user know we didn't find the file.
    if r.status_code == 404:
        error = "No File Found at: " + url
        return "404"
    else:
        previousVersionChecked = r.text
        print("File downloaded successfully! Hash contained within: " + previousVersionChecked)
        return previousVersionChecked


def does_file_already_exist(filename, artifactory_url=artifactory_default_url):
    url = artifactory_url + '/' + artifactory_repo + '/' + filename
    """Check to see if a file already exists on our artifactory."""

    print("URL To Check: " + url)

    r = requests.head(url)
    # If we were able to download the file return the version hash from within it.
    # otherwise let the user know we didn't find the file.
    if r.status_code == 404:
        error = "No File Found at: " + url
        return False
    else:
        print("File exists at: " + url)
        return url
