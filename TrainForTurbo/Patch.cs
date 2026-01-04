using System.Linq;

namespace TrainForTurbo
{
    [HarmonyPatch]
    public class Patch
    {
        // A single reusable lookup table for all settings
        static readonly Dictionary<int, List<string>> BaseSettingNames = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "Advertising" } },

            { 2, new List<string>
                {
                    "Medium Groups",
                    "Individual Dining",
                    "Large Groups",
                    "Flexible Dining"
                }
            },

            { 3, new List<string>
                {
                    "Morning Rush",
                    "Lunch Rush",
                    "Dinner Rush",
                    "Closing Time"
                }
            },

            { 4, new List<string>
                {
                    "All You Can Eat",
                    "Double Helpings",
                    "High Expectations",
                    "Leisurely Eating",
                    "Personalised Waiting",
                    "Picky Eaters"
                }
            }
        };


        [HarmonyPatch(typeof(CreateNewKitchen), "OnUpdate")]
        [HarmonyPrefix]
        static void OnKitchenCreation()
        {
            LogInfo("Patching kitchen");

            int modEnabled = TrainForTurbo.PrefManager.Get<int>(TrainForTurbo.ModEnabledPreferenceKey);
            if (modEnabled == 0)
                return;

            LogInfo("Kitchen created - day" + GetDay());

            // Build card list
            List<int> cards = GetAllowedCardIDs(modEnabled);
            cards.ShuffleInPlace();
            cards.Take(2);

            CreateProgressionOption(cards);
        }

        static HashSet<string> GetAllowedNamesForSetting(int setting)
        {
            HashSet<string> names = new HashSet<string>();

            for (int i = 1; i <= setting; i++)
            {
                if (BaseSettingNames.TryGetValue(i, out var list))
                {
                    foreach (var name in list)
                        names.Add(name);
                }
            }

            return names;
        }


        // -----------------------------
        // ECS Helpers
        // -----------------------------

        static int GetDay()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = em.CreateEntityQuery(typeof(SDay));
            if (!query.IsEmpty)
            {
                SDay day = query.GetSingleton<SDay>();
                return day.Day;
            }

            return -1;
        }
        static List<int> GetAllowedCardIDs(int setting)
        {
            HashSet<string> allowedNames = GetAllowedNamesForSetting(setting);
            List<int> result = new List<int>();

            foreach (var gdo in GameData.Main.Get<GameDataObject>())
            {
                if (gdo is Unlock unlock)
                {
                    if (allowedNames.Contains(unlock.Name))
                    {
                        result.Add(unlock.ID);
                    }
                }
            }

            return result;
        }


        static void CreateProgressionOption(List<int> ids)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            int id0 = ids[0];
            int id1 = ids[1];

            // --- Create first option ---
            Entity e0 = em.CreateEntity(typeof(CProgressionOption));
            em.AddComponentData(e0, new CUnlockSelectPopupType { RewardType = UnlockRewardType.Subcard });
            em.SetComponentData(e0, new CProgressionOption { ID = id0, FromFranchise = false });
            LogInfo($"Created CProgressionOption entity for card id {id0}");

            // --- Create second option ---
            Entity e1 = em.CreateEntity(typeof(CProgressionOption));
            em.AddComponentData(e1, new CUnlockSelectPopupType { RewardType = UnlockRewardType.Subcard });
            em.SetComponentData(e1, new CProgressionOption { ID = id1, FromFranchise = false });
            LogInfo($"Created CProgressionOption entity for card id {id1}");
        }

    }
}