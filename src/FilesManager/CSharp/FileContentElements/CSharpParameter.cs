using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp.FileContentElements
{
    public class CSharpParameter : IField
    {
        public string TypeName { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public CSharpParameter(string typeName, string name, string description)
            => (TypeName, Name, Description) = (typeName, name, description);
    }
}
