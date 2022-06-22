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

        public Character Character => GetComponent<Character>();

        public void SetActiveMount(BasicMountController newMount)
        {
            ActiveMount = newMount;
        }

        public void StoreMount(BasicMountController MountToStore)
        {
            if (!HasStoredMount(MountToStore.MountUID))
            {
                MountInstanceData mountInstanceData = EmoMountMod.MountManager.CreateInstanceDataFromMount(MountToStore);
                StoredMounts.Add(mountInstanceData);
                EmoMountMod.MountManager.DestroyActiveMount(Character);
                SetActiveMount(null);
            }
        }

        public BasicMountController RetrieveStoredMount(string MountUID)
        {
            if (StoredMounts != null && HasStoredMount(MountUID))
            {
                MountInstanceData mountInstanceData = GetStoredMountData(MountUID);
                EmoMountMod.MountManager.CreateMountFromInstanceData(Character, mountInstanceData);
                StoredMounts.Remove(mountInstanceData);
            }

            return null;
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
