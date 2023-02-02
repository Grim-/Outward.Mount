using SideLoader;
using System;
using System.Collections.Generic;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_AddDrop : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_AddDrop);
        public Type GameModel => typeof(AddDrop);

        public string DropTableUID;
    
        public override void ApplyToComponent<T>(T component)
        {
            AddDrop comp = component as AddDrop;

            comp.DropTableUID = DropTableUID;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class AddDrop : Effect
    {
        public string DropTableUID;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (_affectedCharacter)
            {
                SL_DropTable dropTable = new SL_DropTable()
                {
                    GuaranteedDrops = new List<SL_ItemDrop>()
                {
                        new SL_ItemDrop()
                        {
                            DroppedItemID = 4100320,
                            MinQty = 1,
                            MaxQty = 1
                        }
                },
                    UID = "SOMEUID"
                };
                dropTable.ApplyTemplate();
                EmoMountMod.Log.LogMessage($"Adding {dropTable.UID} to {_affectedCharacter.UID} ({_affectedCharacter.Name})");

                _affectedCharacter.Inventory.MakeLootable(false, true, true, false);

                dropTable.AddAsDropableToGameObject(_affectedCharacter.Inventory.Pouch.gameObject, false, "OJWOJDWOJWO");
                dropTable.GenerateDrops(_affectedCharacter.Inventory.Pouch.transform);
            }
        }
    }
}
