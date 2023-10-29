using SideLoader.SaveData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmoMount
{
    public class MountSaveData : PlayerSaveExtension
    {
        //Current Active Mount For Player
        public MountInstanceData ActiveMountInstance = new MountInstanceData();

        //Any stored mounts
        public List<MountInstanceData> StoredMounts = new List<MountInstanceData>();


        public override void Save(Character character, bool isWorldHost)
        {
            CharacterMount characterMount = character.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.ActiveMount != null)
            {
                SaveActiveMount(character, characterMount);
            }
            else
            {
                this.ActiveMountInstance = null;
            }

            if (characterMount != null && characterMount.StoredMounts.Count > 0)
            {
                SaveStoredMounts(character, characterMount);
            }

        }

        public override void ApplyLoadedSave(Character character, bool isWorldHost)
        {
            CharacterMount characterMount = character.gameObject.GetComponent<CharacterMount>();

            if (this.ActiveMountInstance != null)
            {
                LoadActiveMount(character, characterMount);
            }
            

            if (StoredMounts.Count > 0)
            {
                LoadStoredMounts(character, characterMount);
            }
        }


        private void SaveActiveMount(Character character, CharacterMount characterMount)
        {
            if (characterMount.HasActiveMount && !characterMount.ActiveMount.IsTransform)
            {
                EmoMountMod.LogMessage("Saving Active Mount Data");
                this.ActiveMountInstance = characterMount.ActiveMount.CreateInstanceData();
            }
            else
            {
                this.ActiveMountInstance = null;
            }
        }



        public void SaveStoredMounts(Character character, CharacterMount characterMount)
        {
            EmoMountMod.LogMessage($"Saving stored mounts [{StoredMounts.Count}]");
            StoredMounts.Clear();
            StoredMounts = characterMount.StoredMounts.Values.ToList();
        }

        private void LoadStoredMounts(Character character, CharacterMount characterMount)
        {
            EmoMountMod.LogMessage($"Loading stored mounts [{StoredMounts.Count}]");
            characterMount.StoredMounts.Clear();
            foreach (var storedMount in StoredMounts)
            {
               // EmoMountMod.Log.LogMessage($"Loading {storedMount.MountName} Mount Data");
                characterMount.StoredMounts.Add(storedMount.MountUID, storedMount);
            }

        }

        private void LoadActiveMount(Character character, CharacterMount characterMount)
        {
            if (!string.IsNullOrEmpty(this.ActiveMountInstance.MountUID))
            {
                OutwardHelpers.DelayDo(() =>
                {
                    BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountFromInstanceData(characterMount, this.ActiveMountInstance, OutwardHelpers.GetRandomPositionAroundCharacter(character), characterMount.transform.eulerAngles);
                }, 10f);
            }      
        }
    }
}
