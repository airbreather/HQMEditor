using System.IO;

namespace HQMFileConverter
{
    public interface IQuestLineReader
    {
        QuestLine ReadQuestLine(Stream inputStream);
    }
}
