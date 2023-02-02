using System.Xml.Serialization;

namespace EmoMount
{
    public class MountCompProp
    {
        [XmlAttribute("CompName")]
        public string CompName { get; set; }
    }
}

