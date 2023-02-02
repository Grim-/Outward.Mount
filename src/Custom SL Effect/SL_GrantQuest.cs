using SideLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_GrantQuest : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_GrantQuest);
        public Type GameModel => typeof(GrantQuest);

        public int QuestID;


        public override void ApplyToComponent<T>(T component)
        {
            GrantQuest comp = component as GrantQuest;
            comp.QuestID = QuestID;

        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class GrantQuest : Effect
    {
        public int QuestID;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (_affectedCharacter.IsLocalPlayer)
            {
                if (!_affectedCharacter.Inventory.QuestKnowledge.IsItemLearned(QuestID))
                {
                    _affectedCharacter.Inventory.QuestKnowledge.ReceiveQuest(QuestID);
                }
            }
        }
    }


}
