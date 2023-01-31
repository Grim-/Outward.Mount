using EmoMount;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using NodeCanvas.Tasks.Actions;
using System.Collections.Generic;

public class QuestGraphBuilder
{
    //Quest GameObject
    private Quest Quest;
    public QuestProgress QuestProgress { get; private set; }

    public QuestTreeOwner QuestTreeOwner { get; private set; }
    public QuestTree Graph { get; private set; }


    private Dictionary<string, ConcurrentState> ConcurrentStates = new Dictionary<string, ConcurrentState>();
    private Dictionary<string, AnyState> AnyStates = new Dictionary<string, AnyState>();

    public QuestGraphBuilder(Quest quest)
    {
        Quest = quest;
        QuestProgress = Quest.m_questProgress;
        QuestTreeOwner = QuestProgress.m_questSequence;
        Graph = (QuestTree)QuestTreeOwner.graph;
        Graph.ClearGraph();
    }


    public void SetGraphName(string name)
    {
        Graph.name = name;
    }

    public QuestStep CreateNewQuestStep(string QuestStepName, ActionTask FirstNode = null, ActionList.ActionsExecutionMode ExecutionMode = ActionList.ActionsExecutionMode.ActionsRunInSequence)
    {
        QuestStep questStep = new QuestStep();
        questStep.name = QuestStepName;
        questStep._graph = Graph;

        if (questStep._actionList == null) questStep._actionList = new ActionList();     
        questStep._actionList.executionMode = ExecutionMode;
        if (FirstNode != null) questStep.actionList.AddAction(FirstNode);
        if (!GraphContainsNode(questStep)) Graph.allNodes.Add(questStep);
        return questStep;
    }


    public T AddConcurrentState<T>(string StateName) where T : ConcurrentState
    {
        T concurrentState = new ConcurrentState() as T;
        concurrentState._name = StateName;

        if (!ConcurrentStates.ContainsKey(StateName))
        {
            Graph.concurentStates.Add(concurrentState);

        }
        else
        {
            ConcurrentStates[StateName] = concurrentState;
        }

        return concurrentState;
    }
    public T AdAnyState<T>(string StateName) where T : AnyState
    {
        T anystate = new AnyState() as T;
        anystate._name = StateName;

        if (!AnyStates.ContainsKey(StateName))
        {
            Graph.anyStates.Add(anystate);

        }
        else
        {
            AnyStates[StateName] = anystate;
        }

        return anystate;
    }


    public bool GraphContainsNode(Node Node)
    {
        foreach (var item in Graph.allNodes)
        {
            if (item == Node)
            {
                return true;
            }
        }
        return false;
    }
    public FSMConnection ConnectQuestSteps(QuestStep QuestAction,  QuestStep TargetAction, ConditionTask ConnectionCondition = null)
    {
        FSMConnection connection = (FSMConnection)Graph.ConnectNodes(QuestAction, TargetAction);
        if (ConnectionCondition != null)
        {
            connection.condition = ConnectionCondition;
        }

        return connection;
    }

    public void StartQuestGraph()
    {
        QuestTreeOwner QTO = GetQuestTreeOwner();
        if (Graph != null)
        {
            EmoMountMod.Log.LogMessage($"Starting QuestGraph {Graph}");
            if (Graph.primeNode == null)
            {
                Graph.primeNode = Graph.allNodes[0];
            }

            QTO.StartBehaviour();
        }
    }


    public QuestAction_AddLogEntry AddLogEntry(QuestStep QuestStep, string LogEntryText, bool DisplayTimeRecieved = false)
    {
        QuestAction_AddLogEntry LogEntryAction = new QuestAction_AddLogEntry()
        {
            statement = new NodeCanvas.DialogueTrees.Statement()
            {
                _text = LogEntryText
            },
            AssociatedLogType = QuestAction_AddLogEntry.LogType.SimpleText,
            DisplayTimeReceived = DisplayTimeRecieved
        };

        QuestStep.AddQuestAction(LogEntryAction);
        return LogEntryAction;
    }
    public SendQuestEvent AddSendQuestEvent(QuestStep QuestStep, string EventUID)
    {
        SendQuestEvent sendQuestEvent = new SendQuestEvent()
        {
            QuestEventRef = new QuestEventReference()
            {
                EventUID = EventUID,
            },
        };


        QuestStep.AddQuestAction(sendQuestEvent);
        return sendQuestEvent;
    }
}