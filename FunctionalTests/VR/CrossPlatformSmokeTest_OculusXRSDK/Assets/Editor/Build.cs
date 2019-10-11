using com.unity.cliconfigmanager;

namespace Assets.Editor
{
    public class Build
    {
        public static void CommandLineSetup()
        {
            var cliConfigManager = new CliConfigManager();
            cliConfigManager.ConfigureFromCmdlineArgs();
        }
    }
}
