import os
import utility.versionCheck
import utility.YamatoJobs
import utility.JenkinsJob as JenkinsJob

commitParamName = "ghprbActualCommit"

def main():
    unity_version = utility.versionCheck.check_for_new_package_version(os.getenv("ghprbActualCommit"),
                                                                       os.getenv("packageName"),
                                                                       os.getenv("packageRepoURL"))
    print("New Package Version:" + unity_version)
    if unity_version != "" or os.getenv("forceStartJenkinsJob") == "True":

        project_name = os.getenv("jenkinsJob")
        # Retrieve the UnityVERSION, and then determine what type of version it is. (IE: BranchName, UnityVersion, or UnityRevision)
        version = os.getenv("unityVERSION")
        versionType = JenkinsJob.parse_version_for_jenkins(version)
        # Set the versionType and Version for passing to the Jenkins job.

        commitValue = os.getenv(commitParamName)

        params = {versionType: version, commitParamName: commitValue}

        # Invoke the Jenkins Job, and wait for it to complete before returning.
        result = JenkinsJob.start_jenkins_job(project_name, params, False, False)

if __name__ == "__main__":
    main()
