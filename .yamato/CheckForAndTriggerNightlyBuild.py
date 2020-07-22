import os
import utility.versionCheck
import utility.YamatoJobs


def main():
    unity_version = utility.versionCheck.check_for_new_version(os.getenv("unityVersion"))
    print("New unity version:" + unity_version)
    if unity_version != "":
        project_id = utility.YamatoJobs.get_project_id("xr.xrautomatedtests")
        utility.YamatoJobs.start_yamato_job(project_id, os.getenv("yamatoFileName"), os.getenv("yamatoBranch"), os.getenv("yamatoJobName"))


if __name__ == "__main__":
    main()
