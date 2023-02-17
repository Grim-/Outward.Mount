namespace EmoMount.Patches
{
    [System.Serializable]
    public class DropOnCharacterDeath
    {
        public string CharacterUID;
        public int ItemID;
        public int Quantity;

        public DropOnCharacterDeath()
        {
        }

        public DropOnCharacterDeath(string characterUID, int itemID, int quantity)
        {
            CharacterUID = characterUID;
            ItemID = itemID;
            Quantity = quantity;
        }
    }
}
