using EmoMount.Mount_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmoMount
{
    public class MountComponentFactory
    {
        private static Dictionary<string, Type> _componentTypes = new Dictionary<string, Type>();

        public static List<Type> AllComponentTypes => _componentTypes.Values.ToList();

        public static void Initialize()
        {
            var assembly = Assembly.Load("EmoMount");
            var componentTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(MountComp)));

            foreach (var type in componentTypes)
            {
                _componentTypes.Add(type.Name, type);
            }
        }

        public static MountComp CreateComponent(BasicMountController MountController, string compName)
        {                  
            EmoMountMod.Log.LogMessage($"Trying to create {compName} Mountcomp");
            if (_componentTypes.ContainsKey(compName))
            {
                return (MountComp)MountController.gameObject.AddComponent(_componentTypes[compName]);
            }
            else
            {
                throw new ArgumentException("Invalid component name");
            }
        }

        /// <summary>
        /// Applies all matching fields and properties from MountCompProp and its derived types to MountComp and its deriving types.
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="prop"></param>
        public static void ApplyMountCompProps(MountComp comp, MountCompProp prop)
        {
            var type = comp.GetType();

            foreach (var p in prop.GetType().GetFields())
            {
                var propValue = p.GetValue(prop);
                if (propValue != null)
                {
                    type.GetField(p.Name)?.SetValue(comp, propValue);
                }
            }

            foreach (var p in prop.GetType().GetProperties())
            {
                var propValue = p.GetValue(prop);
                if (propValue != null)
                {
                    type.GetProperty(p.Name)?.SetValue(comp, propValue);
                }
            }
        }

        ////EmoMount.Mount_Components, EmoMount, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
        //private Type TryGetMountComponent(string componentName)
        //{
        //    var assembly = Assembly.Load("EmoMount");
        //    return assembly.GetType("EmoMount.Mount_Components." + componentName);
        //}
    }
}
