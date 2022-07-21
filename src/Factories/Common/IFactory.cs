namespace AD.Factories.Common
{
    internal interface IFactory<TItem>
    {
        TItem Create<T>();
    }
}
