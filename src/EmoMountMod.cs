using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using SideLoader;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EmoMount
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public partial class EmoMountMod : BaseUnityPlugin
    {
        public const string GUID = "emo.mountmod";
        public const string NAME = "EmoMountMod";
        public const string VERSION = "1.0.3";

        public const string MOUNT_DISMOUNT_KEY = "MountMod_Dismount";
        public const string MOUNT_FOLLOW_WAIT_TOGGLE = "MountMod_FollowWait_Toggle";
        public const string MOUNT_MOVE_TO_KEY = "MountMod_MoveTo_Toggle";

        public static int[] MountWhistleIDs = new int[]
        {
            -26100,
            -26101,
            -26102,
            -26103,
            -26104,
            -26105,
            -26106,
            -26107,
            -26108,
            -26109,
            -26110,
            -26111,
            -26112,
            -26113,
            -26114,
            -26115,
            -26116,
            -26117,
            -26118
        };

        public static float BAG_LOAD_DELAY = 10f;
        public static float SCENE_LOAD_DELAY = 10f;
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


        public static string RootFolder;

        public static ConfigEntry<float> WorldDropChanceThreshold;
        public static ConfigEntry<float> WorldDropChanceMinimum;
        public static ConfigEntry<float> WorldDropChanceMaximum;


        public static ConfigEntry<bool> EnableFoodNeed;
        public static ConfigEntry<bool> EnableWeightLimit;
        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            //this cannot be the way lol
            RootFolder = this.Info.Location.Replace("EmoMount.dll", "");
            InitKeybinds();
            MountManager = new MountManager(RootFolder);
            SL.OnPacksLoaded += InitializeCanvas;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;


            WorldDropChanceThreshold = Config.Bind<float>(NAME, "Drop Threshold", 1, "You need to roll this number or less in order for a whistle to drop.");
            WorldDropChanceMinimum = Config.Bind<float>(NAME, "Drop Chance Range Minimum", 0, "Minimum number to roll between");
            WorldDropChanceMaximum = Config.Bind<float>(NAME, "Drop Chance Range Maximum", 500, "Maximum number to roll between");

            EnableFoodNeed = Config.Bind<bool>(NAME, "Enable Food Needs", true, "Enables the Mount food system.");
            EnableWeightLimit = Config.Bind<bool>(NAME, "Enable Weight Limits", true, "Enables the Mount weight limit system.");

            SetupNPCs();
            new Harmony(GUID).PatchAll();
        }



        private void InitializeCanvas()
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

        private void InitKeybinds()
        {
            CustomKeybindings.AddAction(MOUNT_DISMOUNT_KEY, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_FOLLOW_WAIT_TOGGLE, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_MOVE_TO_KEY, KeybindingsCategory.CustomKeybindings);
        }
        private void SceneManager_sceneLoaded(Scene Scene, LoadSceneMode LoadMode)
        {
            if (Scene.name == "MainMenu_Empty")
            {
                MountManager.DestroyAllMountInstances();
            }
        }

        private void SetupNPCs()
        {
            SetupLevantNPC();
            SetupBergNPC();
            SetupCierzoNPC();
            SetupMonsoonNPC();
        }

        private void SetupLevantNPC()
        {
            ///levant
            DialogueCharacter levantGuard = new()
            {
                UID = "emomount.mountcharacterlevant",
                Name = "Ianis, Levant Stable Master",
                SpawnSceneBuildName = "Levant",             
                SpawnPosition = new(-39.7222f, 0.2239f, 120.0354f),
                SpawnRotation = new(0, 218f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
            };


            // Create and apply the template
            var template = levantGuard.CreateAndApplyTemplate();

            // Add a listener to set up our dialogue
            levantGuard.OnSetupDialogueGraph += TestCharacter_OnSetupDialogueGraph;

            // Add this func to determine if our character should actually spawn
            template.ShouldSpawn = () => true;
        }
        private void SetupBergNPC()
        {
            ///berg
            DialogueCharacter BergNPC = new()
            {
                UID = "emomount.mountcharacterberg",
                Name = "Iggy the Wild, Berg Stable Master",
                SpawnSceneBuildName = "Berg",
                SpawnPosition = new(1191.945f, -13.7222f, 1383.581f),
                SpawnRotation = new(0, 72f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
            };


            // Create and apply the template
            var bergTemplate = BergNPC.CreateAndApplyTemplate();
            // Add a listener to set up our dialogue
            BergNPC.OnSetupDialogueGraph += TestCharacter_OnSetupDialogueGraph;

            // Add this func to determine if our character should actually spawn
            bergTemplate.ShouldSpawn = () => true;
        }
        private void SetupCierzoNPC()
        {
            ///ciezro
            DialogueCharacter CierzoNPC = new()
            {
                UID = "emomount.mountcharactercierzo",
                Name = "Emo, Cierzo Stable Master",
                SpawnSceneBuildName = "CierzoNewTerrain",      
                SpawnPosition = new(1421.29f, 5.5604f, 1686.195f),
                SpawnRotation = new(0, 270f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
            };


            // Create and apply the template
            var cierzotemplate = CierzoNPC.CreateAndApplyTemplate();

            // Add a listener to set up our dialogue
            CierzoNPC.OnSetupDialogueGraph += TestCharacter_OnSetupDialogueGraph;

            // cierzotemplate this func to determine if our character should actually spawn
            cierzotemplate.ShouldSpawn = () => true;
        }
        private void SetupMonsoonNPC()
        {

            ///monsoon
            DialogueCharacter MonsoonNPC = new()
            {
                UID = "emomount.mountcharactermonsoon",
                Name = "Faeryn, Monsoon Stable Master",
                SpawnSceneBuildName = "Monsoon",
                SpawnPosition = new(82.0109f, -5.1698f, 140.1947f),
                SpawnRotation = new(0, 254.089f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                CharVisualData =
                {
                    Gender =  Character.Gender.Female
                },
                StartingPose = Character.SpellCastType.IdleAlternate,
            };


            // Create and apply the template
            var monsoontemplate = MonsoonNPC.CreateAndApplyTemplate();

            // Add a listener to set up our dialogue
            MonsoonNPC.OnSetupDialogueGraph += TestCharacter_OnSetupDialogueGraph;

            // cierzotemplate this func to determine if our character should actually spawn
            monsoontemplate.ShouldSpawn = () => true;
        }

        private void TestCharacter_OnSetupDialogueGraph(DialogueTree graph, Character character)
        {
            BuildDialouge(graph, character);
        }

        private void BuildDialouge(DialogueTree graph, Character character)
        {
            var ourActor = graph.actorParameters[0];

            // Add our root statement
            var InitialStatement = graph.AddNode<StatementNodeExt>();
            InitialStatement.statement = new($"Welcome, can I store a mount for you perphaps in our stables?");
            InitialStatement.SetActorName(ourActor.name);

            // Add a multiple choice
            var multiChoice1 = graph.AddNode<MultipleChoiceNodeExt>();

            MultipleChoiceNodeExt.Choice DismissChoice = new MultipleChoiceNodeExt.Choice()
            {
                statement = new Statement("Can you look after my current mount?")
            };

            MultipleChoiceNodeExt.Choice SummonChoice = new MultipleChoiceNodeExt.Choice()
            {
                statement = new Statement("I want to retrieve a mount.")
            };

            MultipleChoiceNodeExt.Choice LearnSkillsChoice = new MultipleChoiceNodeExt.Choice()
            {
                statement = new Statement("Can you teach me some mount skills?")
            };

            multiChoice1.availableChoices.Add(DismissChoice);
            multiChoice1.availableChoices.Add(SummonChoice);
            multiChoice1.availableChoices.Add(LearnSkillsChoice);

            // Add our answers
            var answer1 = graph.AddNode<StatementNodeExt>();
            answer1.statement = new("Aye, that I can do.");
            answer1.SetActorName(ourActor.name);

            var answer2 = graph.AddNode<StatementNodeExt>();
            answer2.statement = new("Here's a list of what you have in my stables.");
            answer2.SetActorName(ourActor.name);

            var answer3 = graph.AddNode<StatementNodeExt>();
            answer3.statement = new("Heres watcha do...");
            answer3.SetActorName(ourActor.name);

            DismissMountActionNode dismissMountActionNode = new DismissMountActionNode();
            DisplayMountStorageListNode displayStorageListNode = new DisplayMountStorageListNode();
            LearnMountSkillsNode learnMountSkillsNode = new LearnMountSkillsNode();

            // ===== finalize nodes =====
            graph.allNodes.Clear();
            // add the nodes we want to use
            graph.allNodes.Add(InitialStatement);
            graph.primeNode = InitialStatement;
            graph.allNodes.Add(multiChoice1);
            graph.allNodes.Add(answer1);
            graph.allNodes.Add(answer2);
            graph.allNodes.Add(answer3);
            graph.allNodes.Add(dismissMountActionNode);
            graph.allNodes.Add(displayStorageListNode);
            graph.allNodes.Add(learnMountSkillsNode);
            // setup our connections
            graph.ConnectNodes(InitialStatement, multiChoice1);    // prime node triggers the multiple choice


            graph.ConnectNodes(multiChoice1, answer1, 0);       // choice1: answer1
            graph.ConnectNodes(answer1, dismissMountActionNode);
            graph.ConnectNodes(answer1, InitialStatement);         // - choice1 goes back to root node

            graph.ConnectNodes(multiChoice1, answer2, 1);      
            graph.ConnectNodes(answer2, displayStorageListNode);
            graph.ConnectNodes(answer2, InitialStatement);

            graph.ConnectNodes(multiChoice1, answer3, 2);
            graph.ConnectNodes(answer3, learnMountSkillsNode);
            graph.ConnectNodes(answer3, InitialStatement);
        }
        public static int GetRandomWhistleID()
        {
            return EmoMountMod.MountWhistleIDs[UnityEngine.Random.Range(0, EmoMountMod.MountWhistleIDs.Length)];
        }
    }
}
