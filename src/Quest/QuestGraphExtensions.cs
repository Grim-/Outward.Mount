using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using NodeCanvas.Tasks.Actions;
using System.Collections.Generic;

namespace EmoMount
{   
    public static class QuestGraphExtensions
    {
        public static QuestStep AddQuestAction(this QuestStep QuestStep, ActionTask Action)
        {
            if (QuestStep._actionList == null)
            {
                QuestStep._actionList = new ActionList();
            }
            QuestStep._actionList.AddAction(Action);
            return QuestStep;
        }

        public static List<ConditionTask> GetCurrentConnectionConditions(this QuestStep QuestStep)
        {
            List<ConditionTask> conditionTasks = new List<ConditionTask>();

            foreach (var item in QuestStep.outConnections)
            {
                FSMConnection fsmconnection = (FSMConnection)item;
                ConditionTask condition = fsmconnection.condition;

                if (condition != null)
                {
                    conditionTasks.Add(condition);
                }
            }
            return conditionTasks;
        }

        public static QuestTreeOwner GetQuestTreeOwner(this Quest Quest)
        {
            return Quest?.m_questProgress?.m_questSequence;
        }

        public static QuestTree GetQuestTree(this Quest Quest)
        {
            return (QuestTree)(Quest?.m_questProgress?.m_questSequence?.graph);
        }
        public static QuestStep AddLogEntryQuestAction(this QuestStep QuestStep, string LogEntryText, bool DisplayTimeRecieved = false)
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
            return QuestStep;
        }
        public static QuestStep AddSendQuestEventAction(this QuestStep QuestStep, string EventUID)
        {
            SendQuestEvent sendQuestEvent = new SendQuestEvent()
            {
                QuestEventRef = new QuestEventReference()
                {
                    EventUID = EventUID
                }
            };

            QuestStep.AddQuestAction(sendQuestEvent);
           return QuestStep;
        }
    }
}
