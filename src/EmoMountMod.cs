using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EmoMount
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class EmoMountMod : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "emo.mountmod";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Emo's Mount Mod";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        public const string MOUNT_DISMOUNT_KEY = "MountMod_Dismount";
        public const string MOUNT_FOLLOW_WAIT_TOGGLE = "MountMod_FollowWait_Toggle";
        public const string MOUNT_MOVE_TO_KEY = "MountMod_MoveTo_Toggle";

        public static bool Debug = true;

        // For accessing your BepInEx Logger from outside of this class (MyMod.Log)
        internal static ManualLogSource Log;
        public static Canvas MainCanvas
        {
            get; private set;
        }

        public static MountManager MountManager
        {
            get; private set;
        }

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;

            InitKeybinds();
            MountManager = new MountManager();
            SL.BeforePacksLoaded += SL_BeforePacksLoaded;
            SL.OnPacksLoaded += SL_OnPacksLoaded;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            new Harmony(GUID).PatchAll();
        }

        private void SceneManager_sceneLoaded(Scene Scene, LoadSceneMode LoadMode)
        {
            if (Scene.name == "MainMenu_Empty")
            {
                MountManager.DestroyAllMountInstances();
            }
        }

        private void SL_BeforePacksLoaded()
        {
            InitTestItems();
        }

        private void SL_OnPacksLoaded()
        {
            InitializeCanvas();
        }

        private void InitKeybinds()
        {
            CustomKeybindings.AddAction(MOUNT_DISMOUNT_KEY, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_FOLLOW_WAIT_TOGGLE, KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction(MOUNT_MOVE_TO_KEY, KeybindingsCategory.CustomKeybindings);
        }

        private void InitTestItems()
        {
            //SL_Item Test_WolfWhistle = new SL_Item()
            //{
            //    Target_ItemID = 4300130,
            //    New_ItemID = -26995,
            //    Name = "Wolf  whistle",
            //    Description = "Test",
            //    EffectBehaviour = EditBehaviours.Destroy,
            //    EffectTransforms = new SL_EffectTransform[]
            //     {
            //        new SL_EffectTransform
            //        {
            //            TransformName = "Normal",
            //            Effects = new SL_Effect[]
            //            {
            //                new SL_SpawnMount
            //                {
            //                    SLPackName = "mount",
            //                    AssetBundleName = "emomountbundle",
            //                    PrefabName = "Mount_Wolf",
            //                    MountSpeed = 7,
            //                    RotateSpeed = 90,
            //                }
            //            }
            //        }
            //    }
            //};

            //Test_WolfWhistle.ApplyTemplate();
        }
        private void InitializeCanvas()
        {
            GameObject CanvasPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", "MountCanvas");

            if (CanvasPrefab != null)
            {
                MainCanvas = GameObject.Instantiate(CanvasPrefab).GetComponent<Canvas>();
                MainCanvas.gameObject.AddComponent<MountCanvasManager>();
                DontDestroyOnLoad(MainCanvas);
            }
            else
            {
                Log.LogMessage("CanvasPrefab was null");
            }

        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    public class CharacterAwakePatch
    {
        static void Postfix(Character __instance)
        {
            __instance.gameObject.AddComponent<CharacterMount>();
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Teleport), new Type[] { typeof(Vector3), typeof(Vector3)})]
    public class CharacterTeleport
    {
        static void Postfix(Character __instance, Vector3 _pos, Vector3 _rot)
        {
            CharacterMount characterMount = __instance.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasMount)
            {
                EmoMountMod.Log.LogMessage($"Warping {characterMount.Mount.MountName} with {characterMount.Character.Name}");
                characterMount.Mount.NavMesh.Warp(_pos);
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
}
