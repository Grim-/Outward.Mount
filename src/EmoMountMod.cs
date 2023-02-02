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
        public const string VERSION = "1.0.4";

        public const string MOUNT_FOLLOW_WAIT_TOGGLE = "MountMod_FollowWait_Toggle";
        public const string MOUNT_MOVE_TO_KEY = "MountMod_MoveTo_Toggle";

        public int[] MountWhistleIDs = new int[]
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


        public static string RootFolder;

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

        public static EmoMountMod Instance;
        public static MountQuestManager QuestManager;

        public Action<float> OnGameHourPassed;


        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Instance = this;
            QuestManager = new MountQuestManager();
            //this cannot be the way lol
            RootFolder = this.Info.Location.Replace("EmoMount.dll", "");
            InitKeybinds();

            SL.OnPacksLoaded += InitializeCanvas;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            BindConfigEntries();

            MountManager = new MountManager(RootFolder);

            SetupNPCs();
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

        private void InitializeCanvas()
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
            CustomKeybindings.AddAction(MOUNT_FOLLOW_WAIT_TOGGLE, KeybindingsCategory.CustomKeybindings, ControlType.Both);
            CustomKeybindings.AddAction(MOUNT_MOVE_TO_KEY, KeybindingsCategory.CustomKeybindings, ControlType.Both);
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
            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(graph);


            //You must always set the intial statement
            StatementNodeExt InitialStatement =  dialogueTreeBuilder.SetInitialStatement("Welcome traveler.");


            //Now you can add the next node
            MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "Can you look after my current mount?",
                "I want to retrieve a mount.",
                "Can you teach me some mount skills?",
                "Can you recolor my mount?"
            },
            new ConditionTask[]
            {                
                new CharacterHasActiveMountConditionNode(),
                new CharacterHasNOActiveMountConditionNode(),
                null,
                new CharacterHasActiveMountConditionNode()
             });
   
            //you must connect the inital statement and inital node
            graph.ConnectNodes(InitialStatement, initialChoice);

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, "Aye, that I can do.", new DismissMountActionNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Here's a list of what you have in my stables.", new DisplayMountStorageListNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 2, "Heres watcha do...", new LearnMountSkillsNode());

            MultipleChoiceNodeExt secondChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "Red?",
                "Green?",
                "Blue?",
                "Purple?"
            });


            //Last option of the first multi-choice takes you to the second multi choice
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 3, "Heres watcha do...", secondChoice);

            dialogueTreeBuilder.AddAnswerToMultipleChoice(secondChoice, 0, "Yup", new SetMountColor(Color.red)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(secondChoice, 1, "Yup", new SetMountColor(Color.green)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(secondChoice, 2, "Yup", new SetMountColor(Color.blue)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(secondChoice, 3, "Yup", new SetMountColor(Color.magenta)).ConnectTo(graph, new FinishNode());

        }
        private void Old_BuildDialouge(DialogueTree graph, Character character)
        {
            var ourActor = graph.actorParameters[0];
            // Add our root statement
            var InitialStatement = graph.AddNode<StatementNodeExt>();
            InitialStatement.statement = new($"Welcome, can I store a mount for you in our stables?");
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


            //var multiChoice2 = graph.AddNode<MultipleChoiceNodeExt>();

            //MultipleChoiceNodeExt.Choice choice1 = new MultipleChoiceNodeExt.Choice()
            //{
            //    statement = new Statement("Multiple Choice 1")
            //};

            //var answertochoiceOne = graph.AddNode<StatementNodeExt>();
            //answertochoiceOne.statement = new("Reply to choice 1");
            //answertochoiceOne.SetActorName(ourActor.name);

            //multiChoice2.availableChoices.Add(choice1);
            //graph.allNodes.Add(multiChoice2);
            //graph.allNodes.Add(answertochoiceOne);

            //graph.ConnectNodes(multiChoice1, multiChoice2, 0);
            //graph.ConnectNodes(multiChoice2, answertochoiceOne, 0);

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


            // add ALLL the nodes we want to use, remember this is a literal graph, the nodes must be on the graph to draw connections between them
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
            graph.ConnectNodes(InitialStatement, multiChoice1);    // Connect Initial node to MultiChoice node


            graph.ConnectNodes(multiChoice1, answer1, 0);        //connect MultiChoice port 0 to answer 1
            graph.ConnectNodes(answer1, dismissMountActionNode); //connect answer 1 to dismiss mount node
            graph.ConnectNodes(answer1, InitialStatement);         // go back to start

            graph.ConnectNodes(multiChoice1, answer2, 1);      ///repeat for answer 2 and its actions
            graph.ConnectNodes(answer2, displayStorageListNode);
            graph.ConnectNodes(answer2, InitialStatement);

            graph.ConnectNodes(multiChoice1, answer3, 2);
            graph.ConnectNodes(answer3, learnMountSkillsNode);
            graph.ConnectNodes(answer3, InitialStatement);

        }

        public static int GetRandomWhistleID()
        {
            return EmoMountMod.Instance.MountWhistleIDs[UnityEngine.Random.Range(0, EmoMountMod.Instance.MountWhistleIDs.Length)];
        }
    }
}
