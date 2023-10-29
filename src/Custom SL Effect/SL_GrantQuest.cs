using SideLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            GrantQuest comp = effect as GrantQuest;
            this.QuestID = comp.QuestID;
        }
    }

    public class GrantQuest : Effect
    {
        public int QuestID;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (_affectedCharacter.IsLocalPlayer)
            {
                if (!EmoMountMod.QuestManager.CharacterHasQuest(_affectedCharacter, QuestID))
                {
                    Quest Quest = MountQuestManager.GenerateQuestItemForCharacter(_affectedCharacter, QuestID);
                    OutwardHelpers.DelayDo(() =>
                    {
                        MountQuestManager.StartQuestGraphForQuest(Quest);
                    }, 1f);
                }
            }
        }
    }
}
