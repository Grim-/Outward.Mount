using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount.Patches
{

    [HarmonyPatch(typeof(ItemContainer), nameof(ItemContainer.AddItem), new Type[] { typeof(Item), typeof(bool) })]
    public class ItemContainerAddItem
    {
        static void Postfix(ItemContainer __instance, Item _item, bool _stackIfPossible)
        {
            if (__instance.OwnerCharacter && __instance.OwnerCharacter.IsLocalPlayer)
            {
                CharacterMount characterMount = __instance.OwnerCharacter.gameObject.GetComponent<CharacterMount>();

                if (characterMount)
                {
                    characterMount.OnItemPicked?.Invoke(_item);
                }
            }
        }
    }
}
