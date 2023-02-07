using EmoMount;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
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
        public DialogueCharacter[] StableMasters = new DialogueCharacter[]
        {
            new()
            {
                UID = "emomount.mountcharactercaldera",
                Name = "Libre, Harmattan Stable Master",
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
            },
            new()
            {
                UID = "emomount.mountcharacterharmattan",
                Name = "Libre, Harmattan Stable Master",
                SpawnSceneBuildName = "Levant",
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
            },
            new()
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
            new()
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
            new()
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
            },
            new()
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
            }
        };

   
        public MountNPCManager()
        {
            SetupNPCs();
        }

        private void SetupNPCs()
        {
            SetupLevantNPC();
            SetupBergNPC();
            SetupCierzoNPC();
            SetupMonsoonNPC();
            SetupHarMattanNPC();
            SetupCalederaNPC();
        }

        private void SetupNPC(DialogueCharacter DialogueCharacter, string BuddySpecies = "", string SellText = "", bool SellsAWhistle = true, int SellItemID = -1, int SellAmount = 0)
        {
            // Create and apply the template
            var template = DialogueCharacter.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[] { "GoldenPearlBird" }, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            // Add a listener to set up our dialogue
            DialogueCharacter.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            {
                BuildDialouge(graph, character, SellText, SellItemID, SellAmount);
            };

            // Add this func to determine if our character should actually spawn
            template.ShouldSpawn = () => true;
        }


        private void SetupCalederaNPC()
        {
            ///levant
            DialogueCharacter levantGuard = new()
            {
                UID = "emomount.mountcharactercaldera",
                Name = "Libre, Harmattan Stable Master",
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
            };


            // Create and apply the template
            var template = levantGuard.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[] { "GoldenPearlBird" }, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            // Add a listener to set up our dialogue
            levantGuard.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            {
                BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            };

            // Add this func to determine if our character should actually spawn
            template.ShouldSpawn = () => true;
        }
        private void SetupHarMattanNPC()
        {
            ///levant
            DialogueCharacter levantGuard = new()
            {
                UID = "emomount.mountcharacterharmattan",
                Name = "Libre, Harmattan Stable Master",
                SpawnSceneBuildName = "Levant",
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
            };


            // Create and apply the template
            var template = levantGuard.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[] { "GoldenPearlBird" }, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            // Add a listener to set up our dialogue
            levantGuard.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            {
                BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            };
            // Add this func to determine if our character should actually spawn
            template.ShouldSpawn = () => true;
        }
        private void SetupLevantNPC()
        {
            ///levant
            DialogueCharacter levantGuard = new()
            {
                UID = "emomount.mountcharacterlevant",
                Name = "Ianis, Levant Stable Master",
                SpawnSceneBuildName = "Harmattan",
                SpawnPosition = new(-39.7222f, 0.2239f, 120.0354f),
                SpawnRotation = new(0, 218f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
            };


            // Create and apply the template
            var template = levantGuard.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[] { "GoldenPearlBird" }, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            // Add a listener to set up our dialogue
            levantGuard.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            {
                BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            };
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
            var bergTemplate = BergNPC.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[] { "GoldenPearlBird" }, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            //// Add a listener to set up our dialogue
            //BergNPC.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            //{
            //    BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            //};
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
            var cierzotemplate = CierzoNPC.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Vector3 MountPosition = Character.transform.position + (Vector3)UnityEngine.Random.insideUnitSphere * 0.8f;
                Template_OnSpawn(SLTemplate, Character, RPCData, EmoMountMod.MountManager.SpeciesData.Values.Select(ms => ms.SpeciesName).ToArray(), "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300, true);
            });

            // Add a listener to set up our dialogue
            CierzoNPC.OnSetupDialogueGraph += (DialogueTree graph, Character character)=>
            {
                BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            };

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
            var monsoontemplate = MonsoonNPC.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                Template_OnSpawn(SLTemplate, Character, RPCData, new string[]{ "GoldenPearlBird"}, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            });

            // Add a listener to set up our dialogue
            MonsoonNPC.OnSetupDialogueGraph += (DialogueTree graph, Character character) =>
            {
                BuildDialouge(graph, character, "I can sell you a PearlBird Egg for 300 silver, keep it on you for 12 hours and it will hatch.", -26202, 300);
            };
            // cierzotemplate this func to determine if our character should actually spawn
            monsoontemplate.ShouldSpawn = () => true;
        }

        private void BuildDialouge(DialogueTree graph, Character character, string BuyText, int BuyItemID, int BuyItemCost)
        {
            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(graph);

            //You must always set the intial statement
            StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement("Welcome traveler.");


            //Now you can add the next node
            MultipleChoiceNodeExt initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "Can you look after my current mount?",
                "I want to retrieve a mount.",
                "Can you teach me some mount skills?",
                "I want to buy some color berries",
                "Do you have any creatures for sale?"
            },
            new ConditionTask[]
            {
                new CharacterHasActiveMountConditionNode(),
                new CharacterHasNOActiveMountConditionNode(),
                null,
                new CharacterHasActiveMountConditionNode(),
                new HasCurrency()
                {
                    AmountRequired = BuyItemCost
                }
             });

            //you must connect the inital statement and inital node
            graph.ConnectNodes(InitialStatement, initialChoice);

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, "Aye, that I can do.", new DismissMountActionNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Here's a list of what you have in my stables.", new DisplayMountStorageListNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 2, "Heres watcha do...", new LearnMountSkillsNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 3, BuyText, new GiveItem(BuyItemID));

            MultipleChoiceNodeExt ColorBerryChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                "Red?",
                "Green?",
                "Blue?",
                "Cyan?",
                "Purple?",
                "Yellow?",
                "White?",
                "Black?",
                "Reset? (removes current tint if any)"
            });


            //Last option of the first multi-choice takes you to the second multi choice
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 3, "Heres watcha do...", ColorBerryChoice);

            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 0, "Yup", new GiveItem(-26280)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 1, "Yup", new GiveItem(-26281)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 2, "Yup", new GiveItem(-26282)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 3, "Yup", new GiveItem(-26283)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 4, "Yup", new GiveItem(-26284)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 5, "Yup", new GiveItem(-26285)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 6, "Yup", new GiveItem(-26286)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 7, "Yup", new GiveItem(-26287)).ConnectTo(graph, new FinishNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(ColorBerryChoice, 8, "Yup", new GiveItem(-26288)).ConnectTo(graph, new FinishNode());
        }


        private void Template_OnSpawn(SL_Character SLTemplate, Character character, string rpcData, string[] SpeciesName, string BuyText, int BuyItemID, int BuyItemCost, bool SpawnInARow = false)
        {

            GameObject dialogueTemplate = GameObject.Instantiate(Resources.Load<GameObject>("editor/templates/DialogueTemplate"));
            dialogueTemplate.transform.parent = character.transform;
            dialogueTemplate.transform.position = character.transform.position;

            // set Dialogue Actor name
            DialogueActor ourActor = character.GetComponentInChildren<DialogueActor>();
            ourActor.SetName(SLTemplate.Name);

            // setup dialogue tree
            DialogueTreeController graphController = character.GetComponentInChildren<DialogueTreeController>();
            Graph graph = graphController.graph;

            // the template comes with an empty ActorParameter, we can use that for our NPC actor.
            List<DialogueTree.ActorParameter> actors = (graph as DialogueTree)._actorParameters;
            actors[0].actor = ourActor;
            actors[0].name = ourActor.name;


            BuildDialouge((DialogueTree)graph, character, BuyText, BuyItemID, BuyItemCost);


            if (SpeciesName != null && SpeciesName.Length > 0)
            {
                for (int i = 0; i < SpeciesName.Length; i++)
                {
                    if (!string.IsNullOrEmpty(SpeciesName[i]))
                    {
                        if (SpawnInARow)
                        {
                            SpawnCharacterMountBuddy(character, SpeciesName[i], true, i);
                        }
                        else
                        {
                            SpawnCharacterMountBuddy(character, SpeciesName[i]);
                        }

                    }
                }
            }

 
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
                        FinalPosition = character.transform.position + new Vector3(0, 0, (1.5f * CurrentIndex));
                    }
                    else
                    {
                        FinalPosition = character.transform.position + (UnityEngine.Random.insideUnitSphere * 4f);
                    }

                    onDestroyComp.MountVisualInstance = GameObject.Instantiate(MountPrefab);
                    FinalPosition.y = character.transform.position.y;
                    onDestroyComp.MountVisualInstance.transform.position = FinalPosition;

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


        //private void Old_BuildDialouge(DialogueTree graph, Character character)
        //{
        //    var ourActor = graph.actorParameters[0];
        //    // Add our root statement
        //    var InitialStatement = graph.AddNode<StatementNodeExt>();
        //    InitialStatement.statement = new($"Welcome, can I store a mount for you in our stables?");
        //    InitialStatement.SetActorName(ourActor.name);

        //    // Add a multiple choice
        //    var multiChoice1 = graph.AddNode<MultipleChoiceNodeExt>();

        //    MultipleChoiceNodeExt.Choice DismissChoice = new MultipleChoiceNodeExt.Choice()
        //    {
        //        statement = new Statement("Can you look after my current mount?")
        //    };

        //    MultipleChoiceNodeExt.Choice SummonChoice = new MultipleChoiceNodeExt.Choice()
        //    {
        //        statement = new Statement("I want to retrieve a mount.")
        //    };

        //    MultipleChoiceNodeExt.Choice LearnSkillsChoice = new MultipleChoiceNodeExt.Choice()
        //    {
        //        statement = new Statement("Can you teach me some mount skills?")
        //    };

        //    multiChoice1.availableChoices.Add(DismissChoice);
        //    multiChoice1.availableChoices.Add(SummonChoice);
        //    multiChoice1.availableChoices.Add(LearnSkillsChoice);


        //    //var multiChoice2 = graph.AddNode<MultipleChoiceNodeExt>();

        //    //MultipleChoiceNodeExt.Choice choice1 = new MultipleChoiceNodeExt.Choice()
        //    //{
        //    //    statement = new Statement("Multiple Choice 1")
        //    //};

        //    //var answertochoiceOne = graph.AddNode<StatementNodeExt>();
        //    //answertochoiceOne.statement = new("Reply to choice 1");
        //    //answertochoiceOne.SetActorName(ourActor.name);

        //    //multiChoice2.availableChoices.Add(choice1);
        //    //graph.allNodes.Add(multiChoice2);
        //    //graph.allNodes.Add(answertochoiceOne);

        //    //graph.ConnectNodes(multiChoice1, multiChoice2, 0);
        //    //graph.ConnectNodes(multiChoice2, answertochoiceOne, 0);

        //    // Add our answers
        //    var answer1 = graph.AddNode<StatementNodeExt>();
        //    answer1.statement = new("Aye, that I can do.");
        //    answer1.SetActorName(ourActor.name);

        //    var answer2 = graph.AddNode<StatementNodeExt>();
        //    answer2.statement = new("Here's a list of what you have in my stables.");
        //    answer2.SetActorName(ourActor.name);

        //    var answer3 = graph.AddNode<StatementNodeExt>();
        //    answer3.statement = new("Heres watcha do...");
        //    answer3.SetActorName(ourActor.name);

        //    DismissMountActionNode dismissMountActionNode = new DismissMountActionNode();
        //    DisplayMountStorageListNode displayStorageListNode = new DisplayMountStorageListNode();
        //    LearnMountSkillsNode learnMountSkillsNode = new LearnMountSkillsNode();

        //    // ===== finalize nodes =====
        //    graph.allNodes.Clear();


        //    // add ALLL the nodes we want to use, remember this is a literal graph, the nodes must be on the graph to draw connections between them
        //    graph.allNodes.Add(InitialStatement);
        //    graph.primeNode = InitialStatement;
        //    graph.allNodes.Add(multiChoice1);
        //    graph.allNodes.Add(answer1);
        //    graph.allNodes.Add(answer2);
        //    graph.allNodes.Add(answer3);
        //    graph.allNodes.Add(dismissMountActionNode);
        //    graph.allNodes.Add(displayStorageListNode);
        //    graph.allNodes.Add(learnMountSkillsNode);



        //    // setup our connections
        //    graph.ConnectNodes(InitialStatement, multiChoice1);    // Connect Initial node to MultiChoice node


        //    graph.ConnectNodes(multiChoice1, answer1, 0);        //connect MultiChoice port 0 to answer 1
        //    graph.ConnectNodes(answer1, dismissMountActionNode); //connect answer 1 to dismiss mount node
        //    graph.ConnectNodes(answer1, InitialStatement);         // go back to start

        //    graph.ConnectNodes(multiChoice1, answer2, 1);      ///repeat for answer 2 and its actions
        //    graph.ConnectNodes(answer2, displayStorageListNode);
        //    graph.ConnectNodes(answer2, InitialStatement);

        //    graph.ConnectNodes(multiChoice1, answer3, 2);
        //    graph.ConnectNodes(answer3, learnMountSkillsNode);
        //    graph.ConnectNodes(answer3, InitialStatement);

        //}
    }
}
