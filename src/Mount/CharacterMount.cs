using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            if (!HasStoredMount(MountToStore.MountUID))
            {
                MountInstanceData mountInstanceData = EmoMountMod.MountManager.CreateInstanceDataFromMount(MountToStore);
                // EmoMountMod.MountManager.SerializeMountBagContents(mountInstanceData, MountToStore);
                DropAllStoredItems(MountToStore);
                StoredMounts.Add(mountInstanceData);
                MountToStore.DestroyBagContainer();
                EmoMountMod.MountManager.DestroyActiveMount(Character);
                SetActiveMount(null);
            }
        }


        private void DropAllStoredItems(BasicMountController MountToStore)
        {      
            if (MountToStore.BagContainer != null && MountToStore.BagContainer is Bag)
            {
                Bag ContainerBag = MountToStore.BagContainer as Bag;


                foreach (var item in ContainerBag.Container.GetContainedItems().ToList())
                {
                    EmoMountMod.Log.LogMessage($"Dropping {item.Name} from Mount Bag {ContainerBag.Container.UID}");
                    
                    item.ChangeParent(null, MountToStore.CharacterOwner.transform.position + new Vector3(0, 2f, 0));
                    ContainerBag.Container.RemoveItem(item);
                    //(MountToStore.BagContainer as Bag).m_container.RemoveItem(item);
                }
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
                //EmoMountMod.MountManager.DeSerializeMountBagContents(mountInstanceData, basicMountController);
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
