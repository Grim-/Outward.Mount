using SideLoader.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmoMount
{
    public class MountSaveData : PlayerSaveExtension
    {
        //Current Active Mount For Player
        public MountInstanceData ActiveMountInstance = new MountInstanceData();

        //Any stored mounts
        public List<MountInstanceData> StoredMounts = new List<MountInstanceData>();


        // Serialize your class from the character / world here.
        // The instance members will contain the last loaded save data, if any was found.
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

        // Your class is loaded from an XML save, apply it to the character / world.
        // The instance members contain the last loaded save data.
        public override void ApplyLoadedSave(Character character, bool isWorldHost)
        {
            //needs redoing
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
            if (characterMount.HasActiveMount)
            {
                EmoMountMod.Log.LogMessage("Saving Active Mount Data");
                this.ActiveMountInstance = EmoMountMod.MountManager.CreateInstanceDataFromMount(characterMount.ActiveMount);
                EmoMountMod.MountManager.SerializeMountBagContents(this.ActiveMountInstance, characterMount.ActiveMount);
            }
            else
            {
                this.ActiveMountInstance = null;
            }
        }



        public void SaveStoredMounts(Character character, CharacterMount characterMount)
        {
            EmoMountMod.Log.LogMessage("Saving Stored Mount Data");
            StoredMounts.Clear();

            foreach (var storedMount in characterMount.StoredMounts)
            {
                StoredMounts.Add(storedMount);
                EmoMountMod.Log.LogMessage($"Saving {storedMount.MountName}");
            }
            
        }

        private void LoadStoredMounts(Character character, CharacterMount characterMount)
        {
            EmoMountMod.Log.LogMessage($"LoadingStored Mount Data Stored Mounts{StoredMounts.Count}");
            characterMount.StoredMounts.Clear();
            foreach (var storedMount in StoredMounts)
            {
                EmoMountMod.Log.LogMessage($"Loading {storedMount.MountName} Mount Data");
                characterMount.StoredMounts.Add(storedMount);
            }

        }

        private void LoadActiveMount(Character character, CharacterMount characterMount)
        {
            if (!string.IsNullOrEmpty(this.ActiveMountInstance.MountUID))
            {
                EmoMountMod.Log.LogMessage("Creating Mount From Save Data");
                character.StartCoroutine(LateLoading(character, characterMount, this.ActiveMountInstance));
            }
       
        }


        private IEnumerator LateLoading(Character character, CharacterMount characterMount, MountInstanceData mountInstanceData)
        {
            yield return new WaitForSeconds(10f);
            BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountFromInstanceData(character, mountInstanceData);

            if (basicMountController == null)
            {
                EmoMountMod.Log.LogMessage("Late Loading Basic Mount Controller was null");
                yield break;
            }
            EmoMountMod.MountManager.DeSerializeMountBagContents(this.ActiveMountInstance, basicMountController);
            yield break;
        }
    }
}
