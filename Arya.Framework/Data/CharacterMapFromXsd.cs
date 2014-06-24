using System.Collections.Generic;

namespace Natalie.Framework.Data
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class CharacterMapInformation
    {
        [System.Xml.Serialization.XmlElementAttribute("Character", IsNullable = false)]
        public List<Character> Characters { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Character
    {
        [System.Xml.Serialization.XmlElementAttribute()]
        public string Block { get; set; }

        private string _blockLower;
        public string BlockLower
        {
            get { return _blockLower ?? (_blockLower = Block.ToLower()); }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public string Description { get; set; }

        private string _descriptionLower;
        public string DescriptionLower
        {
            get { return _descriptionLower ?? (_descriptionLower = Description.ToLower()); }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public string Symbol { get; set; }
    }
}