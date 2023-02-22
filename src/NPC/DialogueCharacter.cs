using SideLoader;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EmoMount
{
    public class DialogueCharacter
    {
        public string UID { get; set; }
        public string Name { get; set; }

        public string SpawnSceneBuildName { get; set; }
        public Vector3 SpawnPosition { get; set; }
        public Vector3 SpawnRotation { get; set; }
        public Character.SpellCastType StartingPose { get; set; }

        delegate void OnCharacterDestroy(string message);

        public SL_Character.VisualData CharVisualData { get; set; } = new()
        {
            Gender = Character.Gender.Male,
            SkinIndex = 0,
            HairColorIndex = 0,
            HairStyleIndex = 0,
            HeadVariationIndex = 0,
        };

        public int HelmetID = -1;
        public int ChestID = -1;
        public int BootsID = -1;
        public int WeaponID = -1;
        public int OffhandID = -1;
        public int BackpackID = -1;

        public virtual SL_Character CreateAndApplyTemplate(Action<SL_Character, Character, string> OnSpawnDelegate)
        {
            SL_Character template = new()
            {
                UID = UID,
                Name = Name,
                Faction = Character.Factions.NONE,
                StartingPose = StartingPose,
                SaveType = CharSaveType.Scene,
                SceneToSpawn = SpawnSceneBuildName,
                SpawnPosition = SpawnPosition,
                SpawnRotation = SpawnRotation,
                CharacterVisualsData = CharVisualData,
                Helmet_ID = HelmetID,
                Chest_ID = ChestID,
                Boots_ID = BootsID,
                Weapon_ID = WeaponID,
                Shield_ID = OffhandID,
                Backpack_ID = BackpackID,
            };

            template.ApplyTemplate();


            template.OnSpawn += (Character c, string s) =>
            {
                OnSpawnDelegate?.Invoke(template, c, s);
            };
            
            return template;
        }
    }
}
