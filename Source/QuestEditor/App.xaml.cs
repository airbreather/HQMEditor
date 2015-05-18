using System.Windows;

using GalaSoft.MvvmLight.Threading;

namespace QuestEditor
{
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
