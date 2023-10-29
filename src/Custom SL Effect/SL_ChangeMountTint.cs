using SideLoader;
using System;
using System.ComponentModel;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_ChangeMountTint : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_ChangeMountTint);
        public Type GameModel => typeof(ChangeMountTint);

        public Color TintColor;

        public override void ApplyToComponent<T>(T component)
        {
            ChangeMountTint comp = component as ChangeMountTint;
            comp.TintColor = TintColor;
        }

        public override void SerializeEffect<T>(T effect)
        {
            ChangeMountTint comp = effect as ChangeMountTint;
            this.TintColor = comp.TintColor;
        }
    }
    public class ChangeMountTint : Effect
    {
        public Color TintColor;
        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount && !characterMount.ActiveMountDisabled)
            {
                if (characterMount.ActiveMountDisabled)
                {
                    characterMount.EnableActiveMount();
                }

                characterMount.ActiveMount.SetTintColor(TintColor);
            }
            else
            {
                EmoMountMod.LogMessage($"ChangeMountTint, no Active Mount found for {_affectedCharacter.Name}.");
            }
        }
    }

}
