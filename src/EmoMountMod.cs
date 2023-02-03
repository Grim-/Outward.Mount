using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Conditions;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using EmoMount.Custom_SL_Effect;
namespace EmoMount
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public partial class EmoMountMod : BaseUnityPlugin
    {
        public const string GUID = "emo.mountmod";
        public const string NAME = "EmoMountMod";
        public const string VERSION = "1.1.0";

        public const string MOUNT_FOLLOW_WAIT_TOGGLE = "MountMod_FollowWait_Toggle";
        public const string MOUNT_MOVE_TO_KEY = "MountMod_MoveTo_Toggle";



        public static int SummonMountSkillID = -26200;
        public static int DismissMountSkillID = -26201;

        public static float BAG_LOAD_DELAY = 10f;
        public static float SCENE_LOAD_DELAY = 2f;
        public static bool Debug = false;

        internal static ManualLogSource Log;
        public static Canvas MainCanvas
        {
            get; private set;
        }
        public static MountCanvasManager MainCanvasManager
        {
            get; private set;
        }


        public static MountManager MountManager
        {
            get; private set;
        }

        public static string RootFolder
        {
            get; private set;
        }

        public static ConfigEntry<float> WorldDropChanceThreshold;
        public static ConfigEntry<float> WorldDropChanceMinimum;
        public static ConfigEntry<float> WorldDropChanceMaximum;

        public static ConfigEntry<float> LeashDistance;
        public static ConfigEntry<float> LeashRadius;

        public static ConfigEntry<float> FoodLostTravelling;
        public static ConfigEntry<float> TravelDistanceThreshold;

        public static ConfigEntry<float> EncumberenceSpeedModifier;


        public static ConfigEntry<bool> EnableFastMount;
        public static ConfigEntry<bool> EnableFoodNeed;
        public static ConfigEntry<bool> EnableWeightLimit;


        public static ConfigEntry<float> WeightLimitOverride;

        public static EmoMountMod Instance
        {
            get; private set;
        }
        public static MountQuestManager QuestManager
        {
            get; private set;
        }
        public static MountNPCManager NPCManager
        {
            get; private set;
        }

        public Action<float> OnGameHourPassed;


        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Instance = this;
            RootFolder = this.Info.Location.Replace("EmoMount.dll", "");

            QuestManager = new MountQuestManager();
            NPCManager = new MountNPCManager();
            MountManager = new MountManager(RootFolder);


            CustomKeybindings.AddAction(MOUNT_FOLLOW_WAIT_TOGGLE, KeybindingsCategory.CustomKeybindings, ControlType.Both);
            CustomKeybindings.AddAction(MOUNT_MOVE_TO_KEY, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            SL.OnPacksLoaded += ()=>
            {
                //CreateColorBerrys();
                CreateMainCanvas();
            };

            SceneManager.sceneLoaded += (Scene Scene, LoadSceneMode LoadMode) =>
            {
                if (Scene.name == "MainMenu_Empty")
                {
                    MountManager.DestroyAllMountInstances();
                }

            };

            BindConfigEntries();

            new Harmony(GUID).PatchAll();
        }

        private void BindConfigEntries()
        {
            WeightLimitOverride = Config.Bind<float>(NAME, "Weight Limit Override", 0, "If this value is anything other than 0 then all mounts will use the specified value as their weight limit instead of the values from the MountSpecies XML.");
            EncumberenceSpeedModifier = Config.Bind<float>(NAME, "Encumberence Speed Modifier Override", 0, "If this value is anything other than 0 then all mounts will use the specifiec value as their encumberence speed modifier (0.5 is 50% speed penalty) instead of the values from the MountSpecies XML.");

            LeashDistance = Config.Bind<float>(NAME, "Leash Distance", 6f, "The maximum distance allowed between you and your mount before it will attempt to move to you.");
            LeashRadius = Config.Bind<float>(NAME, "Leash Radius", 2.3f, "The mount will attempt to move to a random point in the specified radius around you.");

            TravelDistanceThreshold = Config.Bind<float>(NAME, "Travel Distance Threshold", 40f, "Everytime your mount covers this distance, food is taken.");
            FoodLostTravelling = Config.Bind<float>(NAME, "Food lost traveling", 2f, "The amount of food taken per 'travel' distance.");

            WorldDropChanceThreshold = Config.Bind<float>(NAME, "Drop Threshold", 1, "You need to roll this number or less in order for a whistle to drop.");
            WorldDropChanceMinimum = Config.Bind<float>(NAME, "Drop Chance Range Minimum", 0, "Minimum number to roll between");
            WorldDropChanceMaximum = Config.Bind<float>(NAME, "Drop Chance Range Maximum", 500, "Maximum number to roll between");

            EnableFastMount = Config.Bind<bool>(NAME, "Enable Fast Mount", false, "If enabled the summon mount skill will instantly summon and mount you.");
            EnableFoodNeed = Config.Bind<bool>(NAME, "Enable Food Needs", true, "Enables the Mount food system.");
            EnableWeightLimit = Config.Bind<bool>(NAME, "Enable Weight Limits", true, "Enables the Mount weight limit system.");
        }
        private void CreateMainCanvas()
        {
            Log.LogMessage("EmoMountMod Initalizing Canvas..");
            GameObject CanvasPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", "MountCanvas");

            if (CanvasPrefab != null)
            {
                MainCanvas = GameObject.Instantiate(CanvasPrefab).GetComponent<Canvas>();
                MainCanvasManager = MainCanvas.gameObject.AddComponent<MountCanvasManager>();
                DontDestroyOnLoad(MainCanvas);
                Log.LogMessage("EmoMountMod Canvas Initialized..");
            }
            else
            {
                Log.LogMessage("EmoMountMod CanvasPrefab was null");
            }
        }
        private void CreateColorBerrys()
        {
            Dictionary<string, Color> ColorChoices = new Dictionary<string, Color>();
            ColorChoices.Add("Red", Color.red);
            ColorChoices.Add("Green", Color.red);
            ColorChoices.Add("Blue", Color.blue);
            ColorChoices.Add("Cyan", Color.cyan);
            ColorChoices.Add("Purple", Color.magenta);
            ColorChoices.Add("Yellow", Color.yellow);
            ColorChoices.Add("White", Color.white);
            ColorChoices.Add("Black", Color.black);
            ColorChoices.Add("Reset", Color.clear);


            int StartingID = 26280;

            foreach (var item in ColorChoices)
            {
                SL_Item ColorBerryItem = new SL_Item()
                {
                    Target_ItemID = 4100320,
                    New_ItemID = -StartingID,
                    Name = $"Color Berry ({item.Key})",
                    Description = "Use this to change your mounts tint color.",
                    EffectTransforms = new SL_EffectTransform[]
                    {
                        new SL_EffectTransform()
                        {
                            TransformName = "Normal",
                            Effects = new SL_Effect[]
                            {
                                new SL_ChangeMountTint()
                                {
                                    TintColor = item.Value
                                }
                            }
                        }
                    }

                };

                ColorBerryItem.ApplyTemplate();
                StartingID++;
            }
        }
    }
}
