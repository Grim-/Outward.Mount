using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{

    /// <summary>
    /// This simply holds a reference to the characters mount instance, might not need, IDK yet.
    /// </summary>
    public class CharacterMount : MonoBehaviour
    {
        public BasicMountController Mount
        {
            get; private set;
        }

        public bool HasMount
        {
            get
            {
                return Mount != null;
            }
        }

        public Character Character => GetComponent<Character>();

        public void SetMount(BasicMountController newMount)
        {
            Mount = newMount;
        }
    }
}
