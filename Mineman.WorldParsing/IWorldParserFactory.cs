namespace Mineman.WorldParsing
{
    public interface IWorldParserFactory
    {
        IWorldParser Create(string worldPath);
    }
}