using EmoMount.UI;
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

        //bottom left hud elemet
        public RectTransform UIContainer
        {
            get; private set;
        }

        public CanvasGroup UIContainerGroup
        {
            get; private set;
        }

        //vertical layout group
        public RectTransform StorageContainer
        {
            get; private set;
        }

        public CanvasGroup StorageContainerGroup
        {
            get; private set;
        }

        public GameObject StorageUIPrefab
        {
            get; private set;
        }

        private Dictionary<BasicMountController, MountUI> MountUIInstances = new Dictionary<BasicMountController, MountUI>();
        private List<GameObject> StorageContainerInstances = new List<GameObject>();

        public void Awake()
        {
            Instance = this;
            ParentCanvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();


            UIContainer = transform.Find("UIContainer").GetComponent<RectTransform>();
            UIContainerGroup = UIContainer.GetComponent<CanvasGroup>();


            if (UIContainer == null)
            {
                EmoMountMod.Log.LogMessage($"Mount Canvas Manager UI Container cannot be found.");
            }


            StorageContainer = transform.Find("StorageContainer").GetComponent<RectTransform>();
            StorageContainerGroup = StorageContainer.GetComponent<CanvasGroup>();


            if (StorageContainer == null)
            {
                EmoMountMod.Log.LogMessage($"Mount Canvas Manager UI Storage Container cannot be found.");
            }


            MountUIPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", "MountUIPrefab");
            StorageUIPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", "StoredMountPrefab");


            if (MountUIPrefab == null)
            {
                EmoMountMod.Log.LogMessage("Mount UI Prefab could not be found");
                return;
            }


            HideMountHud();
            HideStorage();
        }

      
        public void Show()
        {
            if (UIContainerGroup != null)
            {
                UIContainerGroup.alpha = 1;
            }
        }

        public void HideMountHud()
        {
            if (UIContainerGroup != null)
            {
                UIContainerGroup.alpha = 0;
            }
        }

        public void DisplayStorageForCharacter(Character character)
        {
            EmoMountMod.Log.LogMessage($"Displaying Storage UI for {character.Name}");

            if (StorageUIPrefab == null)
            {
                EmoMountMod.Log.LogMessage("No Storage UI Prefab found");
                return;
            }

            SideLoader.Helpers.ForceUnlockCursor.AddUnlockSource();

            CharacterMount characterMount = character.GetComponent<CharacterMount>();


            if (characterMount != null)
            {

                if (characterMount.StoredMounts.Count == 0)
                {
                    EmoMountMod.Log.LogMessage($"No Stored Mounts");
                    //no mounts to show
                    return;
                }


                foreach (var item in characterMount.StoredMounts)
                {
                    EmoMountMod.Log.LogMessage($"Creating Stored Mount UI Selection Instance for {item.MountName}");

                    GameObject mountSelectionInstance = GameObject.Instantiate<GameObject>(StorageUIPrefab);
                    MountSelectionElement mountSelectionElement = mountSelectionInstance.AddComponent<MountSelectionElement>();

                    mountSelectionElement.SetNameLabel($"{item.MountName} ({item.MountSpecies.SpeciesName})");
                    mountSelectionElement.SetFoodLabel($"Food : {item.CurrentFood} / {item.MaximumFood}");
                    mountSelectionElement.RetrieveButton.onClick.AddListener(() =>
                    {
                        BasicMountController basicMountController = characterMount.RetrieveStoredMount(item.MountUID);
                        HideStorage();
                        SideLoader.Helpers.ForceUnlockCursor.RemoveUnlockSource();
                    });

                    StorageContainerInstances.Add(mountSelectionInstance);

                    mountSelectionInstance.transform.SetParent(StorageContainer, false);
                }

                ShowStorage();
            }
            else
            {
                EmoMountMod.Log.LogMessage($"CharacterMount component not found for  {character.Name}");
            }


        }

        public void ShowStorage()
        {
            EmoMountMod.Log.LogMessage($"Showing Storage UI");
            StorageContainerGroup.alpha = 1;
            StorageContainerGroup.interactable = true;
        }

        public void HideStorage()
        {
            EmoMountMod.Log.LogMessage($"Hiding Storage UI");
            StorageContainerGroup.alpha = 0;
            StorageContainerGroup.interactable = false;


            EmoMountMod.Log.LogMessage($"Destroying Storage Container Instances");
            foreach (var item in StorageContainerInstances)
            {
                Destroy(item);
            }

            StorageContainerInstances.Clear();
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
                    HideMountHud();
                }
            }
        }
    }

}
