using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_SummonActiveMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SummonActiveMount);
        public Type GameModel => typeof(SummonActiveMount);

        public override void ApplyToComponent<T>(T component)
        {

        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class SummonActiveMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SummonActiveMount);
        public Type GameModel => typeof(SummonActiveMount);

        public string SpeciesName;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();


            if (characterMount != null && characterMount.HasActiveMount)
            {
                if (characterMount.ActiveMountDisabled)
                {
                    characterMount.EnableActiveMount();
                }
                else
                {
                    characterMount.ActiveMount.Teleport(_affectedCharacter.transform.position, _affectedCharacter.transform.eulerAngles);
                }
              
            }
            else
            {
                EmoMountMod.Log.LogMessage($"Summon Active Mount, no Active Mount found for {_affectedCharacter.Name}.");
            }
        }
    }
}
