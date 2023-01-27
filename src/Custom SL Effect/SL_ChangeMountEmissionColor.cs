using SideLoader;
using System;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_ChangeMountEmissionColor : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_ChangeMountEmissionColor);
        public Type GameModel => typeof(ChangeMountEmissionColor);

        public Color Color;

        public override void ApplyToComponent<T>(T component)
        {
            ChangeMountEmissionColor comp = component as ChangeMountEmissionColor;
            comp.Color = Color;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class ChangeMountEmissionColor : Effect
    {
        public Color Color;
        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount && !characterMount.ActiveMountDisabled)
            {
                characterMount.ActiveMount.SetEmissionColor(Color);
            }
        }
    }
}
