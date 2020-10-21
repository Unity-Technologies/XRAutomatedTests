import requests
import sys
import os
import subprocess


def CloneGitRepo_GitHub(githubURL, branch, output=""):

    CloneCMD = "git clone " + githubURL + " -b " + branch + ""
    print("Cloing Git Repo With Command: " + CloneCMD)
    print("Cloning Into Folder at: "+output)

    subprocess.check_output("cd " + output + " && "+CloneCMD,
                            shell=True)
