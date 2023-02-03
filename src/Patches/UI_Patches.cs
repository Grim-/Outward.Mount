using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount.Patches
{
    public class UI_Patches
    {
        ///FaerynSavesTheDay++
        [HarmonyPatch(typeof(ItemDisplayOptionPanel))]
        public static class ItemDisplayOptionPanelPatches
        {
            [HarmonyPatch(nameof(ItemDisplayOptionPanel.GetActiveActions)), HarmonyPostfix]
            private static void EquipmentMenu_GetActiveActions_Postfix(ItemDisplayOptionPanel __instance, GameObject pointerPress, ref List<int> __result)
            {
                CharacterMount characterMount = __instance.LocalCharacter.GetComponent<CharacterMount>();

                if (characterMount && characterMount.ActiveMount)
                {
                    if (__instance.m_pendingItem != null && __instance.m_pendingItem is Food)
                    {
                        if (characterMount.ActiveMount.MountFood.CanEat(__instance.m_pendingItem))
                        {
                            __result.Add(69696969);
                        }
                    }              
                }
            }

            [HarmonyPatch(nameof(ItemDisplayOptionPanel.ActionHasBeenPressed)), HarmonyPrefix]
            private static void EquipmentMenu_ActionHasBeenPressed_Prefix(ItemDisplayOptionPanel __instance, int _actionID)
            {
                if (_actionID == 69696969)
                {
                    Character owner = __instance.m_characterUI.TargetCharacter;
                    CharacterMount characterMount = owner.GetComponent<CharacterMount>();

                    if (characterMount != null)
                    {            
                        if (__instance.m_pendingItem != null && __instance.m_pendingItem is Food)
                        {
                            Food foodItem = (Food)__instance.m_pendingItem;
                            float foodValue = OutwardHelpers.GetTotalFoodValue(foodItem);
                            characterMount.ActiveMount.MountFood.Feed(__instance.m_pendingItem, foodValue);
                        }
                    }
                }
            }

            [HarmonyPatch(nameof(ItemDisplayOptionPanel.GetActionText)), HarmonyPrefix]
            private static bool EquipmentMenu_GetActionText_Prefix(ItemDisplayOptionPanel __instance, int _actionID, ref string __result)
            {
                if (_actionID == 69696969)
                {
                    Character owner = __instance.m_characterUI.TargetCharacter;
                    CharacterMount characterMount = owner.GetComponent<CharacterMount>();

                    if (characterMount != null && characterMount.ActiveMount != null)
                    {
                        if (__instance.m_pendingItem != null && __instance.m_pendingItem is Food)
                        {
                            __result = $"Feed to {characterMount.ActiveMount.MountName}";
                        }

                       
                    }

                    //DONT do the original method 
                    return false;
                }
                //just do the original method
                return true;
            }
        }
    }
}
