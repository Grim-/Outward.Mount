using System.Collections.Generic;

namespace EmoMount.Patches
{
    [System.Serializable]
    public class DropOnCharacterDeath
    {
        public string CharacterUID;
        public List<DropItemInfo> ItemDrops;

        public DropOnCharacterDeath()
        {

        }

        public DropOnCharacterDeath(string characterUID, List<DropItemInfo> itemDrops)
        {
            CharacterUID = characterUID;
            ItemDrops = itemDrops;
        }
    }

    [System.Serializable]
    public class DropItemInfo
    {
        public int ItemID;
        public int Quantity;
    }
}
