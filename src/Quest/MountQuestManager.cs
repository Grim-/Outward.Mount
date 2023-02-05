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
using UnityEngine;

namespace EmoMount
{
    public class MountQuestManager
	{
		private const string EggQuestFamily = "MountEggSeries";

		private QuestEventSignature EggQuestPart_1Signature;
		private QuestEventSignature EggQuestPart_2Signature;

		private const string EggQuestEventOne = "mount.egg.partone";
		private const string EggQuestEventTwo = "mount.egg.parttwo";


		public Dictionary<int, int> MountToEggMap { get; private set; }


		public List<EggQuestMap> EggQuests { get; private set; }



		public MountQuestManager()
		{
			MountToEggMap = new Dictionary<int, int>();
			EggQuests = new List<EggQuestMap>();

			CreateQuestEvents();
			CreateEggQuestMap();

			SL.OnPacksLoaded += GenerateQuests;
		}

		private void CreateEggQuestMap()
        {

			EggQuestMap PearlBirdBase = new EggQuestMap(-26502, -26202, "PearlBird", "Wriggling Egg", Color.clear, false);
			EggQuests.Add(PearlBirdBase);
			EggQuestMap PearlBirdYellow = new EggQuestMap(-26503, -26203, "PearlBird", "Yellow Wriggling Egg", Color.yellow, false);
			EggQuests.Add(PearlBirdYellow);
			EggQuestMap PearlBirdBlue = new EggQuestMap(-26504, -26204, "PearlBird", "Blue Wriggling Egg", Color.cyan, false);
			EggQuests.Add(PearlBirdBlue);
			EggQuestMap PearlBirdRed = new EggQuestMap(-26505, -26205, "PearlBird", "Red Wriggling Egg", Color.red, false);
			EggQuests.Add(PearlBirdRed);
			EggQuestMap PearlBirdGreen = new EggQuestMap(-26506, -26206, "PearlBird", "Green Wriggling Egg", Color.green, false);
			EggQuests.Add(PearlBirdGreen);
			EggQuestMap PearlBirdBlack = new EggQuestMap(-26507, -26207, "PearlBird", "Black Wriggling Egg", Color.black, false);
			EggQuests.Add(PearlBirdBlack);
			EggQuestMap PearlBirdGolden = new EggQuestMap(-26508, -26208, "PearlBird", "Golden Wriggling Egg", Color.yellow, false);
			EggQuests.Add(PearlBirdGolden);


			EggQuestMap Manticore = new EggQuestMap(-26509, -26209, "Manticore", "Shaking Egg", Color.clear, false);
			EggQuests.Add(Manticore);
			EggQuestMap Tuanosaur = new EggQuestMap(-26510, -26210, "Tuanosaur", "Rattling Egg", Color.clear, false);
			EggQuests.Add(Tuanosaur);
		}

		private void CreateQuestEvents()
        {
			EggQuestPart_1Signature = CustomQuests.CreateQuestEvent(EggQuestEventOne, true, false, true, EggQuestFamily);
			EggQuestPart_2Signature = CustomQuests.CreateQuestEvent(EggQuestEventTwo, false, false, true, EggQuestFamily);
		}


		public EggQuestMap GetEggQuestMappingByEggID(int EggItemID)
		{
			foreach (var item in EggQuests)
			{
				if (item.EggItemID == EggItemID)
				{
					return item;
				}
			}

			return null;
		}
		public EggQuestMap GetEggQuestMappingByQuestID(int QuestID)
        {
            foreach (var item in EggQuests)
            {
                if (item.QuestID == QuestID)
                {
					return item;
                }
            }

			return null;
        }
		public bool HasQuestForItemID(int ItemID)
        {
			foreach (var item in EggQuests)
			{
				if (item.EggItemID == ItemID)
				{
					return true;
				}
			}

			return false;
		}

