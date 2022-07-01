using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_DismissActiveMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public override void ApplyToComponent<T>(T component)
        {

        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class DismissActiveMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SpeciesName;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

                if (characterMount != null)
                {
                    BasicMountController basicMountController = EmoMountMod.MountManager.GetActiveMount(_affectedCharacter);

                    if (basicMountController != null)
                    {
                        characterMount.StoreMount(basicMountController);
                    }           
                }
            }
            else
            {
                EmoMountMod.Log.LogMessage($"Dismiss Active Mount, no Active Mount found for {_affectedCharacter.Name}.");
            }
        }
    }
}
