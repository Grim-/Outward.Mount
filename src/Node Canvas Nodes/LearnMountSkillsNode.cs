﻿using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class LearnMountSkillsNode : ActionNode
        {
            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                if (PlayerTalking.Inventory.SkillKnowledge.GetItemFromItemID(-26400) == null)
                {
                    PlayerTalking.Inventory.ReceiveSkillReward(-26400);
                }

                if (PlayerTalking.Inventory.SkillKnowledge.GetItemFromItemID(-26401) == null)
                {
                    PlayerTalking.Inventory.ReceiveSkillReward(-26401);
                }

                return Status.Success;
            }
        }
    }
}
