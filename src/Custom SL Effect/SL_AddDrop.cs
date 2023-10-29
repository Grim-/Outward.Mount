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
            AddDrop comp = effect as AddDrop;
            this.DropTableUID = comp.DropTableUID;
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
                    UID = DropTableUID
                };
                dropTable.ApplyTemplate();
                EmoMountMod.LogMessage($"Adding {dropTable.UID} to {_affectedCharacter.UID} ({_affectedCharacter.Name})");

                _affectedCharacter.Inventory.MakeLootable(false, true, true, false);

                dropTable.AddAsDropableToGameObject(_affectedCharacter.Inventory.Pouch.gameObject, false, Guid.NewGuid().ToString());
                dropTable.GenerateDrops(_affectedCharacter.Inventory.Pouch.transform);
            }
        }
    }
    //public class SL_RestoreEquippedItemDurability : SL_Effect, ICustomModel
    //{
    //    public Type SLTemplateModel => typeof(SL_RestoreEquippedItemDurability);
    //    public Type GameModel => typeof(RestoreEquippedItemDurability);

    //    public override void ApplyToComponent<T>(T component)
    //    {
    //        RestoreEquippedItemDurability comp = component as RestoreEquippedItemDurability;
    //        comp.EquipmentSlot = this.EquipmentSlot;
    //        comp.Durability = this.Durability;
    //        comp.Percentage = this.Percentage;
    //    }

    //    public override void SerializeEffect<T>(T effect)
    //    {
    //        RestoreEquippedItemDurability comp = effect as RestoreEquippedItemDurability;
    //        comp.EquipmentSlot = this.EquipmentSlot;
    //        comp.Durability = this.Durability;
    //        comp.Percentage = this.Percentage;
    //    }
    //    public EquipmentSlot.EquipmentSlotIDs EquipmentSlot;
    //    public float Durability;
    //    public bool Percentage;
    //}

    //public class RestoreEquippedItemDurability : Effect
    //{
    //    public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
    //    {
    //        // if no affected character, do nothing and return (exit).
    //        if (_affectedCharacter == null) { return; }


    //        if (_affectedCharacter.Inventory == null)
    //        {
    //            //no inventory
    //            return;
    //        }

    //        if (_affectedCharacter.Inventory.Equipment == null)
    //        {
    //            //no equipment?
    //            return;
    //        }

    //        if (!_affectedCharacter.Inventory.Equipment.IsEquipmentSlotActive(EquipmentSlot))
    //        {
    //            //slot isnt active, whatever that means
    //            return;
    //        }


    //        EquipmentSlot ChosenSlot = _affectedCharacter.Inventory.Equipment.GetMatchingSlot(EquipmentSlot);

    //        if (ChosenSlot == null)
    //        {
    //            //Log chosen slot is null
    //            return;
    //        }


    //        if (!ChosenSlot.HasItemEquipped)
    //        {
    //            //log no item equipped in slot
    //            return;
    //        }

    //        //


    //        // Gets equipped item in EquipmentSlot
    //        Item equippedItem = ChosenSlot.EquippedItem;
    //        RepairEquipment(equippedItem, Durability);
    //    }
    //    public EquipmentSlot.EquipmentSlotIDs EquipmentSlot;
    //    public float Durability;
    //    public bool Percentage;

    //    private void RepairEquipment(Item ItemToRepair, float RepairAmount)
    //    {
    //        if (ItemToRepair is not Equipment)
    //        {
    //            //log Item isnt equipment
    //            return;
    //        }

    //        float FinalRepairValue = 0;

    //        //if its an absolute value
    //        if (!this.Percentage)
    //        {
    //            FinalRepairValue = RepairAmount;
    //        }
    //        //restore a percent of max instead
    //        else
    //        {
    //            FinalRepairValue = this.Durability / 100f * (float)ItemToRepair.MaxDurability;
    //        }

    //        ItemToRepair.RepairAmount(FinalRepairValue);
    //    }
    //}


    
}