using System.IO;
using System.Text;
using System.Xaml;

namespace HQMFileConverter
{
    public sealed class XamlQuestLineWriter : IQuestLineWriter
    {
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);

        public void WriteQuestLine(QuestLine questLine, Stream outputStream)
        {
            XamlQuestLine questLineProxy = new XamlQuestLine(questLine);
            using (StreamWriter streamWriter = new StreamWriter(outputStream, UTF8NoBOM, 4096, true))
            {
                XamlServices.Save(streamWriter, questLineProxy);
            }
        }
    }
}
