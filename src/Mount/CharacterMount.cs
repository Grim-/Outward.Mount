using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmoMount
{

    /// <summary>
    /// This simply holds a reference to the characters mount instance, might not need, IDK yet.
    /// </summary>
    public class CharacterMount : MonoBehaviour
    {
        public BasicMountController ActiveMount
        {
            get; private set;
        }

        public List<MountInstanceData> StoredMounts = new List<MountInstanceData>();

        public bool HasActiveMount
        {
            get
            {
                return ActiveMount != null;
            }
        }


        public bool ActiveMountDisabled { get; private set; }

        public Character Character => GetComponent<Character>();

        public void SetActiveMount(BasicMountController newMount)
        {
            ActiveMount = newMount;
        }


        /// <summary>
        /// Serialize a BasicMountController Instance into MountInstanceData, add it to the StoredMounts List then Destroy it's GameObject.
        /// </summary>
        /// <param name="MountToStore"></param>
        public void StoreMount(BasicMountController MountToStore)
        {

            EmoMountMod.Log.LogMessage($"Storing Mount {MountToStore.MountName}");
            if (!HasStoredMount(MountToStore.MountUID))
            {
                MountInstanceData mountInstanceData = EmoMountMod.MountManager.CreateInstanceDataFromMount(MountToStore);
                DropAllStoredItems(MountToStore);
                StoredMounts.Add(mountInstanceData);
                MountToStore.DestroyBagContainer();
                EmoMountMod.MountManager.DestroyActiveMount(Character);
                SetActiveMount(null);
            }
            else
            {
                EmoMountMod.Log.LogMessage($"A Mount by this name is already stored {MountToStore.MountName}, updating");

                MountInstanceData StoredData = StoredMounts.Find(x => x.MountUID == MountToStore.MountUID);


                if (StoredData != null)
                {

                    MountInstanceData mountInstanceData = EmoMountMod.MountManager.CreateInstanceDataFromMount(MountToStore);
                    DropAllStoredItems(MountToStore);
                    StoredData = mountInstanceData;

                    MountToStore.DestroyBagContainer();
                    EmoMountMod.MountManager.DestroyActiveMount(Character);
                    SetActiveMount(null);
                }
                else
                {
                    EmoMountMod.Log.LogMessage($"Mount Stored Data is null?");
                }

            }
        }


        private void DropAllStoredItems(BasicMountController MountToStore)
        {      
            if (MountToStore.BagContainer != null)
            {
                if (MountToStore.BagContainer is Bag)
                {
                    Bag ContainerBag = MountToStore.BagContainer as Bag;
                    EmoMountMod.Log.LogMessage($"DropAllStoredItems BagContainer Contained Item Count {ContainerBag.Container.GetContainedItems().Count}");
                    MountToStore.CharacterOwner.Inventory.TakeAllContent(ContainerBag.Container, true);
                    MountToStore.DisplayNotification("Mount Inventory contents added to your Inventory.");
                }
                else
                {
                    EmoMountMod.Log.LogMessage($"DropAllStoredItems BagContainer isnt a bag.");
                }

            }
            else
            {
                EmoMountMod.Log.LogMessage($"DropAllStoredItems BagContainer is null.");
            }
        }

        /// <summary>
        /// DeSerialize a MountInstanceData and create a BasicMountController from the data.
        /// </summary>
        /// <param name="MountUID"></param>
        /// <returns></returns>
        public BasicMountController RetrieveStoredMount(string MountUID)
        {
            if (StoredMounts != null && HasStoredMount(MountUID))
            {
                MountInstanceData mountInstanceData = GetStoredMountData(MountUID);
                BasicMountController basicMountController =  EmoMountMod.MountManager.CreateMountFromInstanceData(Character, mountInstanceData);
                basicMountController.Teleport(Character.transform.position, mountInstanceData.Rotation);
                StoredMounts.Remove(mountInstanceData);
                return basicMountController;
            }

            return null;
        }

        public void EnableActiveMount()
        {
            if (HasActiveMount)
            {
                this.ActiveMount.gameObject.SetActive(true);
                this.ActiveMount.Teleport(Character.transform.position, Character.transform.eulerAngles);
                ActiveMountDisabled = false;
            }
        }

        public void DisableActiveMount()
        {
            if (HasActiveMount)
            {
                this.ActiveMount.gameObject.SetActive(false);
                ActiveMountDisabled = true;
            }
        }


        public MountInstanceData GetStoredMountData(string MountUID)
        {
            return StoredMounts.Find(x => x.MountUID == MountUID);
        }

        public bool HasStoredMount(string MountUID)
        {
            return StoredMounts.Find(x => x.MountUID == MountUID) != null ? true: false;
        }
    }
}
