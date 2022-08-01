using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp
{
    public sealed class CSharpClass : IClass
    {
        public string Description { get; init; }
        public string Name { get; init; }

        public IMethod[] Methods { get; init; }
        public IField[] Fields { get; init; }

        public CSharpClass(string description, string name, IMethod[] methods, IField[] fields)
        {
            Description = description;
            Name = name;
            Methods = methods;
            Fields = fields;
        }
    }
}