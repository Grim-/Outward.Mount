using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

namespace EmoMount
{
    public class DialogueTreeBuilder
    {
        public DialogueTree TargetDialogueTree { get; private set; }
        public DialogueTree.ActorParameter Actor { get; private set; }

        public DialogueTreeBuilder(DialogueTree targetDialogueTree, bool ClearGraph = true)
        {
            TargetDialogueTree = targetDialogueTree;
            Actor = TargetDialogueTree.actorParameters[0];
            if(ClearGraph) TargetDialogueTree.allNodes.Clear();
        }


        public StatementNodeExt SetInitialStatement(string InitialStatement)
        {
            // Add our root statement
            StatementNodeExt InitialStatementNode = CreateStatementNode(InitialStatement);
            TargetDialogueTree.primeNode = InitialStatementNode;
            return InitialStatementNode;
        }

        public MultipleChoiceNodeExt AddMultipleChoiceNode(string[] choices, ConditionTask[] Condition = null)
        {
            MultipleChoiceNodeExt multiChoice = TargetDialogueTree.AddNode<MultipleChoiceNodeExt>();

            for (int i = 0; i < choices.Length; i++)
            {
                MultipleChoiceNodeExt.Choice multipleChoice = new MultipleChoiceNodeExt.Choice()
                {
                    statement = new Statement()
                    {
                        text = choices[i]
                    },

                    condition = Condition != null && Condition[i] != null ? Condition[i] : null
                };

                multiChoice.availableChoices.Add(multipleChoice);
            }

            TargetDialogueTree.allNodes.Add(multiChoice);
            return multiChoice;
        }

        public StatementNodeExt CreateStatementNode(string AnswerText)
        {
            StatementNodeExt AnswerStatement = TargetDialogueTree.AddNode<StatementNodeExt>();
            AnswerStatement.statement = new(AnswerText);
            AnswerStatement.SetActorName(Actor.name);
            TargetDialogueTree.allNodes.Add(AnswerStatement);
            return AnswerStatement;
        }
        public DTNode AddAnswerToMultipleChoice(MultipleChoiceNodeExt multiChoice, int answerIndex, string AnswerText, DTNode AnswerNode)
        {
            StatementNodeExt AnswerStatement = TargetDialogueTree.AddNode<StatementNodeExt>();
            AnswerStatement.statement = new(AnswerText);
            AnswerStatement.SetActorName(Actor.name);

            TargetDialogueTree.allNodes.Add(AnswerStatement);
            if(AnswerNode != null) TargetDialogueTree.allNodes.Add(AnswerNode);
            TargetDialogueTree.ConnectNodes(multiChoice, AnswerStatement, answerIndex);
            if (AnswerNode != null) TargetDialogueTree.ConnectNodes(AnswerStatement, AnswerNode);
            return AnswerStatement;
        }
    }

    public static class NodeExtensions
    {
        public static DTNode ConnectTo(this DTNode sourceNode, DialogueTree DT, DTNode TargetNode)
        {
            if (!DT.allNodes.Contains(TargetNode))
            {
                DT.allNodes.Add(TargetNode);
            }

            DT.ConnectNodes(sourceNode, TargetNode);
            return TargetNode;
        }



        //public static DTNode ConnectFailureNode(this MultipleChoiceNodeExt sourceNode, DialogueTree DT, DTNode TargetNode)
        //{
        //    DT.ConnectNodes(sourceNode, TargetNode, -1, 1);
        //    if (!DT.allNodes.Contains(TargetNode))
        //    {
        //        DT.allNodes.Add(TargetNode);
        //    }
        //    sourceNode.availableChoices.
        //    return TargetNode;
        //}
    }
}
