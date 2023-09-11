namespace GaleForce.GPT.Attributes
{
    using System;
    using System.Linq;
    [AttributeUsage(AttributeTargets.Class)]
    public class AIFunctionAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public AIFunctionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}