using System.IO;
using System.Text;
using System.Xaml;

namespace HQMFileConverter
{
    public sealed class XamlQuestLineReader : IQuestLineReader
    {
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);

        public QuestLine ReadQuestLine(Stream inputStream)
        {
            XamlQuestLine questLineProxy;
            using (StreamReader streamReader = new StreamReader(inputStream, UTF8NoBOM, false, 4096, true))
            {
                questLineProxy = (XamlQuestLine)XamlServices.Load(streamReader);
            }

            return questLineProxy.ToQuestLine();
        }
    }
}
