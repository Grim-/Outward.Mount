﻿using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Conditions;
using System.Collections.Generic;
using UnityEngine;
using static EmoMount.EmoMountMod;

namespace EmoMount
{
    public class StableMaster : DialogueCharacter
    {
        public List<string> BuddySpecies = new List<string>
        {
            "PearlBird"
        };

        public string IntroStatement = "Welcome Traveler";


        public bool SellsWhistle = false;
        public string BuyText = "I can sell you a PearlBird Egg for 500 silver, keep it on you for 12 hours and it will hatch.";
        public int BuyItemID = -26202;
        public int BuyPrice = 500;

        public bool HasUniqueBuyable = false;
        public int UniqueBuyableID = -26203;
        public string UniqueBuyText = "I can sell you a PearlBird Egg for 500 silver, keep it on you for 12 hours and it will hatch.";
        public int UniqueBuyQuestID = -1;
        public int UniqueBuyPrice = 800;
        public string UniqueSellableHint = string.Empty;

        public void BuildDialogueForCharacter(DialogueTree graph, Character character)
        {
            DialogueTreeBuilder dialogueTreeBuilder = new DialogueTreeBuilder(graph);
            ((DialogueTreeExt)graph).SetCanBenInterrupted(false);

            //You must always set the intial statement
            StatementNodeExt InitialStatement = dialogueTreeBuilder.SetInitialStatement(IntroStatement);

            MultipleChoiceNodeExt initialChoice = null;

            #region Initial Choice


            if (HasUniqueBuyable)
            {
                initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
                {
                    "Can you look after my current mount?",
                    "I want to retrieve a mount.",
                    "Can you teach me a little about mounts?",
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
                });
            }
            else
            {
                initialChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
                {
                    "Can you look after my current mount?",
                    "I want to retrieve a mount.",
                    "Can you teach me a little about mounts?",
                    "I want to buy some color berries",
                    "Do you have any creatures for sale?",
                },
                new ConditionTask[]
                    {
                    new CharacterHasActiveMountConditionNode(),
                    new CharacterHasNOActiveMountConditionNode(),
                    null,
                    null,
                    null,
                });
            }






            //you must connect the inital statement and inital node
            graph.ConnectNodes(InitialStatement, initialChoice);
            #endregion


            string MountSimpleExplanation = $"You can buy, find and I have even heard in rare cases <i> create </i> - a mount." +
                $"There are usually two ways to get a new mount, find a whistle and use it or find an egg use it and 12 hours later it will hatch. " +
                $"Most of my colleagues in the Stablery business sell some kind of mount to get you moving. Ask them!";

            string MountSimpleExplanation2 = $"<b>Most</b> mounts will require feeding! " +
                $"They all have a favourite food in the case of carnivores it is usually <b>Jewel Bird Meat</b> and <b>Herbivores seem to love Marsh Mellons</b> favourite foods fill a mount up better than other foods" +
                $"Most mounts will have a maximum carry weight which they will move slower than usual the closer they are to reaching it.";

            string MountSimpleExplanation3 = $"Then there are eggs you can find Pearl Bird nests out in the world sometimes these may contain eggs that you are able to raise into mounts sometimes the plumage on the Pearl birds varies!" +
                $"I hear you can also find eggs for Manticores and Tuanosaurs sometimes from their corpses! I hear even Alphas!";

            string MountSimpleExplanation4 = $"And there are also disturbing rumours about a <b>Gold Lich</b> experimenting with living pearl bird egg using <b>Living Gold</b>, can you imagine? How does such a creature even exist?" +
                $"Reminds me of that <b>Mad Pirate Cpt</b> famed for his unusual Pearl Bird companion he had some how transmuted <b>Living Obsidian</b> and a Pearl bird egg to create an incredibly fast mount that did not require feeding but is such an advantage worth the cost?";

