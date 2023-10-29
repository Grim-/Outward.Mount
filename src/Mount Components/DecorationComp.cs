using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount.Mount_Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using UnityEngine;

    namespace EmoMount.Mount_Components
    {
        public class DecorationComp : MountComp
        {
            public int ItemID;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;

            private GameObject DecorationInstance = null;

            public override void OnApply(BasicMountController BasicMountController)
            {
                base.OnApply(BasicMountController);

                if (BasicMountController.IsTransform)
                {
                    return;
                }

                if (DecorationInstance == null)
                {
                    Item Item = ResourcesPrefabManager.Instance.GetItemPrefab(ItemID);

                    if (Item)
                    {
                        GameObject ItemVisualGO = Item.GetItemVisual().gameObject;

                        if (ItemVisualGO)
                        {
                            DecorationInstance = GameObject.Instantiate(ItemVisualGO, Controller.MountPoint);
                            DecorationInstance.transform.localPosition = Position;
                            DecorationInstance.transform.localEulerAngles = Rotation;
                            DecorationInstance.transform.localScale = Scale != Vector3.zero ? Scale : Vector3.one;
                        }
  
                    }
                    else EmoMountMod.LogMessage($"DecorationComp : An Item cannot be found with ItemID {ItemID}");
                        
                }
            }

            public override void OnRemove(BasicMountController Controller)
            {
                base.OnRemove(Controller);

                if (DecorationInstance)
                {
                    GameObject.Destroy(DecorationInstance);
                    DecorationInstance = null;
                }
            }
        }

        [XmlType("DecorationCompProp")]
        public class DecorationComppProp : MountCompProp
        {
            [XmlElement("ItemID")]
            public int ItemID;

            [XmlElement("Position")]
            public Vector3 Position;

            [XmlElement("Rotation")]
            public Vector3 Rotation;

            [XmlElement("Scale")]
            public Vector3 Scale;
        }
    }

}
