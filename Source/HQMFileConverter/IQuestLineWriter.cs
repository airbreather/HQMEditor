using System.IO;

namespace HQMFileConverter
{
    public interface IQuestLineWriter
    {
        void WriteQuestLine(QuestLine questLine, Stream outputStream);
    }
}
