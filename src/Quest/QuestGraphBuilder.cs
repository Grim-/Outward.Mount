using EmoMount;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using NodeCanvas.Tasks.Actions;
using System.Collections.Generic;

public class QuestGraphBuilder
{
    ///A QuestStep extends from ActionState which extends from FSMState
    //These are all valid node types, adding them to the Graph.allNodes array is enough for the graph component to find and correctly categorize a state

    ///ActionState (QuestStep)
    ///Execute a number of Action Tasks OnEnter. All actions will be stoped OnExit. This state is Finished when all Actions are finished as well

    //Concurrent State
    //Execute a number of Actions with optional conditional requirement and in parallel to any other state, as soon as the FSM is started.
    //All actions will prematurely be stoped as soon as the FSM stops as well.This is not a state per-se and thus can have neither incomming, nor outgoing transitions.

    //AnyState
    //The Transitions of this node will constantly be checked. If any becomes true, the target connected State will Enter regardless of the current State. This node can have no incomming transitions

    //NestedBTState
    //Execute a Behaviour Tree OnEnter. OnExit that Behavior Tree will be stoped or paused based on the relevant specified setting.
    //You can optionaly specify a Success Event and a Failure Event which will be sent when the BT's root node status returns either of the two. If so, use alongside with a CheckEvent on a transition.

    //NestedFSMState
    //Execute a nested FSM OnEnter and Stop that FSM OnExit. This state is Finished when the nested FSM is finished as well"

    //SuperActionState
    //The Super Action State provides finer control on when to execute actions.
    //This state is never Finished by it's own if there is any Actions in the OnUpdate list and thus OnFinish transitions will never be called in that case. OnExit Actions are only called for 1 frame when the state exits.

    //Quest GameObject
    private Quest Quest;
    public QuestProgress QuestProgress { get; private set; }

    public QuestTreeOwner QuestTreeOwner { get; private set; }
    public QuestTree Graph { get; private set; }

    private Dictionary<string, FSMState> CustomStates = new Dictionary<string, FSMState>();

    public QuestGraphBuilder(Quest quest)
    {
        Quest = quest;
        QuestProgress = Quest.m_questProgress;
        QuestTreeOwner = QuestProgress.m_questSequence;
        Graph = (QuestTree)QuestTreeOwner.graph;
        Graph.ClearGraph();
        QuestProgress.m_progressState = QuestProgress.ProgressState.InProgress;
    }


    public void SetGraphName(string name)
    {
        Graph.name = name;
    }


    public QuestLogEntrySignature AddNewDynamicLogEntry(string LogUID, bool DisplayTimeOnLog, string LogText, string LocalizatonKey = "")
    {
        QuestLogEntrySignature questLogEntrySignature = new QuestLogEntrySignature()
        {
            DefaultText = LogText,
            LogType = QuestLogEntrySignature.Type.Dynamic,
            m_UID = LogUID,
            LogEntryLocKey = LocalizatonKey
        };

        if (QuestProgress)
        {
            QuestProgress.m_logSignatures.Add(questLogEntrySignature);

            //QuestProgress.UpdateLogEntry(LogUID, DisplayTimeOnLog, questLogEntrySignature);
        }

        return questLogEntrySignature;
    }


    /// <summary>
    /// Create a new QuestStep and add it to the current graph, you can optionally specify the first action node for this quest step, and execution mode.
    /// </summary>
    /// <param name="QuestStepName"></param>
    /// <param name="FirstNode"></param>
    /// <param name="ExecutionMode">Defines if the actions on this QuestStep RunInSequence or Parrallel </param>
    /// <returns></returns>
    public QuestStep CreateNewQuestStep(string QuestStepName, ActionTask FirstNode = null, ActionList.ActionsExecutionMode ExecutionMode = ActionList.ActionsExecutionMode.ActionsRunInSequence, bool RepeatStateActions = false, FSMState.TransitionEvaluationMode transitionEvaluation = FSMState.TransitionEvaluationMode.CheckAfterStateFinished)
    {
        QuestStep questStep = new QuestStep();
        questStep.name = QuestStepName;
        questStep._graph = Graph;
        questStep.transitionEvaluation = transitionEvaluation;
        questStep.repeatStateActions = RepeatStateActions;

        if (questStep._actionList == null) questStep._actionList = new ActionList();     
        questStep._actionList.executionMode = ExecutionMode;
        if (FirstNode != null) questStep.actionList.AddAction(FirstNode);
        if (!GraphContainsNode(questStep)) Graph.allNodes.Add(questStep);
        return questStep;
    }

    public FSMState AddState(string StateName, FSMState FSMState)
    {
        FSMState._name = StateName;

        if (!CustomStates.ContainsKey(StateName))
        {
            CustomStates.Add(StateName, FSMState);

        }
        else
        {
            CustomStates[StateName] = FSMState;
        }

        return FSMState;
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

    /// <summary>
    /// A helper method to connect two distinct QuestStep nodes together on the graph, optional Condition can be added.
    /// </summary>
    /// <param name="QuestAction"></param>
    /// <param name="TargetAction"></param>
    /// <param name="ConnectionCondition"></param>
    /// <returns></returns>
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
        if (Graph != null && QuestTreeOwner != null)
        {
            EmoMountMod.Log.LogMessage($"Starting QuestGraph {Graph}");
            if (Graph.primeNode == null)
            {
                Graph.primeNode = Graph.allNodes[0];
            }

            QuestTreeOwner.StartBehaviour();
        }
    }




    public QuestAction_AddLogEntry AddStaticLogEntry(QuestStep QuestStep, string LogEntryText, bool DisplayTimeRecieved = false)
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