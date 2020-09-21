namespace ChangeScalingBehavior
{
	internal static class Helpers
    {
        public static void ApplyChanges(string installPath, string resourceFileName)
        {
            var registerKeyMan = new RegistryKeyManager();
            registerKeyMan.AddExternalManifest();
            registerKeyMan.ExtractResourceFile("ChangeScalingBehavior", installPath, "Resources", resourceFileName);
        }
        public static void UndoChanges(ApplicationVersion appVersion, string installPath, string resourceFileName)
        {
            var registerKeyMan = new RegistryKeyManager();
            var resourceFileManager = new ResourceFilesManager();

            resourceFileManager.RemoveResourceFile(installPath + resourceFileName);
            if (resourceFileManager.NoStudioManifestFile(appVersion.StudioVersions) && 
                registerKeyMan.IsRegistryKey() 
                && resourceFileManager.NoMultiTermManifestFile(appVersion.MultiTermVersions))
                registerKeyMan.RemoveExternalManifest();
        }
    }
}
