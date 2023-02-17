using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EmoMount.MountQuestManager;

namespace EmoMount
{

    /// <summary>
    /// This simply holds a reference to the characters mount instance, might not need, IDK yet.
    /// </summary>
    public class CharacterMount : MonoBehaviour
    {
        #region Properties
        public BasicMountController ActiveMount
        {
            get; private set;
        }

        public Dictionary<string, MountInstanceData> StoredMounts = new Dictionary<string, MountInstanceData>();

        public bool HasActiveMount
        {
            get
            {
                return ActiveMount != null;
            }
        }
        public bool IsMounted { get; private set; }
        public bool ActiveMountDisabled { get; private set; }
        public Character Character => GetComponent<Character>();
        #endregion

        public Action<Item> OnItemPicked;

        public void Start()
        {
            SetIsMounted(false);
        }

        public void SetActiveMount(BasicMountController newMount)
        {
            ActiveMount = newMount;
        }

        public void SetIsMounted(bool isMounted)
        {
            IsMounted = isMounted;
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
                AddMountToStore(MountToStore);
            }
            else
            {
                EmoMountMod.Log.LogMessage($"Updating {MountToStore.MountName} store data");
                UpdateMountInStore(MountToStore);
            }


            EmoMountMod.MountManager.DestroyMount(Character, MountToStore);

            if (MountToStore == ActiveMount)
            {
                EmoMountMod.Log.LogMessage($"Stored Mount is ActiveMount");
                SetActiveMount(null);
            }
        }

        private void AddMountToStore(BasicMountController basicMountController)
        {
            StoredMounts.Add(basicMountController.MountUID, basicMountController.CreateInstanceData());
            EmoMountMod.MountManager.RemoveMountControllerForCharacter(Character);
        }

        private void UpdateMountInStore(BasicMountController basicMountController)
        {
            StoredMounts[basicMountController.MountUID] = basicMountController.CreateInstanceData();

            EmoMountMod.MountManager.RemoveMountControllerForCharacter(Character);
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
                BasicMountController basicMountController =  EmoMountMod.MountManager.CreateMountFromInstanceData(Character, mountInstanceData, Character.transform.position, Character.transform.eulerAngles);
                StoredMounts.Remove(MountUID);
                return basicMountController;
            }

            return null;
        }

        public void EnableActiveMount()
        {
            if (HasActiveMount)
            {
                if (!this.ActiveMount.isActiveAndEnabled)
                {
                    this.ActiveMount.gameObject.SetActive(true);
                }

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
            if (HasStoredMount(MountUID))
            {
                return StoredMounts[MountUID];
            }

            return null;
        }

        public bool HasStoredMount(string MountUID)
        {
            return StoredMounts.ContainsKey(MountUID);
        }
    }
}
