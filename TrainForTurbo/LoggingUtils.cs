namespace TrainForTurbo
{
    public static class LoggingUtils
    {
        public static void LogInfo(string _log) { Debug.Log($"[{ModInfo.ModName}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{ModInfo.ModName}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{ModInfo.ModName}] " + _log); }
    }
}
