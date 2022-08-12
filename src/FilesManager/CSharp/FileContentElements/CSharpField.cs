using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp.FileContentElements
{
    internal class CSharpField : IField
    {
        public string Name { get; init; }
        public string Description { get; init; }

        public string TypeName { get; init; }

        public CSharpField(string typeName, string name, string description)
        {
            TypeName = typeName;
            Name = name;
            Description = description;
        }
    }
}
