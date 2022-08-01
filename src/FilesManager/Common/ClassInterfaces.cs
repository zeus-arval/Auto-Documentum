namespace AD.FilesManager.Common
{
    public interface IClass
    {
        public string Description { get; }
        public string Name { get; }

        public IMethod[] Methods { get; }
        public IField[] Fields { get; }
    }

    public interface IField
    {
        public IClass Class { get; }
        public string Name { get; }
        public string Description { get; }
    }

    public interface IMethod { }
}
