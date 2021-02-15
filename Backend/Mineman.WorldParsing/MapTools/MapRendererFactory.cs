using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.MapTools
{
    public class MapRendererFactory : IMapRendererFactory
    {
        private readonly ITextureProvider _textureProvider;

        public MapRendererFactory(ITextureProvider textureProvider)
        {
            _textureProvider = textureProvider;
        }

        public IMapRenderer2D Create2DRender(IWorldParser parser, IMapColumnCacheProvider cacheProvider)
        {
            return new MapRenderer2D(parser, _textureProvider, cacheProvider);
        }
    }
}
