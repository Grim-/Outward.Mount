using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class MountComp : MonoBehaviour
    {
        public BasicMountController Controller
        {
            get; private set;
        }

        public SkinnedMeshRenderer SkinnedMesh
        {
            get
            {
                if (Controller)
                {
                    return Controller.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                return null;
            }
        }

        public void Awake()
        {

        }

        public virtual void Update()
        {

        }


        public virtual void OnApply(BasicMountController BasicMountController)
        {
            Controller = BasicMountController;
        }

        public virtual void OnRemove(BasicMountController Controller)
        {
            Controller = null;
        }

        public virtual bool CanRun(BasicMountController Controller)
        {
            if (Controller.IsTransform)
            {
                return false;
            }

            return true;
        }

        public void OnDestroy()
        {
           // OnRemove(Controller);
        }
    }

    public class MountCompProp
    {
        [XmlAttribute("CompName")]
        public string CompName { get; set; }
    }
}
