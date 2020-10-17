import requests
import sys
import os
import subprocess


def CloneGitRepo_GitHub(githubURL, branch, output=""):
    subprocess.check_output("cd " + output + " && git clone " + githubURL + " -b " + branch + " ",
                            shell=True)

    subprocess.check_output("cd " + output + " && dir",
                            shell=True)
