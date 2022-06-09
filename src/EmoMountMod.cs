using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using System;
using System.Linq;
using System.Text;

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

        // If you need settings, define them like so:
        public static ConfigEntry<bool> ExampleConfig;

        public const string MOUNT_KEY = "EmoMountToggleKey";

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;

            InitKeybinds();   
            //new Harmony(GUID).PatchAll();
        }


        private void InitKeybinds()
        {
            CustomKeybindings.AddAction(MOUNT_KEY, KeybindingsCategory.CustomKeybindings);
        }


        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {

        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    public class Character_Awake
    {
        [HarmonyPrefix]
        static void Prefix(Character __instance)
        {
            //on awake for whatever character this is add the component
            __instance.gameObject.AddComponent<CharacterMountController>();
        }
    }
}
