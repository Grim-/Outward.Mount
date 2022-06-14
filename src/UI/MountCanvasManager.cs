using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Probably change to have the mount itself register and create a ui from prefab, then have this add it to a layoutcomponent or somethin.
    /// </summary>
    public class MountCanvasManager : MonoBehaviour
    {
        public Canvas ParentCanvas
        {
            get; private set;
        }

        public CanvasGroup CanvasGroup
        {
            get; private set;
        }

        public static MountCanvasManager Instance
        {
            get; private set;
        }

        public GameObject MountUIPrefab
        {
            get; private set;
        }

        public Transform UIContainer
        {
            get; private set;
        }

        private Dictionary<BasicMountController, MountUI> MountUIInstances = new Dictionary<BasicMountController, MountUI>();

        public void Awake()
        {
            Instance = this;
            ParentCanvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
            UIContainer = transform.Find("UIContainer");

            MountUIPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", "MountUIPrefab");

            if (MountUIPrefab == null)
            {
                EmoMountMod.Log.LogMessage("Mount UI Prefab could not be found");
                return;
            }


            Hide();
        }

      
        public void Show()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 1;
            }
        }

        public void Hide()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 0;
            }
        }


        /// <summary>
        /// Register a Mount Controller with the MountCanvasManager, this creates and returns an Instance of MountUI for the MountController.
        /// </summary>
        /// <param name="mountController"></param>
        /// <returns></returns>
        public MountUI RegisterMount(BasicMountController mountController)
        {
            if (!MountUIInstances.ContainsKey(mountController))
            {
                GameObject MountUIInstance = GameObject.Instantiate(MountUIPrefab, UIContainer.transform);
                MountUI mountUI = MountUIInstance.AddComponent<MountUI>();
                mountUI.SetTargetMount(mountController);
                MountUIInstances.Add(mountController, mountUI);

                Show();
                return mountUI;
            }

            return null;
        }

        public void UnRegisterMount(BasicMountController mountController)
        {
            if (MountUIInstances.ContainsKey(mountController))
            {
                GameObject.Destroy(MountUIInstances[mountController].gameObject);
                MountUIInstances.Remove(mountController);

                if (MountUIInstances.Count == 0)
                {
                    Hide();
                }
            }
        }
    }
}
