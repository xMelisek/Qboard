namespace Qboard.Systems;

public static class ConfigSerializer
{
    public static (Block, string) ReadBlock(string data)
    {
        string line = data.Substring(0, data.IndexOf("\n"));
        data = data.Remove(0, data.IndexOf("\n") + 1);
        string header = line.Trim(':');
        (string[], string) blockData = ReadSubBlock(data);
        return (new Block(header, blockData.Item1), blockData.Item2);
    }

    public static (string[], string) ReadSubBlock(string block)
    {
        List<string> data = new List<string>();
        while(block[0] == ' ')
        {
            string line = block.IndexOf("\n") == -1 ? block : block.Substring(0, block.IndexOf("\n"));
            block = block.Remove(0, block.IndexOf("\n") == -1? block.Length : block.IndexOf("\n") + 1);
            line = line.Trim([' ', '\"']);
            data.Add(line);
            if (block == "") break;
        }
        return (data.ToArray(), block);
    }
}

public struct Config
{
    public Block header;
    public Block[] blocks;
}

public struct Block
{
    public string header;
    public string[] data;

    public Block(string header, string[] data)
    {
        this.header = header;
        this.data = data;
    }
}