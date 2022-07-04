using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    public class CharacterAwakePatch
    {
        static void Postfix(Character __instance)
        {
            __instance.gameObject.AddComponent<CharacterMount>();
        }
    }

    [HarmonyPatch(typeof(DefeatScenariosManager), nameof(DefeatScenariosManager.ActivateDefeatScenario))]
    public class DefeatScenarioPatch
    {
        static void Prefix(DefeatScenariosManager __instance, DefeatScenario _scenario)
        {
            foreach (var item in EmoMountMod.MountManager.MountControllers)
            {
                EmoMountMod.Log.LogMessage($"Dismount {item.Key} from {item.Value.MountName} before defeat scenario");
                item.Value.DismountCharacter(item.Value.CharacterOwner);
            }
        }
    }


    [HarmonyPatch(typeof(Character), nameof(Character.Teleport), new Type[] { typeof(Vector3), typeof(Vector3) })]
    public class CharacterTeleport
    {
        static void Postfix(Character __instance, Vector3 _pos, Vector3 _rot)
        {
            CharacterMount characterMount = __instance.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount)
            {
                EmoMountMod.Log.LogMessage($"Warping {characterMount.ActiveMount.MountName} with {characterMount.Character.Name}");
                characterMount.ActiveMount.Teleport(_pos, _rot);
            }
        }
    }

    //SinaiSavesTheDay++
    [HarmonyPatch(typeof(InteractionDisplay), nameof(InteractionDisplay.SetInteractable))]
    public class InteractionDisplayPatch
    {
        static void Postfix(InteractionDisplay __instance, InteractionTriggerBase _interactionTrigger)
        {
            if (!_interactionTrigger || !_interactionTrigger.ItemToPreview || _interactionTrigger.ItemToPreview is not Bag)
                return;
            //maybe not the best way to do this but it fucken works.
            BasicMountController basicMountController = _interactionTrigger.ItemToPreview.gameObject.GetComponentInParent<BasicMountController>();
            if (basicMountController != null && basicMountController.BagContainer.UID == _interactionTrigger.ItemToPreview.UID)
            {
                //EmoMountMod.Log.LogMessage("Interaction Display UID is Mount Bag UID");

                __instance.m_interactionBag.Show(false);
            }
        }
    }

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
                __result.Add(69696969);
                // __result.Add(69696968);
            }

        }

        [HarmonyPatch(nameof(ItemDisplayOptionPanel.ActionHasBeenPressed)), HarmonyPrefix]
        private static void EquipmentMenu_ActionHasBeenPressed_Prefix(ItemDisplayOptionPanel __instance, int _actionID)
        {
            if (_actionID == 69696969)
            {
                Character owner = __instance.m_characterUI.TargetCharacter;
                CharacterMount characterMount = owner.GetComponent<CharacterMount>();

                if (characterMount != null && __instance.m_pendingItem is Food)
                {
                    Food foodItem = (Food)__instance.m_pendingItem;
                    float foodValue = 0;

                    foreach (var item in foodItem.m_affectFoodEffects)
                    {
                        foodValue += item.m_affectQuantity * item.m_totalPotency;
                    }

                    //EmoMountMod.Log.LogMessage($"Final Food Value? {foodValue} from {foodItem.Name}");

                    characterMount.ActiveMount.MountFood.Feed(__instance.m_pendingItem, foodValue);
                }
            }
            //else if (_actionID == 69696968)
            //{
            //    Character owner = __instance.m_characterUI.TargetCharacter;
            //    CharacterMount characterMount = owner.GetComponent<CharacterMount>();

            //    if (characterMount != null)
            //    {
            //        if (characterMount.ActiveMount.CanCarryWeight(__instance.m_pendingItem.Weight))
            //        {
            //            characterMount.ActiveMount.AddItemToBag(__instance.m_pendingItem);
            //        }
            //    }
            //}
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
                    __result = $"Feed {characterMount.ActiveMount.MountName}";                 
                }

                return false;
            }
            //else if (_actionID == 69696968)
            //{
            //    Character owner = __instance.m_characterUI.TargetCharacter;
            //    CharacterMount characterMount = owner.GetComponent<CharacterMount>();

            //    if (characterMount != null && characterMount.ActiveMount != null && characterMount.ActiveMount.BagContainer != null)
            //    {
            //        __result = $"Move To {characterMount.ActiveMount.MountName} Bag";
            //    }

            //    return false;
            //}

            return true;
        }



        //IggySavesTheDay
        [HarmonyPatch(typeof(ItemDropper), "GenerateItem")]
        public class ItemDropper_GenerateItem
        {
            [HarmonyPrefix]
            public static void Prefix(ItemDropper __instance, ItemContainer _container, BasicItemDrop _itemDrop, int _spawnAmount)
            {
                if (UnityEngine.Random.Range(EmoMountMod.WorldDropChanceMinimum.Value, EmoMountMod.WorldDropChanceMaximum.Value) <= EmoMountMod.WorldDropChanceThreshold.Value)
                {
                    Item item = ItemManager.Instance.GenerateItemNetwork(EmoMountMod.GetRandomWhistleID());
                    item.ChangeParent(_container.transform);
                }
            }
        }

    }
}
