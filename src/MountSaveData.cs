using SideLoader.SaveData;
using UnityEngine;

namespace EmoMount
{
    public class MountSaveData : PlayerSaveExtension
    {
        // Declare as many members as you want, but they must be XML-serializable.
        // They must also be public, and not static.


        public string MountName;
        public string MountUID;

        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;

        public string BagID;

        public float CurrentFood;
        public float MaximumFood;

        public float MountSpeed;
        public float RotateSpeed;


        public Vector3 Position;
        public Vector3 Rotation;

        // Serialize your class from the character / world here.
        // The instance members will contain the last loaded save data, if any was found.
        public override void Save(Character character, bool isWorldHost)
        {
            CharacterMount characterMount = character.gameObject.GetComponent<CharacterMount>();
        
            if (characterMount != null && characterMount.Mount != null)
            {
                EmoMountMod.Log.LogMessage("Saving Mount Data");
                this.MountName = characterMount.Mount.MountName;
                this.MountUID = characterMount.Mount.MountUID;

                this.SLPackName = characterMount.Mount.SLPackName;
                this.AssetBundleName = characterMount.Mount.AssetBundleName;
                this.PrefabName = characterMount.Mount.PrefabName;
                this.BagID = characterMount.Mount.BagContainer.ItemIDString;
                this.CurrentFood = characterMount.Mount.MountFood.CurrentFood;
                this.MaximumFood = characterMount.Mount.MountFood.MaximumFood;
                this.Position = characterMount.Mount.transform.position;
                this.Rotation = characterMount.Mount.transform.eulerAngles;

                this.MountSpeed = characterMount.Mount.MoveSpeed;
                this.RotateSpeed = characterMount.Mount.RotateSpeed;
            }
        }

        // Your class is loaded from an XML save, apply it to the character / world.
        // The instance members contain the last loaded save data.
        public override void ApplyLoadedSave(Character character, bool isWorldHost)
        {
            //needs redoing
            CharacterMount characterMount = character.gameObject.GetComponent<CharacterMount>();

            if (!string.IsNullOrEmpty(MountName))
            {
                EmoMountMod.Log.LogMessage("Creating Mount From Save Data");
                BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountForCharacter(character, MountName, SLPackName, AssetBundleName, PrefabName, BagID, Position, Rotation, MountSpeed, RotateSpeed);
                basicMountController.MountUID = this.MountUID;
                basicMountController.MountFood.CurrentFood = this.CurrentFood;
                basicMountController.MountFood.MaximumFood = this.MaximumFood;

                characterMount.SetMount(basicMountController);
            }

        }
    }
}
