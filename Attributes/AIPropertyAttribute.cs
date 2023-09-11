namespace GaleForce.GPT.Attributes
{
    using System;
    using System.Linq;
    [AttributeUsage(AttributeTargets.Property)]
    public class AIPropertyAttribute : Attribute
    {
        public bool IsRequired { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AIPropertyAttribute(bool isRequired = false, string description = null, string name = null)
        {
            IsRequired = isRequired;
            Description = description;
            Name = name;
        }

        public AIPropertyAttribute(string description)
        {
            Description = description;
        }
    }
}