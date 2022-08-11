namespace AD.Aids.Common
{
    internal interface IFactory<TItem>
    {
        TItem Create<T>();
    }
}
