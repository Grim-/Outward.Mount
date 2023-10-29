using EmoMount.Mounted_States;
using HarmonyLib;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

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
                if (item.Value.CurrentlyMountedCharacter != null)
                {
                    item.Value.DismountCharacter(item.Value.CurrentlyMountedCharacter);
                }
                //EmoMountMod.Log.LogMessage($"Dismount {item.Key} from {item.Value.MountName} before defeat scenario");
              
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Die))]
    public class CharacterDiePatch
    {
        static void Prefix(Character __instance, Vector3 _hitVec, bool _loadedDead = false)
        {
            if (!_loadedDead)
            {
                //EmoMountMod.Log.LogMessage($"CharacterDiePatch.");

                CharacterMount characterMount = __instance.gameObject.GetComponent<CharacterMount>();

                if (characterMount)
                {
                    if (characterMount.IsTransformed && characterMount.ActiveMount)
                    {
                        //EmoMountMod.Log.LogMessage($"CharacterDiePatch. IsTransformed");
                        characterMount.ActiveMount.DismountCharacter(__instance);
                    }
                    else if (characterMount.ActiveMount && characterMount.ActiveMount.CurrentlyMountedCharacter == __instance && characterMount.IsMounted)
                    {
                        //EmoMountMod.Log.LogMessage($"Character died while mounted, dismounting.");
                        characterMount.ActiveMount.DismountCharacter(__instance);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.CancelActions))]
    public class CharacterCancelActionsPatch
    {
        static void Prefix(Character __instance)
        {
            //EmoMountMod.Log.LogMessage($"CancelActions.");

            CharacterMount characterMount = __instance.gameObject.GetComponent<CharacterMount>();

            if (characterMount)
            {
                if (characterMount.IsTransformed || characterMount.IsMounted)
                {
                   // EmoMountMod.Log.LogMessage($"CancelActions. Returning Because mounted or transformed");
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Teleport), new Type[] { typeof(Vector3), typeof(Vector3) })]
    public class CharacterTeleport
    {
        static void Postfix(Character __instance, Vector3 _pos, Vector3 _rot)
        {
            CharacterMount characterMount = __instance.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null)
            {
                if (characterMount.HasActiveMount)
                {
                    if (characterMount.ActiveMount.IsTransform || characterMount.IsMounted)
                    {
                        characterMount.ActiveMount.DismountCharacter(characterMount.ActiveMount.CurrentlyMountedCharacter);
                    }

                    //if (characterMount.IsMounted)
                    //{
                    //    characterMount.ActiveMount.DismountCharacter(characterMount.ActiveMount.CurrentlyMountedCharacter);
                    //}

                    //only teleport if the player is not mounted on the active mount
                    if (characterMount.ActiveMount.CurrentlyMountedCharacter != __instance)
                    {
                        characterMount.ActiveMount.Teleport(_pos, _rot);
                    }
                    else
                    {
                        EmoMountMod.Log.LogMessage($"Character is mounted while teleport is sent, died while mounted, dismounting.");
                    }

                   
                }

                //EmoMountMod.Log.LogMessage($"Warping {characterMount.ActiveMount.MountName} with {characterMount.Character.Name}");
                
            }
        }
    }

}