            string MountSimpleExplanation5 = $"Anyway .. Here are two skills you can use to Summon and Dismiss your <b>Active</b> mount - you will still have to visit a stable master in order to change it though, we don't charge for this service.";

            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 0, "Aye, I will take care of them.", new DismissMountActionNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 1, "Here's a list of what you have in my stables.", new DisplayMountStorageListNode());
            dialogueTreeBuilder.AddAnswerToMultipleChoice(initialChoice, 2, MountSimpleExplanation, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation2))
                .ConnectTo(graph, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation3))
                .ConnectTo(graph, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation4))
                .ConnectTo(graph, dialogueTreeBuilder.CreateNPCStatement(MountSimpleExplanation5))
                .ConnectTo(graph, new LearnMountSkillsNode());

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


            MultipleChoiceNodeExt BuyMultiChoice = dialogueTreeBuilder.AddMultipleChoiceNode(new string[]
            {
                $"Sure, I'll pay {BuyPrice}.",
                "No thanks, not right now"
            },
            new ConditionTask[]
            {
                new HasCurrency()
                {
                    AmountRequired = BuyPrice
                },
                null
            });

            dialogueTreeBuilder.AddAnswerToMultipleChoice<MultipleChoiceNodeExt>(initialChoice, 4, BuyText, BuyMultiChoice);
            dialogueTreeBuilder.AddAnswerToMultipleChoice(BuyMultiChoice, 0, dialogueTreeBuilder.CreateNPCStatement("Thanks, take care of it.")).ConnectTo(graph, new RemoveMoneyAction(BuyPrice)).ConnectTo(graph, new GiveItem(BuyItemID));
            dialogueTreeBuilder.AddAnswerToMultipleChoice(BuyMultiChoice, 1, dialogueTreeBuilder.CreateNPCStatement("No problem, take care.")).ConnectTo(graph, graph.AddNode<FinishNode>());


            if (HasUniqueBuyable)
            {
                // Start -> Do you have any unique for sale -> Condition (Done Quest) => True -> Multichoice yes buy / no dont
                //                                                                    => False -> ShowHint
                ConditionNode CanBuyUnique_Condition = dialogueTreeBuilder.AddAnswerToMultipleChoice<ConditionNode>(initialChoice, 5, graph.AddNode<ConditionNode>());
                CanBuyUnique_Condition.SetCondition(new HasCompleteQuest()
                {
                    QuestID = UniqueBuyQuestID
                });




                MultipleChoiceNodeExt UniqueBuyMultiChoice = dialogueTreeBuilder.AddMultipleChoiceNode(
                new string[]
                {
                $"Sure, I'll pay {BuyPrice}.",
                "No thanks, not right now."
                },
                new ConditionTask[]
                {
                    new HasCurrency()
                    {
                        AmountRequired = UniqueBuyPrice
                    },
                    null
                    }
                );
                CanBuyUnique_Condition.OnSuccess(graph, dialogueTreeBuilder.CreateNPCStatement(UniqueBuyText)).ConnectTo(graph, UniqueBuyMultiChoice);
                CanBuyUnique_Condition.OnFailure(graph, dialogueTreeBuilder.CreateNPCStatement(UniqueSellableHint));

                dialogueTreeBuilder.AddAnswerToMultipleChoice(UniqueBuyMultiChoice, 0, dialogueTreeBuilder.CreateNPCStatement("Thanks.")).ConnectTo(graph, new RemoveMoneyAction(UniqueBuyPrice)).ConnectTo(graph, new GiveItem(UniqueBuyableID));
                dialogueTreeBuilder.AddAnswerToMultipleChoice(UniqueBuyMultiChoice, 1, dialogueTreeBuilder.CreateNPCStatement("No problem, take care.")).ConnectTo(graph, graph.AddNode<FinishNode>());
            }
        }

        public void SpawnCharacterMountBuddy(Character character, string SpeciesName, int CurrentIndex = 0, bool RandomizeColor = false)
        {
            OnDestroyComp onDestroyComp = character.gameObject.AddComponent<OnDestroyComp>();
            MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);
            Vector3 FinalPosition = Vector3.zero;

            if (mountSpecies != null)
            {
                GameObject MountPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", mountSpecies.AssetBundleName, mountSpecies.PrefabName);

                if (MountPrefab)
                {             
                    FinalPosition = GetPositionInLineUp(character.transform.position + new Vector3(0, 0, 2f), 2f, CurrentIndex);                    
                    onDestroyComp.MountVisualInstance = GameObject.Instantiate(MountPrefab);
                    FinalPosition.y = character.transform.position.y;
                    onDestroyComp.MountVisualInstance.transform.position = FinalPosition;           
                    onDestroyComp.MountVisualInstance.transform.forward = character.transform.forward;

                    if (RandomizeColor)
                    {
                        MountPrefab.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", mountSpecies.GetRandomColor());
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
