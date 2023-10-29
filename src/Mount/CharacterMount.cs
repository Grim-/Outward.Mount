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
        public bool IsTransformed { get; set; }
        public bool IsMounted { get; private set; }
        public bool ActiveMountDisabled { get; private set; }
        public Character Character => GetComponent<Character>();
        #endregion

        public Action<Item> OnItemPicked;


        private AnimatorOverrideController AC;
        private AnimatorOverrideController LastAC;
        private RuntimeAnimatorController OriginalAC;


        public void Start()
        {
            SetIsMounted(false);


        }

        private void OverrideAC()
        {
            // OriginalAC = Character.Animator.runtimeAnimatorController;


            // AC = new AnimatorOverrideController(OriginalAC);


            // AnimationClip SittingMounted = OutwardHelpers.GetFromAssetBundle<AnimationClip>("mount", "emomountbundle", "SittingMounted");

            // if (SittingMounted == null)
            // {
            //     EmoMountMod.Log.LogMessage("Cant find animation clip");
            // }
            // else
            // {
            //     EmoMountMod.Log.LogMessage("Found clip");
            // }


            //string SitAnimation =  CustomAnimations.ClipEnumToName(SideLoader.Animation.PlayerAnimationClip.HumanSitGroundIdle1_a);

            // foreach (var item in AC.animationClips)
            // {
            //     if (item.name == SitAnimation)
            //     {
            //         AC.SetClip(item, SittingMounted, true);
            //         OutwardHelpers.CopyAnimationEvents(item, SittingMounted);
            //     }
            // }
        }

        public void SetActiveMount(BasicMountController newMount)
        {
            ActiveMount = newMount;
        }

        public void SetIsMounted(bool isMounted)
        {
            IsMounted = isMounted;
        }


        public void OnMounted(BasicMountController BasicMountController)
        {
            SetIsMounted(true);
        }

        public void OnUnMounted(BasicMountController BasicMountController)
        {
            SetIsMounted(false); 
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
            if (!StoredMounts.ContainsKey(basicMountController.MountUID))
            {
                StoredMounts.Add(basicMountController.MountUID, basicMountController.CreateInstanceData());
                EmoMountMod.MountManager.RemoveMountControllerForCharacter(Character);
            }
        }

        private void UpdateMountInStore(BasicMountController basicMountController)
        {
            if (StoredMounts.ContainsKey(basicMountController.MountUID))
            {
                StoredMounts[basicMountController.MountUID] = basicMountController.CreateInstanceData();
                EmoMountMod.MountManager.RemoveMountControllerForCharacter(Character);
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
                BasicMountController basicMountController =  EmoMountMod.MountManager.CreateMountFromInstanceData(this, mountInstanceData, Character.transform.position, Character.transform.eulerAngles);
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
