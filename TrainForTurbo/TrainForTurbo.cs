namespace TrainForTurbo
{
    [UsedImplicitly]
    public class TrainForTurbo : GenericSystemBase, IModSystem
    {
        public static PreferenceSystemManager PrefManager;
        public static readonly string ModEnabledPreferenceKey = "TrainForTurbo_ModEnabledKey";

        public override void Initialise()
        {
            LogInfo($"v{ModInfo.ModVersion} in use!");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());


            PrefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            PrefManager.AddLabel("Day 1 card")
                        .AddOption(ModEnabledPreferenceKey, initialValue: 4, values: new int[] { 0, 1, 2, 3, 4 }, strings: new string[] { "Disabled", "Customers", "Customers and Groups", "Customers and Rush", "Harder Customers" })
                        .AddSpacer()
                        .AddSpacer();
            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);

            // Will apply the mod once per in game day
            RequireSingletonForUpdate<SIsDayFirstUpdate>();
        }

        // This will only apply the mod once per day when the day starts.
        protected override void OnUpdate()
        {
            LogInfo($" On update!");
            var modEnabled = PrefManager.Get<bool>(ModEnabledPreferenceKey);
        }
    }
}