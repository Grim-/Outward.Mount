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
            CustomKeybindings.AddAction("TEST_DISMOUNT_BUTTON", KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction("TEST_FOLLOWWAIT_TOGGLE_BUTTON", KeybindingsCategory.CustomKeybindings);
            CustomKeybindings.AddAction("TEST_MOVETO_BUTTON", KeybindingsCategory.CustomKeybindings);
        }

        private void InitTestItems()
        {
            SL_Item Test_WolfWhistle = new SL_Item()
            {
                Target_ItemID = 4300130,
                New_ItemID = -26995,
                Name = "Wolf  whistle",
                Description = "Test",
                EffectBehaviour = EditBehaviours.Destroy,
                EffectTransforms = new SL_EffectTransform[]
    {
                    new SL_EffectTransform
                    {
                        TransformName = "Effects",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount
                            {
                                SLPackName = "mount",
                                AssetBundleName = "emomountbundle",
                                PrefabName = "Mount_Wolf",
                                MountSpeed = 7,
                                RotateSpeed = 90,
                            }
                        }
                    }
    }
            };

            Test_WolfWhistle.ApplyTemplate();


            SL_Item Test_CosmicWolfWhistle = new SL_Item()
            {
                Target_ItemID = 4300130,
                New_ItemID = -26993,
                Name = "Wolf (Cosmic) whistle",
                Description = "Test",
                EffectBehaviour = EditBehaviours.Destroy,
                EffectTransforms = new SL_EffectTransform[]
{
                    new SL_EffectTransform
                    {
                        TransformName = "Effects",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount
                            {
                                SLPackName = "mount",
                                AssetBundleName = "emomountbundle",
                                PrefabName = "Mount_Wolf_Cosmic",
                                MountSpeed = 7,
                                RotateSpeed = 90,
                            }
                        }
                    }
}
            };

            Test_CosmicWolfWhistle.ApplyTemplate();

            SL_Item Test_HolyWolfWhistle = new SL_Item()
            {
                Target_ItemID = 4300130,
                New_ItemID = -26992,
                Name = "Wolf (Holy) whistle",
                Description = "Test",
                EffectBehaviour = EditBehaviours.Destroy,
                EffectTransforms = new SL_EffectTransform[]
{
                    new SL_EffectTransform
                    {
                        TransformName = "Effects",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount
                            {
                                SLPackName = "mount",
                                AssetBundleName = "emomountbundle",
                                PrefabName = "Mount_Wolf_Holy",
                                MountSpeed = 7,
                                RotateSpeed = 90,
                            }
                        }
                    }
}
            };

            Test_HolyWolfWhistle.ApplyTemplate();

            SL_Item Test_GolemWhistle = new SL_Item()
            {
                Target_ItemID = 4300130,
                New_ItemID = -26994,
                Name = "Golem whistle",
                Description = "Test",
                EffectBehaviour = EditBehaviours.Destroy,
                EffectTransforms = new SL_EffectTransform[]
                {
                    new SL_EffectTransform
                    {
                        TransformName = "Effects",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount
                            {
                                SLPackName = "mount",
                                AssetBundleName = "emomountbundle",
                                PrefabName = "Mount_RockGolem",
                                MountSpeed = 7,
                                RotateSpeed = 90,
                            }
                        }
                    }
             }
            };

            Test_GolemWhistle.ApplyTemplate();

            SL_Item Test_RaptorWhistle = new SL_Item()
            {
                Target_ItemID = 4300130,
                New_ItemID = -26900,
                Name = "Raptor whistle",
                Description = "Test",
                EffectBehaviour = EditBehaviours.Destroy,
                EffectTransforms = new SL_EffectTransform[]
    {
                    new SL_EffectTransform
                    {
                        TransformName = "Effects",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount
                            {
                                SLPackName = "mount",
                                AssetBundleName = "emomountbundle",
                                PrefabName = "Mount_Raptor",
                                MountSpeed = 9,
                                RotateSpeed = 90,
                            }
                        }
                    }
 }
            };

            Test_RaptorWhistle.ApplyTemplate();
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
}
