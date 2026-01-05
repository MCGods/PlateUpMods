using Kitchen;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
/**

Picky Eaters:-2040314977
Double Helpings:2055765569
All You Can Eat:-347199069
Personalised Waiting:233335391
Leisurely Eating:-287956430
Dinner Rush:-37551439
Flexible Dining:-2112255403
Individual Dining:-1747821833
Large Groups:-523195599
Medium Groups:-1183014556
Lunch Rush:-53330922
Advertising: 73387665
Advertising: 1765310572
Morning Rush:2079763934
High Expectations:-534291083
*/
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

        static List<int> AvailableCards = new List<int>();
        static int CardsChosen = 0;

        [HarmonyPatch(typeof(HandleUnlockChoice), "OnUpdate")]
        [HarmonyPostfix]
        static void AfterUnlockChoice(HandleUnlockChoice __instance)
        {
            var em = __instance.EntityManager;

            // Query the popup entity
            EntityQuery q = em.CreateEntityQuery(
                typeof(CPopup),
                typeof(CUnlockSelectPopupOption),
                typeof(CUnlockSelectPopupResult)
            );

            if (q.IsEmpty)
            {
                return;
            }

            // There should be exactly one popup entity
            Entity popup = q.GetSingletonEntity();

            // Read the options buffer
            DynamicBuffer<CUnlockSelectPopupOption> options =
                em.GetBuffer<CUnlockSelectPopupOption>(popup);

            // Read the result
            CUnlockSelectPopupResult result =
                em.GetComponentData<CUnlockSelectPopupResult>(popup);

            int selectedIndex = result.Selection.Index;
            foreach (var item in options)
            {
                if (selectedIndex == item.Entity.Index)
                {
                    LogInfo($"Player chose card ID: {item.ID}");
                    var success = RemoveCardFromPool(item.ID);
                    if (success)
                    {
                        CardsChosen++;

                    }
                }
            }
        }


        [HarmonyPatch(typeof(CreateNewKitchen), "OnUpdate")]
        [HarmonyPostfix]
        static void OnKitchenCreation()
        {
            LogInfo("Patching kitchen");

            int modEnabled = TrainForTurbo.PrefManager.Get<int>(TrainForTurbo.ModEnabledPreferenceKey);
            if (modEnabled == 0)
                return;

            LogInfo("Kitchen created - day" + GetDay());
            CardsChosen = 0;

            // Build card list
            AvailableCards = GetAllowedCardIDs(modEnabled);
            AvailableCards.ShuffleInPlace();

            ChooseCards();

            // Queue second set for next frame
            //_ = DelayNextFrame(() => { ChooseCards(); });
        }

        [HarmonyPatch(typeof(CreateUnlockChoicePopup), "OnUpdate")]
        [HarmonyPostfix]
        static void AfterPopupClosed(CreateUnlockChoicePopup __instance)
        {
            var em = __instance.EntityManager;

            // If a popup still exists, do nothing
            EntityQuery popupQuery = em.CreateEntityQuery(
                typeof(CPopup),
                typeof(CUnlockSelectPopupOption)
            );

            if (!popupQuery.IsEmpty)
                return;

            // If no popup exists AND we have more cards to show, spawn next pair
            int modEnabled = TrainForTurbo.PrefManager.Get<int>(TrainForTurbo.ModEnabledPreferenceKey);
            int numOfCardsSetting = TrainForTurbo.PrefManager.Get<int>(TrainForTurbo.CardsNumberPreferenceKey);
            if (modEnabled != 0 && CardsChosen < numOfCardsSetting  && AvailableCards.Count >= 2)
            {
                LogInfo("Another card");
                ChooseCards();
            }
        }

        static void ChooseCards()
        {
            LogInfo($"Cards available: {AvailableCards.Count}");
            int count = Math.Min(2, AvailableCards.Count);
            List<int> choice = AvailableCards.Take(count).ToList();
            CreateProgressionOption(choice);
        }
        static async Task DelayNextFrame(Action action)
        {
            await Task.Yield(); // waits one frame
            action();
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

        static bool RemoveCardFromPool(int id)
        {
            LogInfo($"Card removed: {id}");
            return AvailableCards.Remove(id);
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
            if (ids.Count < 2)
            {
                return;
            }

            int id0 = ids[0];
            int id1 = ids[1];
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity e = em.CreateEntity(typeof(CSubcardChoice));
            em.SetComponentData(e,
                new CSubcardChoice
                {
                    Choice1 = id0,
                    Choice2 = id1,
                    FromFranchise = false
                });
            LogInfo($"Created CSubcardChoice: {id0}, {id1}");
        }
    }
}