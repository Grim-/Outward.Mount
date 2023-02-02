using HarmonyLib;
using SideLoader;
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
    //[HarmonyPatch(typeof(Character), nameof(Character.Start))]
    //public class CharacterStartPatchLichLootPatch
    //{
    //    //make sure the method matches the 
    //    static void Postfix(Character __instance)
    //    {
    //        if (__instance.IsAI)
    //        {
   
    //               SL_DropTable dropTable = new SL_DropTable()
    //            {
    //                GuaranteedDrops = new List<SL_ItemDrop>()
    //                {
    //                     new SL_ItemDrop()
    //                     {
    //                          DroppedItemID = 4100320,
    //                          MinQty = 1,
    //                          MaxQty = 1
    //                     }
    //                },
    //                UID = "SOMEUID"
    //            };
    //            dropTable.ApplyTemplate();
    //            EmoMountMod.Log.LogMessage($"Adding {dropTable.UID} to {__instance.UID} ({__instance.Name})");

    //            __instance.Inventory.MakeLootable(false, true, true, false);

    //            dropTable.AddAsDropableToGameObject(__instance.Inventory.Pouch.gameObject, false, "OJWOJDWOJWO");
    //            dropTable.GenerateDrops(__instance.Inventory.Pouch.transform);
    //        }

    //    }
    //}
    //[HarmonyPatch(typeof(Character), nameof(Character.Die))]
    //public class GoldLich_Die_Patch
    //{
    //    //make sure the method matches the one you're trying to patch, with the exception of the first variable being Type __instance
    //    //so for this Character Type its Character __instance
    //    static void Prefix(Character __instance, Vector3 _hitVec, bool _loadedDead = false)
    //    {
    //        //do something for this specific character UID, it has just died, but ignore this if the character loaded dead (return to a scene where you had previously killed it for example)
    //        if (__instance.UID == "EwoPQ0iVwkK-XtNuaVPf3g" && !_loadedDead)
    //        {
    //            //CharacterManager.Instance.Characters is dictionary<string, Character> (where everything has a key, with a value aka a tuple or a kvp, many names same thing) 
    //            //we are getting all the values in the case of this dictionary,  the string 'key' is the characters UID and the 'value' is a reference to the Character component for that character
    //            //we find every character component in that Characters.ValuesArray (we dont care about what the UID key is so this is a property to JUSt return just an array of the characters)
    //            //where IsLocalPlayer returns true and put that into a new array (ToArray())
    //            //the none shorthand version is x.IsLocalPlayer == true
    //            Character[] playerCharacters = CharacterManager.Instance.Characters.ValuesArray.Where(x => x.IsLocalPlayer).ToArray();

    //            //we then loop that array

    //            //foreach character that is a player
    //            foreach (var player in playerCharacters)
    //            {
    //                //just show some notification
    //                player.CharacterUI.NotificationPanel.ShowNotification("You gained some berries, gg");
    //                // so we can call the the recieveitemreward which will generate the item from a itemID and put it whatever bag is best
    //                //the other way of giving items I showed works too but this one seems more straightforward, it does most of what the last one did for you
    //                player.Inventory.ReceiveItemReward(4100320, 100, false);
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Character), nameof(Character.Start))]
    //public class CharacterAwakePatchLichLootPatch
    //{
    //    //make sure the method matches the 
    //    static void Postfix(Character __instance)
    //    {
    //        //do something for this specific character UID, it has just died
    //        if (__instance.UID == "EwoPQ0iVwkK-XtNuaVPf3g")
    //        {
    //            SL_DropTable dropTable = new SL_DropTable()
    //            {
    //                GuaranteedDrops = new List<SL_ItemDrop>()
    //                {
    //                     new SL_ItemDrop()
    //                     {
    //                          DroppedItemID = 4100320,
    //                          MinQty = 1,
    //                          MaxQty = 1
    //                     }
    //                },
    //                UID = "SOMEUID"
    //            };
    //            dropTable.ApplyTemplate();

    //            LootableOnDeath lootableOnDeath = __instance.GetComponent<LootableOnDeath>();
    //            lootableOnDeath.enabled = true;
    //            lootableOnDeath.EnabledPouch = true;

    //            __instance.Inventory.MakeLootable(false, true, true, false);

    //            dropTable.AddAsDropableToGameObject(__instance.Inventory.Pouch.gameObject, false, "OJWOJDWOJWO");
    //            dropTable.GenerateDrops(__instance.Inventory.Pouch.transform);
    //        }

    //    }
    //}

    [HarmonyPatch(typeof(EnvironmentConditions), nameof(EnvironmentConditions.OnHour))]
    public class EnvironmentConditionsOnHourPatch
    {
        static void Postfix(EnvironmentConditions __instance)
        {
            EmoMountMod.Instance.OnGameHourPassed?.Invoke(TOD_Sky.Instance.Cycle.Hour);
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
    //[HarmonyPatch(typeof(InteractionDisplay), nameof(InteractionDisplay.SetInteractable))]
    //public class InteractionDisplayPatch
    //{
    //    static void Postfix(InteractionDisplay __instance, InteractionTriggerBase _interactionTrigger)
    //    {
    //        if (!_interactionTrigger || !_interactionTrigger.ItemToPreview || _interactionTrigger.ItemToPreview is not Bag)
    //            return;
    //        //maybe not the best way to do this but it fucken works.
    //        BasicMountController basicMountController = _interactionTrigger.ItemToPreview.gameObject.GetComponentInParent<BasicMountController>();
    //        if (basicMountController != null && basicMountController.BagContainer.UID == _interactionTrigger.ItemToPreview.UID)
    //        {
    //            //EmoMountMod.Log.LogMessage("Interaction Display UID is Mount Bag UID");

    //            __instance.m_interactionBag.Show(false);
    //        }
    //    }
    //}

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

                //DONT do the original method 
                return false;
            }
            //just do the original method
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
