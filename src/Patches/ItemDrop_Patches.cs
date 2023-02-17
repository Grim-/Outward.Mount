using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount.Patches
{
    public class ItemDrop_Patches
    {
        //IggySavesTheDay
        [HarmonyPatch(typeof(ItemDropper), "GenerateItem")]
        public class ItemDropper_GenerateItem
        {
            [HarmonyPrefix]
            public static void Prefix(ItemDropper __instance, ItemContainer _container, BasicItemDrop _itemDrop, int _spawnAmount)
            {
                if (EmoMountMod.DisableNonNinedots.Value != true)
                {
                    if (UnityEngine.Random.Range(EmoMountMod.WorldDropChanceMinimum.Value, EmoMountMod.WorldDropChanceMaximum.Value) <= EmoMountMod.WorldDropChanceThreshold.Value)
                    {
                        Item item = ItemManager.Instance.GenerateItemNetwork(EmoMountMod.MountManager.GetRandomWhistleID());
                        item.ChangeParent(_container.transform);
                    }
                }


            }
        }
    }
}
