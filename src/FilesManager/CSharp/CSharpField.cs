using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp
{
    public class CSharpField : IField
    {
        public string Name { get; init; }
        public string Description { get; init; }

        public IClass Class { get; init; }

        public CSharpField(CSharpClass cSharpClass, string name, string description)
        {
            Class = cSharpClass;
            Name = name;
            Description = description;
        }
    }
}
