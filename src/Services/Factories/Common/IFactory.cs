namespace AD.Services.Factories
{
    internal interface IFactory<TItem>
    {
        TItem Create<T>();
    }
}
