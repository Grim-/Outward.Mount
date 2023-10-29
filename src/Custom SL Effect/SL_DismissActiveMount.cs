using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_DismissActiveMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_DismissActiveMount);
        public Type GameModel => typeof(DismissActiveMount);

        public override void ApplyToComponent<T>(T component)
        {

        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class DismissActiveMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_DismissActiveMount);
        public Type GameModel => typeof(DismissActiveMount);


        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount && !characterMount.ActiveMountDisabled && characterMount.ActiveMount.CharacterOwner == _affectedCharacter)
            {
                if (!characterMount.ActiveMount.IsMounted)
                {
                    characterMount.DisableActiveMount();
                }
                else _affectedCharacter.CharacterUI.ShowInfoNotification($"{characterMount.ActiveMount.CurrentlyMountedCharacter} is riding this mount!");
            }
            else
            {
                EmoMountMod.LogMessage($"Dismiss Active Mount, no Active Mount found for {_affectedCharacter.Name}.");
            }
        
          
           
        }
    }




}
