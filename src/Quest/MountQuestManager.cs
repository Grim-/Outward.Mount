using NodeCanvas.DialogueTrees;
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


        public Dictionary<int, int> MountToEggMap { get; private set; }


        public MountQuestManager()
        {
            MountToEggMap = new Dictionary<int, int>();
            EggQuestPart_1Signature = CustomQuests.CreateQuestEvent(EggQuestEventOne, true, false, true, EggQuestFamily);
            EggQuestPart_2Signature = CustomQuests.CreateQuestEvent(EggQuestEventTwo, false, false, true, EggQuestFamily);

            SL.OnPacksLoaded += GenerateQuests;
            SL.OnGameplayResumedAfterLoading += SL_OnGameplayResumedAfterLoading;


            //Pearl bird to Pearl Bird Egg White
            MountToEggMap.Add(-26300, -26202);
        }

        public int GetEggItemIDFromMountItemID(int MountWhistleID)
        {
            if (MountToEggMap.ContainsKey(MountWhistleID))
            {
                return MountToEggMap[MountWhistleID];
            }

            return -1;
        }

        public int GetMountItemIDFromEggItemID(int EggItemID)
        {
            foreach (var item in MountToEggMap)
            {
                if (item.Value == EggItemID)
                {
                    return item.Key;
                }
            }
            return -1;
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
            //Character host = CharacterManager.Instance.GetFirstLocalCharacter();
            ////host.CharacterUI.ShowInfoNotification("Quest Added");

            //if (!CharacterHasQuest(host, -26501))
            //{
            //    GenerateQuestItemForCharacter(host, -26501);
            //}

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


           //QuestLogEntrySignature LogOneEntry = questGraphBuilder.AddNewDynamicLogEntry("egg.foundtime.log", true, "This egg feels warm and ocasionally wobbles");
           //QuestLogEntrySignature LogTwoEntry = questGraphBuilder.AddNewDynamicLogEntry("egg.hatchtime.log", true, "The egg hatched!");

            QuestStep StepOne = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step One", null, ActionList.ActionsExecutionMode.ActionsRunInSequence);

            StepOne.AddLogEntryQuestAction("This egg feels warm and ocasionally wobbles.", true)
            .AddSendQuestEventAction(EggQuestPart_1Signature.EventUID);


            QuestStep StepTwo = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step Two", null, ActionList.ActionsExecutionMode.ActionsRunInSequence, false, FSMState.TransitionEvaluationMode.CheckAfterStateFinished);

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
                statement = new Statement()
                {
                    _text = "The egg hatched!"
                },
                AssociatedLogType = QuestAction_AddLogEntry.LogType.SimpleText,
                DisplayTimeReceived = true
            })
            .AddQuestAction(new FadeOut()
            {

            })
            .AddQuestAction(new GenerateMountFromEgg()
            {
                SpeciesID = "PearlBird"
            })
            .AddQuestAction(new FadeIn()
            {

            });

            QuestStep StepThree = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step Three", null, ActionList.ActionsExecutionMode.ActionsRunInSequence);

            StepThree.AddQuestAction(new ShowCharacterNotification()
            {
                NotificationText = "The egg hatched!"
            });

            StepThree.AddQuestAction(new CompleteQuest()
            {
                isSuccessful = true,
            });

            //.AddQuestAction(new RemoveQuest()
            //{
            //    questRef = new BBParameter<QuestReference>()
            //    {
            //        value = new QuestReference()
            //        {
            //            m_itemID = -26501,
            //        }
            //    }
            //});


          questGraphBuilder.ConnectQuestSteps(StepOne, StepTwo, new Condition_CheckQuestEventExpiry()
          {
                QuestEventRef = new QuestEventReference()
                {
                    EventUID = EggQuestEventOne
                }
          });

            //questGraphBuilder.ConnectQuestSteps(StepTwo, StepThree, null);

            questGraphBuilder.ConnectQuestSteps(StepTwo, StepThree, new Condition_QuestEventOccured()
            {
                 m_eventUID = EggQuestEventOne,
                QuestEventRef = new QuestEventReference()
                {
                    EventUID = EggQuestEventOne
                }
            });

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
