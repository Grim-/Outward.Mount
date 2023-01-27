using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

            if (characterMount != null)
            {
                if (characterMount.HasActiveMount && characterMount.ActiveMount.CharacterOwner == _affectedCharacter)
                {
                    if (characterMount.ActiveMountDisabled)
                    {
                        characterMount.EnableActiveMount();
                    }
                    else
                    {
                        Vector3 Position = _affectedCharacter.transform.position;
                        Vector3 Rotation = _affectedCharacter.transform.eulerAngles;

                        characterMount.ActiveMount.Teleport(Position, Rotation, () =>
                        {
                            if (EmoMountMod.EnableFastMount.Value)
                            {
                                if (characterMount.ActiveMount.CanMount(_affectedCharacter))
                                {
                                    characterMount.ActiveMount.MountCharacter(_affectedCharacter);
                                }
                            }
                        });


         
                    }
                }              
            }
        }
    }
}
