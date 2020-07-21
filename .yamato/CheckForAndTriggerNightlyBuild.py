import os
import utility.versionCheck
import utility.YamatoJobs


def main():
    unity_version = utility.versionCheck.check_for_new_version("2019.4/staging")
    print("New unity version:" + unity_version)
    if unity_version != "":
        project_id = utility.YamatoJobs.get_project_id("xr.xrautomatedtests")
        utility.YamatoJobs.start_yamato_job(project_id, "2019.4-RunAllTests.yml", "2019.4", "2019.4_RunAllTests")


if __name__ == "__main__":
    main()
