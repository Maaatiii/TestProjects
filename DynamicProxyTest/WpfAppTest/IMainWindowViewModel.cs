using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfAppTest
{
    public interface IAutoNotifyPropertyChanged : INotifyPropertyChanged
    {
        void NotifyPropertyChanged([CallerMemberName] string propertyName = "");
    }

    public interface IMainWindowViewModel 
    {
        //string TestValue { get; set; }
        string TestValue2 { get; set; }
    }
}