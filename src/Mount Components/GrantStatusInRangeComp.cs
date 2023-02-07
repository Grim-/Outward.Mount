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


        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

        }

        public override void Update()
        {
            base.Update();

            if (string.IsNullOrEmpty(StatusName))
            {
                return;
            }

            //Timer += Time.deltaTime;

            //if (Timer >= RefreshTime)
            //{
                if (Controller.DistanceToOwner <= Radius)
                {
                    GrantCharacterStatus(Controller.CharacterOwner);
                }
                else
                {
                    RemoveStatusFromCharacter(Controller.CharacterOwner);
                }

               // Timer = 0;
            //}
        }


        public void GrantCharacterStatus(Character Character)
        {
            if (string.IsNullOrEmpty(StatusName) || Controller.IsTransform)
            {
                return;
            }

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
