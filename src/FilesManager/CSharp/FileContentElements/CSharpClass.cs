using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp.FileContentElements
{
    internal sealed class CSharpClass : IClass
    {
        public string NamespaceName { get; init; }
        public string Description { get; init; }
        public string Name { get; init; }

        public IMethod[] Methods { get; init; }
        public IField[] Fields { get; init; }
        public CSharpProperty[] Properties { get; }

        public CSharpClass(string namespaceName, string description, string name, IMethod[] methods, IField[] fields, CSharpProperty[] properties)
        {
            NamespaceName = namespaceName;
            Description = description;
            Name = name;
            Methods = methods;
            Fields = fields;
            Properties = properties;
        }
    }
}