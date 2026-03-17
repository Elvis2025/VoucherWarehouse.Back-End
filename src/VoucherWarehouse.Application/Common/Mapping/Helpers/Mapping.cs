using IBS.VoucherWarehouse.Common.Mapping.Base;

namespace IBS.VoucherWarehouse.Common.Mapping.Helpers;

public class Mapping<TEntity1, TEntity2> : IbsTwoWayMapperBase<TEntity1, TEntity2> where TEntity1 : new() where TEntity2 : new()
{
    private static readonly Lazy<Mapping<TEntity1, TEntity2>> map =
     new Lazy<Mapping<TEntity1, TEntity2>>(() => new Mapping<TEntity1, TEntity2>());
    public static Mapping<TEntity1, TEntity2> Auto => map.Value;
    private Mapping()
    {
    }

    protected override TEntity2 CreateDestination(TEntity1 source)
    => new TEntity2();
    protected override TEntity1 CreateSource(TEntity2 destination)
    => new();

    protected override void MapCore(TEntity1 source, TEntity2 destination)
    {

    }

    protected override void ReverseMapCore(TEntity2 destination, TEntity1 source)
    {

    }
}
