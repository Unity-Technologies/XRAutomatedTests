import requests
import sys
import os
import subprocess


def install_unity(version, includeAndroid="False", includeUWP="False", includeIL2CPP="False", includeIOS="False"):
    """Download and install the requested Unity version with the requested components.
        This requires a pre-install of unity-downloader-cli.
        A future improvement would be to install unity-downloader-cli if it hasn't been."""

    components = "-c editor"
    if includeAndroid.lower() == "true":
        components = components + " -c Android"
    if includeUWP.lower() == "true":
        components = components + " -c UWP"
    if includeIL2CPP.lower() == "true":
        components = components + " -c StandaloneSupport-IL2CPP"
    if includeIOS.lower() == "true":
        components = components + " -c iOS"

    subprocess.check_output("unity-downloader-cli -u " + version + " " + components + " --wait --published",
                            shell=True)


def download_utr():
    """Download the latest version of UTR.bat from the internal artifactory."""
    url = 'https://artifactory.internal.unity3d.com/core-automation/tools/utr-standalone/utr.bat'

    print("Downloading UTR from: " + url)
    utr_bat = requests.get(url)
    if utr_bat.status_code == 200:
        output = os.getcwd() + '/utr.bat'
        open(output, 'wb').write(utr_bat.content)
        print("Done downloading UTR it can be found at: " + output)
    else:
        print("Failed to download UTR.bat from" + url)
        print("Request returned status code:" + utr_bat.status_code)
        exit(1)


def build_player(cmd):
    """Build a player with the setting specified by the cmd variable."""
    download_utr()
    command = "utr.bat " + cmd
    print("cmd: " + command)
    print(subprocess.check_output(command, shell=True))
