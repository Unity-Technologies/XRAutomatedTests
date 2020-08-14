import os
import utility.versionCheck
import utility.YamatoJobs
import utility.JenkinsJob as JenkinsJob


def main():
    unity_version = utility.versionCheck.check_for_new_package_version(os.getenv("PackageRepoBranch"),
                                                                       os.getenv("packageName"),
                                                                       os.getenv("packageRepoURL"))
    print("New Package Version:" + unity_version)
    if unity_version != "" or os.getenv("forceStartJenkinsJob") == "True":
        project_name = os.getenv("jenkinsJob")
        params = {}

        # Invoke the Jenkins Job, and wait for it to complete before returning.
        JenkinsJob.start_jenkins_job(project_name, params, False, False)


if __name__ == "__main__":
    main()
