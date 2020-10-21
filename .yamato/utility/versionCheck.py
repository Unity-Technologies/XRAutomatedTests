import sys
import subprocess
import utility.JenkinsJob
import utility.ArtifactoryFileTransferManager
import re


def check_for_new_version(branch, isPackage=False):
    """Check to see if there is a new version of the specified branch."""

    # Set a file name appriopriate to that branch.
    filename = "last_unity_" + branch
    # Cleanup the filename
    filename = re.sub(r'[\\/:"*?<>|]+', "", filename)
    # download the file containing the last checked hash if it exxists in our artifactory repo.
    last_checked_version = utility.ArtifactoryFileTransferManager.download_hash_file(filename)

    # get the latest unity version.
    latest_version = get_latest_version(branch)
    # If there isn't a new version exit with that status.
    if last_checked_version == latest_version:
        print("No new version! Exiting!")
        return ""

    # If there is a new version, update the hash file and upload it.
    # Objectively, we should consider doing this at the end of a successful test run,
    # rather than immediately when a new version is detected. Though I can see arguements
    # for both approaches.
    print("New version detected: " + latest_version)

    new_version_file = open(filename, "w+")
    new_version_file.write(latest_version)
    new_version_file.close()
    utility.ArtifactoryFileTransferManager.upload_file(filename)

    return latest_version


def get_latest_version(branch):
    """Get the hash of the latest version of the specified branch."""
    latest_unity_version = subprocess.check_output(
        "unity-downloader-cli -u " + branch + " -c editor --skip-download --fast", shell=True).strip()
    latest_unity_version = str(latest_unity_version)[2:-1]
    return latest_unity_version


def check_for_new_package_version(branch, packageName, packageRepoURL):
    """Check to see if there is a new version of a package at packageRepoURL with the specified branch."""

    # Set a file name appriopriate to that branch and packageName.
    filename = "last_package_" + packageName + "_" + branch
    # Cleanup the filename
    filename = re.sub(r'[\\/:"*?<>|]+', "", filename)
    # download the file containing the last checked hash if it exxists in our artifactory repo.
    last_checked_version = utility.ArtifactoryFileTransferManager.download_hash_file(filename)

    # get the latest unity version.
    latest_version = get_latest_package_version(branch, packageRepoURL)
    # If there isn't a new version exit with that status.
    if last_checked_version == latest_version:
        print("No new version of package " + packageName + "! Exiting!")
        return ""

    # If there is a new version, update the hash file and upload it.
    print("New version detected: " + latest_version)

    new_version_file = open(filename, "w+")
    new_version_file.write(latest_version)
    new_version_file.close()
    utility.ArtifactoryFileTransferManager.upload_file(filename)

    return latest_version


def get_latest_package_version(branch, packageRepoURL):
    """Get the hash of the latest version of the specified branch at packageRepoURL."""
    command = "git ls-remote " + packageRepoURL + " " + branch
    latest_package_version = subprocess.check_output(
        command, shell=True).strip()
    latest_package_version = str(latest_package_version)[2:-1]
    print(latest_package_version)
    return latest_package_version
