using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.Die))]
    public class OnDieDropPatch
    {
        static void Prefix(Character __instance, Vector3 _hitVec, bool _loadedDead = false)
        {
            if (!_loadedDead)
            {
                foreach (var dropData in EmoMountMod.OnDeathDrops)
                {
                    if (__instance.UID == dropData.CharacterUID)
                    {
                        OutwardHelpers.GrantItemRewardToAllPlayers(dropData.ItemID, dropData.Quantity);
                    }
                }
            }
        }
    }
}
