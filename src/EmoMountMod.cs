using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NodeCanvas.DialogueTrees;
using SideLoader;
using System;
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
        public const string NAME = "Emo's Mount Mod";
        public const string VERSION = "1.0.0";

        public const string MOUNT_DISMOUNT_KEY = "MountMod_Dismount";
        public const string MOUNT_FOLLOW_WAIT_TOGGLE = "MountMod_FollowWait_Toggle";
        public const string MOUNT_MOVE_TO_KEY = "MountMod_MoveTo_Toggle";

        public static bool Debug = true;

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
            SetupTestCharacter();
            new Harmony(GUID).PatchAll();
        }

        private void SceneManager_sceneLoaded(Scene Scene, LoadSceneMode LoadMode)
        {
            if (Scene.name == "MainMenu_Empty")
            {
                MountManager.DestroyAllMountInstances();
            }
        }


        private void InitKeybinds()
        {
            CustomKeybindings.AddAction(MOUNT_DISMOUNT_KEY, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_FOLLOW_WAIT_TOGGLE, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_MOVE_TO_KEY, KeybindingsCategory.CustomKeybindings);
        }

        internal static DialogueCharacter levantGuard;
        private void SetupTestCharacter()
        {
            levantGuard = new()
            {
                UID = "emomount.mountcharacter",
                Name = "Levant Guard",
                SpawnSceneBuildName = "Abrassar",
                SpawnPosition = new(-159.4f, 131.8f, -532.7f),
                SpawnRotation = new(0, 43.7f, 0),
                HelmetID = 3000115,
                ChestID = 3000112,
                BootsID = 3000118,
                WeaponID = 2130305,
                StartingPose = Character.SpellCastType.IdleAlternate,
            };
            

            // Create and apply the template
            var template = levantGuard.CreateAndApplyTemplate();

            // Add a listener to set up our dialogue
            levantGuard.OnSetupDialogueGraph += TestCharacter_OnSetupDialogueGraph;

            // Add this func to determine if our character should actually spawn
            template.ShouldSpawn = () => true;
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
            InitialStatement.statement = new("Yo dwag, I heard yu liek dogs so I got dogs.");
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


            multiChoice1.availableChoices.Add(DismissChoice);
            multiChoice1.availableChoices.Add(SummonChoice);


            // Add our answers
            var answer1 = graph.AddNode<StatementNodeExt>();
            answer1.statement = new("Aye, that I can do.");
            answer1.SetActorName(ourActor.name);

            var answer2 = graph.AddNode<StatementNodeExt>();
            answer2.statement = new("Take a look at what you have stored.");
            answer2.SetActorName(ourActor.name);

            DismissMountActionNode dismissMountActionNode = new DismissMountActionNode();
            DisplayMountStorageListNode displayStorageListNode = new DisplayMountStorageListNode();

            // ===== finalize nodes =====
            graph.allNodes.Clear();
            // add the nodes we want to use
            graph.allNodes.Add(InitialStatement);
            graph.primeNode = InitialStatement;
            graph.allNodes.Add(multiChoice1);
            graph.allNodes.Add(answer1);
            graph.allNodes.Add(answer2);
            graph.allNodes.Add(dismissMountActionNode);
            graph.allNodes.Add(displayStorageListNode);
            // setup our connections
            graph.ConnectNodes(InitialStatement, multiChoice1);    // prime node triggers the multiple choice


            graph.ConnectNodes(multiChoice1, answer1, 0);       // choice1: answer1
            graph.ConnectNodes(answer1, dismissMountActionNode);
            graph.ConnectNodes(answer1, InitialStatement);         // - choice1 goes back to root node

            graph.ConnectNodes(multiChoice1, answer2, 1);      
            graph.ConnectNodes(answer2, displayStorageListNode);
            graph.ConnectNodes(answer2, InitialStatement);
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
    }
}
