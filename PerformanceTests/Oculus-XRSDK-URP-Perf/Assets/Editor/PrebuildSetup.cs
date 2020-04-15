using com.unity.cliconfigmanager;


namespace Assets.Editor
{
    public class RenderPerformancePrebuildStep
    {

        public static void Setup()
        {
            var cliConfigManager = new CliConfigManager();
            cliConfigManager.ConfigureFromCmdlineArgs();
        }
    }
}
