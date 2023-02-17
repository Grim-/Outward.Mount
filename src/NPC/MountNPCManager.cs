using EmoMount;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using NodeCanvas.Tasks.Conditions;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EmoMount.EmoMountMod;

namespace EmoMount
{
    public class MountNPCManager
    {

        public List<StableMaster> StableMasters = new List<StableMaster>()
        {
            new StableMaster()
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
                BuddySpecies = new List<string>()
                {
                    "PearlBird",
                    "SilverPearlBird",
                    "GoldenPearlBird",
                    "BlackPearlBird"
                },
                SpawnMountsInLineUp = true
            },
            new StableMaster()
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
                StartingPose = Character.SpellCastType.IdleAlternate
            },
            new StableMaster()
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
            },
            new StableMaster()
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
            },
            new StableMaster()
            {
                UID = "emomount.mountcharacterharmattan",
                Name = "Libre, Harmattan Stable Master",
                SpawnSceneBuildName = "Harmattan",
                SpawnPosition = new(-39.7222f, 0.2239f, 120.0354f),
                SpawnRotation = new(0, 218f, 0),
                HelmetID = 3100261,
                ChestID = 3100260,
                BootsID = 3100262,
                WeaponID = 2140080,
                StartingPose = Character.SpellCastType.IdleAlternate,
                CharVisualData = new SideLoader.SL_Character.VisualData()
                {
                    Gender = Character.Gender.Female,
                    SkinIndex = 2,
                    HairStyleIndex = 8,
                    HairColorIndex = 4,
                    HeadVariationIndex = 4
                }, BuddySpecies = new List<string>()
                {
                    "PearlBird"
                },
                SpawnMountsInLineUp = true
            },
            new StableMaster()
            {
                UID = "emomount.mountcharactercaldera",
                Name = "Schnabeldoktor, New Sirocco Stable Master",
                SpawnSceneBuildName = "NewSirocco",
                SpawnPosition = new(-39.7222f, 0.2239f, 120.0354f),
                SpawnRotation = new(0, 218f, 0),
                HelmetID = 3100261,
                ChestID = 3100260,
                BootsID = 3100262,
                WeaponID = 2140080,
                StartingPose = Character.SpellCastType.IdleAlternate,
                CharVisualData = new SideLoader.SL_Character.VisualData()
                {
                    Gender = Character.Gender.Female,
                    SkinIndex = 2,
                    HairStyleIndex = 8,
                    HairColorIndex = 4,
                    HeadVariationIndex = 4
                }
            }

        };

        public MountNPCManager()
        {
            CreateNPCs();
        }

        private void CreateNPCs()
        {
            foreach (var StableMsater in StableMasters)
            {
                SetupNPC(StableMsater);
            }
        }

        private void SetupNPC(StableMaster DialogueCharacter)
        {
            EmoMountMod.Log.LogMessage($"EmoMountMod :: Setting up NPC {DialogueCharacter.Name}");

            // Create and apply the template
            var template = DialogueCharacter.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                EmoMountMod.Log.LogMessage($"EmoMountMod :: Applying Template to  {SLTemplate.Name}");

                GameObject dialogueTemplate = GameObject.Instantiate(Resources.Load<GameObject>("editor/templates/DialogueTemplate"));
                dialogueTemplate.transform.parent = Character.transform;
                dialogueTemplate.transform.position = Character.transform.position;

                // set Dialogue Actor name
                DialogueActor ourActor = Character.GetComponentInChildren<DialogueActor>();
                ourActor.SetName(SLTemplate.Name);

                // setup dialogue tree
                DialogueTreeController graphController = Character.GetComponentInChildren<DialogueTreeController>();
                Graph graph = graphController.graph;

                // the template comes with an empty ActorParameter, we can use that for our NPC actor.
                List<DialogueTree.ActorParameter> actors = (graph as DialogueTree).actorParameters;
                actors[0].actor = ourActor;
                actors[0].name = ourActor.name;


                BuildDialouge((DialogueTree)graph, Character, DialogueCharacter.SellText, DialogueCharacter.SellableID, DialogueCharacter.SellPrice, DialogueCharacter.UniqueSellableQuestEventID, DialogueCharacter.UniqueSellableID);
                CreateBuddySpeciesForStableMaster(DialogueCharacter, Character);
            });

            template.ShouldSpawn = () => true;
        }

        private void CreateBuddySpeciesForStableMaster(StableMaster DialogueCharacter, Character Character)
        {
            if (DialogueCharacter.BuddySpecies != null && DialogueCharacter.BuddySpecies.Count > 0)
            {
                for (int i = 0; i < DialogueCharacter.BuddySpecies.Count; i++)
                {
                    if (!string.IsNullOrEmpty(DialogueCharacter.BuddySpecies[i]))
                    {
                        if (DialogueCharacter.SpawnMountsInLineUp)
                        {
                            SpawnCharacterMountBuddy(Character, DialogueCharacter.BuddySpecies[i], true, i);
                        }
                        else
                        {
                            SpawnCharacterMountBuddy(Character, DialogueCharacter.BuddySpecies[i]);
                        }

                    }
                }
            }
        }

        private void BuildDialouge(DialogueTree graph, Character character, string BuyText, int BuyItemID, int BuyItemCost, string UniqueCreatureEvent = "", int UniqueBuyItemID = -1)
        {
            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(graph);
            ((DialogueTreeExt)graph).SetCanBenInterrupted(false);

            //You must always set the intial statement
            StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement("Welcome traveler.");


            #region Initial Choice
            MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
                {
                    "Can you look after my current mount?",
                    "I want to retrieve a mount.",
                    "Can you teach me some mount skills?",
                    "I want to buy some color berries",
                    "Do you have any creatures for sale?",
                    "Do you have any unique creatures for sale?"
                },
                new ConditionTask[]
                    {
                    new CharacterHasActiveMountConditionNode(),
                    new CharacterHasNOActiveMountConditionNode(),
                    null,
                    null,
                    null,
                    null
                    //new HasQuestEventTriggered()
                    //{
                    //    EventUID = UniqueCreatureEvent
                    //}
                });



            //you must connect the inital statement and inital node
            graph.ConnectNodes(InitialStatement, initialChoice);
            #endregion

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, "Aye, that I can do.", new DismissMountActionNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Here's a list of what you have in my stables.", new DisplayMountStorageListNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 2, "Heres watcha do...", new LearnMountSkillsNode());

            #region COlor Berry Choice

            MultipleChoiceNodeExt ColorBerryChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "Red?",
                "Green?",
                "Blue?",
                "Cyan?",
                "Purple?",
                "Yellow?",
                "Black?",
                "Reset? (removes current tint if any)"
            });

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 3, "There are many available, take a look", ColorBerryChoice);

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 0, "Cheers", new GiveItem(-26280))
                 .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 1, "Cheers", new GiveItem(-26281))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 2, "Cheers", new GiveItem(-26282))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 3, "Cheers", new GiveItem(-26283))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 4, "Cheers", new GiveItem(-26284))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 5, "Cheers", new GiveItem(-26285))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 6, "Cheers", new GiveItem(-26287))
                .ConnectTo(graph, new RemoveMoneyAction(ColorBerryCost.Value));

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 7, "Cheers", new GiveItem(-26286))
                .ConnectTo(graph, new RemoveMoneyAction(1));


            #endregion



            ConditionNode BuyChoice = dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 4, "Aye, that I can do.", graph.AddNode<ConditionNode>());
            BuyChoice.SetCondition(new HasCurrency()
            {
                AmountRequired = BuyItemCost
            });



            BuyChoice.OnSuccess(graph, dialogueTreeBuilder.CreateNPCStatement("Thank you."))
            .ConnectTo(graph, new RemoveMoneyAction(BuyItemCost))
            .ConnectTo(graph, new GiveItem(BuyItemID))
            .ConnectTo(graph, new FinishNode());


            BuyChoice.OnFailure(graph, dialogueTreeBuilder.CreatePlayerStatement($"I don't seem to have enough silver, I'll return when I have {BuyItemCost} silver."))
            .ConnectTo(graph, new FinishNode());


            ConditionNode UniqueBuyChoice = dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 5, "Ofcourse, take a look.", graph.AddNode<ConditionNode>());

            UniqueBuyChoice.SetCondition(new HasCurrency(BuyItemCost));

            UniqueBuyChoice.OnSuccess(graph, dialogueTreeBuilder.CreateNPCStatement("Thank you."))
            .ConnectTo(graph, new RemoveMoneyAction(BuyItemCost))
            .ConnectTo(graph, new GiveItem(UniqueBuyItemID))
            .ConnectTo(graph, new FinishNode());

            UniqueBuyChoice.OnFailure(graph, dialogueTreeBuilder.CreatePlayerStatement($"I don't seem to have enough silver, I'll return when I have {BuyItemCost} silver."))
            .ConnectTo(graph, new FinishNode());
        }



        private void SpawnCharacterMountBuddy(Character character, string SpeciesName, bool SpawnInARow = false, int CurrentIndex = 0)
        {
            OnDestroyComp onDestroyComp = character.gameObject.AddComponent<OnDestroyComp>();
            MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);
            Vector3 FinalPosition = Vector3.zero;

            if (mountSpecies != null)
            {
                GameObject MountPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", mountSpecies.AssetBundleName, mountSpecies.PrefabName);

                if (MountPrefab)
                {
                    if(SpawnInARow)
                    {
                        FinalPosition = GetPositionInLineUp(character.transform.position + new Vector3(0,0, 2f), 2f, CurrentIndex);
                    }
                    else
                    {
                        FinalPosition = character.transform.position + (UnityEngine.Random.insideUnitSphere * 6f);
                    }

                    onDestroyComp.MountVisualInstance = GameObject.Instantiate(MountPrefab);
                    FinalPosition.y = character.transform.position.y;
                    onDestroyComp.MountVisualInstance.transform.position = FinalPosition;

                    EmoMountMod.Log.LogMessage($"Spawned {SpeciesName} for {character.Name}");

                    if (SpawnInARow)
                    {
                        onDestroyComp.MountVisualInstance.transform.forward = character.transform.forward;
                    }
                    else
                    {
                        onDestroyComp.MountVisualInstance.transform.LookAt(character.transform);
                    }                  
                }
            }
        }

        private Vector3 GetPositionInLineUp(Vector3 Origin, float Spacing, int CurrentIndex)
        {
            return Origin + new Vector3(0, 0, Spacing * CurrentIndex);
        }
    }
}