		public void GenerateQuests()
		{
            foreach (var item in EggQuests)
            {
				SL_Quest PearlBirdEgg = new SL_Quest()
				{
					Target_ItemID = 7011617,
					New_ItemID = item.QuestID,
					Name = item.QuestName,
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
				PearlBirdEgg.OnQuestLoaded += (Quest quest) =>
				{
					EggQuestMap cached = item;

					if (!QuestHasCustomGraph(quest))
					{
						GenerateEggHatchingQuestTree(quest, cached);
					}
				};
				PearlBirdEgg.ApplyTemplate();
			}
		}
		public static void StartQuestGraphForQuest(Quest Quest)
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


		private Graph GenerateEggHatchingQuestTree(Quest quest, EggQuestMap eggQuestMap, int QuestEventExpiryTime = 12, string QuestTreeName = "CustomGraph")
		{
			EmoMountMod.Log.LogMessage($"Generating QuestTree for {quest.DisplayName} ({quest.ItemID})");
			EmoMountMod.Log.LogMessage($"ItemIDToRemove {eggQuestMap.EggItemID}");
			EmoMountMod.Log.LogMessage($"Species to generate on complete {eggQuestMap.SpeciesID}");

			QuestGraphBuilder questGraphBuilder = new QuestGraphBuilder(quest);
			questGraphBuilder.SetGraphName(QuestTreeName);

            #region Step One

            QuestStep StepOne = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step One", null, ActionList.ActionsExecutionMode.ActionsRunInSequence);

			StepOne.AddLogEntryQuestAction("This egg feels warm and ocasionally wobbles.", false)
			.AddSendQuestEventAction(EggQuestPart_1Signature.EventUID);

            #endregion

            #region Step Two
            QuestStep StepTwo = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step Two", null, ActionList.ActionsExecutionMode.ActionsRunInSequence, false, FSMState.TransitionEvaluationMode.CheckAfterStateFinished);

			StepTwo.AddQuestAction(new QuestAction_WaitGameTime()
			{
				m_objective = new WaitGameTime()
				{
					QuestEventToCheck = new QuestEventReference()
					{
						m_eventUID = EggQuestPart_1Signature.EventUID
					},
					ExpiredTime = QuestEventExpiryTime
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
				DisplayTimeReceived = false
			})
			.AddQuestAction(new FadeOut()
			{

			})
			.AddQuestAction(new GenerateMountFromEgg()
			{
				SpeciesID = eggQuestMap.SpeciesID,
				GenerateRandomTint = eggQuestMap.GenerateRandomTint,
				OverrideColor = eggQuestMap.ForceTintColor
			})
			.AddQuestAction(new FadeIn()
			{

			});


            #endregion

            #region Step Three
            QuestStep StepThree = questGraphBuilder.CreateNewQuestStep("Mount Egg Quest Step Three", null, ActionList.ActionsExecutionMode.ActionsRunInSequence);

			StepThree.AddQuestAction(new ShowCharacterNotification()
			{
				NotificationText = "The egg hatched!"
			}).AddQuestAction(new RemoveItem()
			{
				Items = new List<BBParameter<ItemReference>>()
                {
					new BBParameter<ItemReference>()
                    {
						value = new ItemReference()
                        {
							ItemID = eggQuestMap.EggItemID,
							m_itemID = eggQuestMap.EggItemID,
						}
                    }
                },
				fromCharacter = new BBParameter<Character>()
                {
					value = quest.OwnerCharacter
                },
				Amount = new List<BBParameter<int>>()
                {
					new BBParameter<int>()
                    {
						value = 1
                    }
                }
			});


			StepThree.AddQuestAction(new RemoveQuestEvent()
			{
				QuestEventRef = new QuestEventReference()
				{
					EventUID = EggQuestPart_1Signature.EventUID
				}
			});

			StepThree.AddQuestAction(new CompleteQuest()
			{
				isSuccessful = true,
			}).AddQuestAction(new RemoveQuest()
            {
                questRef = new BBParameter<QuestReference>()
                {
                    value = new QuestReference()
                    {
                        m_itemID = quest.ItemID,
                    }
                }
            });

            #endregion

            questGraphBuilder.ConnectQuestSteps(StepOne, StepTwo, new Condition_CheckQuestEventExpiry()
			{
					QuestEventRef = new QuestEventReference()
					{
						EventUID = EggQuestEventOne
					}
			});

			questGraphBuilder.ConnectQuestSteps(StepTwo, StepThree, null);

			return questGraphBuilder.Graph;
		}

		public bool QuestHasCustomGraph(Quest Quest)
        {
			return Quest.GetQuestTree().name == "CustomGraph";
		}

		public bool CharacterHasQuest(Character Character, int QuestID)
		{
			return Character.Inventory.QuestKnowledge.IsItemLearned(QuestID);
		}

		public static Quest GenerateQuestItemForCharacter(Character character, int QuestItemID)
		{
			Quest quest = ItemManager.Instance.GenerateItemNetwork(QuestItemID) as Quest;
			quest.transform.SetParent(character.Inventory.QuestKnowledge.transform);
			character.Inventory.QuestKnowledge.AddItem(quest);
			return quest;
		}
	}
}
