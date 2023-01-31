using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using NodeCanvas.Tasks.Actions;
using NodeCanvas.Tasks.Conditions;
using QuestSystem.NodeObjectives;
using QuestSystem.QuestObjectives;
using SideLoader;
using SideLoader.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount
{    
    public class MountQuestManager
    {
        private const string EggQuestFamily = "MountEggSeries";
        private Quest EggQuest;

        private QuestEventSignature EggQuestPart_1Signature;
        private QuestEventSignature EggQuestPart_2Signature;

        private const string EggQuestEventOne = "mount.egg.partone";
        private const string EggQuestEventTwo = "mount.egg.parttwo";

        public MountQuestManager()
        {
            EggQuestPart_1Signature = CustomQuests.CreateQuestEvent(EggQuestEventOne, true, false, true, EggQuestFamily);
            EggQuestPart_2Signature = CustomQuests.CreateQuestEvent(EggQuestEventTwo, false, false, true, EggQuestFamily);


            SL.OnPacksLoaded += GenerateQuests;
            SL.OnGameplayResumedAfterLoading += SL_OnGameplayResumedAfterLoading;
        }

        public void GenerateQuests()
        {
            SL_Quest EggTestQuest = new SL_Quest()
            {
                Target_ItemID = 7011617,
                New_ItemID = -26501,
                Name = "Wriggling PearlBird Egg",
                ExtensionsEditBehaviour = EditBehaviours.Destroy,
                IsSideQuest = true,                
                ItemExtensions = new SL_ItemExtension[]
                {
                    new SL_QuestProgress()
                    {
                        Savable = true,
                    }
                },
            };
            EggTestQuest.OnQuestLoaded += GeneratePealBirdQuest;
            EggTestQuest.ApplyTemplate();
        }


        private void GeneratePealBirdQuest(Quest quest)
        {
            EggQuest = quest;

            if(EggQuest.GetQuestTree().name != "CustomGraph")
            {
                EmoMountMod.Log.LogMessage($"Quest has no CustomGraph");
                GenerateQuestTree(EggQuest);
            }
            else
            {
                EmoMountMod.Log.LogMessage($"Quest has a CustomGraph");
            }

            StartQuestGraphForQuest(EggQuest);
        }

        private void SL_OnGameplayResumedAfterLoading()
        {
            Character host = CharacterManager.Instance.GetFirstLocalCharacter();
            //host.CharacterUI.ShowInfoNotification("Quest Added");

            if (!CharacterHasQuest(host, -26501))
            {
                GenerateQuestItemForCharacter(host, -26501);
            }

        }


        private void StartQuestGraphForQuest(Quest Quest)
        {
            QuestTreeOwner QTO = Quest.GetQuestTreeOwner();

            QuestTree Graph = (QuestTree)QTO.graph;
            if (Graph != null)
            {
                EmoMountMod.Log.LogMessage($"Starting Graph {Graph} Graph Is Currently Running ? : {Graph.isRunning}");
                if (Graph.primeNode == null)
                {
                    Graph.primeNode = Graph.allNodes[0];
                }

                QTO.StartBehaviour();
            }
        }

        private Graph GenerateQuestTree(Quest quest, string QuestTreeName = "CustomGraph")
        {
            EmoMountMod.Log.LogMessage($"Generating QuestTree for {quest.DisplayName}");

            QuestGraphBuilder questGraphBuilder = new QuestGraphBuilder(quest);
            questGraphBuilder.SetGraphName(QuestTreeName);


            QuestStep StepOne = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step One", null, ActionList.ActionsExecutionMode.ActionsRunInParallel);

            StepOne.AddLogEntryQuestAction("This egg feels warm and ocasionally wobbles.", true)
            .AddSendQuestEventAction(EggQuestPart_1Signature.EventUID);


            QuestStep StepTwo = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step Two");

            StepTwo.AddQuestAction(new QuestAction_WaitGameTime()
            {
                  m_objective = new WaitGameTime()
                  {
                      QuestEventToCheck = new QuestEventReference()
                      {
                          m_eventUID = EggQuestPart_1Signature.EventUID
                      },
                      ExpiredTime = 10
                  },
            })
            .AddQuestAction(new QuestAction_AddLogEntry()
            {
                m_logSignatureUID = "mount.egghatched",
                statement = new NodeCanvas.DialogueTrees.Statement()
                {
                    _text = "The egg hatched!"
                },
                AssociatedLogType = QuestAction_AddLogEntry.LogType.SimpleText,
                DisplayTimeReceived = true
            })
            .AddQuestAction(new GenerateMountFromEgg()
            {
                SpeciesID = "PearlBird"
            })
            .AddQuestAction(new CompleteQuest()
            {
                isSuccessful = true,
            });

            questGraphBuilder.ConnectQuestSteps(StepOne, StepTwo, new Condition_CheckQuestEventExpiry()
            {
                QuestEventRef = new QuestEventReference()
                {
                    EventUID = EggQuestEventOne
                }
            });


            //questGraphBuilder.ConnectQuestSteps(StepTwo, StepThree);

            return questGraphBuilder.Graph;
        }

        public bool CharacterHasQuest(Character Character, int QuestID)
        {
            return Character.Inventory.QuestKnowledge.IsItemLearned(QuestID);
        }
        private Quest GenerateQuestItemForCharacter(Character character, int QuestItemID)
        {
            Quest quest = ItemManager.Instance.GenerateItemNetwork(QuestItemID) as Quest;
            quest.transform.SetParent(character.Inventory.QuestKnowledge.transform);
            character.Inventory.QuestKnowledge.AddItem(quest);
            return quest;
        }
    }
}
