using System;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class GrantStatusInRangeComp : MountComp
    {
        public string StatusName;
        public float Radius = 10f;

        public float RefreshTime = 5f;
        private float Timer = 0;

        private string GrantedStatusUID = string.Empty;
        private bool ISActive = true;


        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);


            if (BasicMountController.IsTransform)
            {
                return;
            }


            BasicMountController.EventComp.OnDodgeDown += OnDodge;
        }

        private void OnDodge(BasicMountController _arg1, Character _arg2)
        {
            ISActive = !ISActive;
        }

        public override void Update()
        {
            if (string.IsNullOrEmpty(StatusName) || Controller == null || Controller.CharacterOwner == null || Controller.IsTransform)
            {
                return;
            }

            if (ISActive)
            {
                if (Controller.DistanceToOwner <= Radius)
                {
                    GrantCharacterStatus(Controller.CharacterOwner);
                }
                else
                {
                    RemoveStatusFromCharacter(Controller.CharacterOwner);
                }
            }
        }

        public void GrantCharacterStatus(Character Character)
        {
            if (string.IsNullOrEmpty(StatusName) || Controller.IsTransform)
            {
                return;
            }

           // StatusEffect statusEffectPrefab = ResourcesPrefabManager.Instance.GetStatusEffectPrefab(StatusName);

            if (!Character.StatusEffectMngr.HasStatusEffect(StatusName))
            {
                StatusEffect statusEffect = Character.StatusEffectMngr.AddStatusEffect(StatusName);
                GrantedStatusUID = statusEffect.UID;
            }

        }

        public void RemoveStatusFromCharacter(Character Character)
        {
            if (string.IsNullOrEmpty(StatusName) || string.IsNullOrEmpty(GrantedStatusUID) || Controller.IsTransform)
            {
                return;
            }

            if (Character.StatusEffectMngr.HasStatusEffect(StatusName))
            {
                Character.StatusEffectMngr.RemoveStatus(GrantedStatusUID);
                GrantedStatusUID = string.Empty;
            }
        }

        [XmlType("GrantStatusInRangeCompProp")]
        public class GrantStatusInRangeCompProp : MountCompProp
        {
            [XmlAttribute("Radius")]
            public float Radius;

            [XmlAttribute("StatusName")]
            public string StatusName;
        }
    }

}
