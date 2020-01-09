using com.unity.xr.test.prepare;
using UnityEditor;

public class XRCmdlineSetup
{

    [MenuItem("Tests/CmdlineSetup")]
    public static void CommandLineSetup()
    {
        XrTestPrepare xrTestPrepare = new XrTestPrepare();
        xrTestPrepare.EnsureCorrectXrPackagesAndScriptDefines();
    }
}