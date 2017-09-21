namespace Mineman.WorldParsing.MapTools
{
    public interface IMapRendererFactory
    {
        IMapRenderer2D Create2DRender(IWorldParser parser, IMapColumnCacheProvider cacheProvider);
    }
}