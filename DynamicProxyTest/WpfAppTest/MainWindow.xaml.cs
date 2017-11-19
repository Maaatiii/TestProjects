using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext =
                 new ProxyGenerator()
                 .CreateClassProxy(typeof(MainWindowViewModel), new[] {typeof(INotifyPropertyChanged) },
                 new NotifierInterceptor()); 
            //new NotifyPropertyChangedInterceptor());


        }
    }

    public class NotifierInterceptor : IInterceptor
    {
        private PropertyChangedEventHandler handler;
        public static Dictionary<String, PropertyChangedEventArgs> _cache =
          new Dictionary<string, PropertyChangedEventArgs>();

        public void Intercept(IInvocation invocation)
        {
            //Each subscription to the PropertyChangedEventHandler is intercepted (add)
            if (invocation.Method.Name == "add_PropertyChanged")
            {
                handler = (PropertyChangedEventHandler)
                      Delegate.Combine(handler, (Delegate)invocation.Arguments[0]);
                invocation.ReturnValue = handler;
            }
            //Each de-subscription to the PropertyChangedEventHandler is intercepted (remove)
            else if (invocation.Method.Name == "remove_PropertyChanged")
            {
                handler = (PropertyChangedEventHandler)
                   Delegate.Remove(handler, (Delegate)invocation.Arguments[0]);
                invocation.ReturnValue = handler;
            }
            //Each setter raise a PropertyChanged event
            else if (invocation.Method.Name.StartsWith("set_"))
            {
                //Do the setter execution
                invocation.Proceed();
                //Launch the event after the execution
                if (handler != null)
                {
                    PropertyChangedEventArgs arg =
                      retrievePropertyChangedArg(invocation.Method.Name);
                    handler(invocation.Proxy, arg);
                }
            }
            else invocation.Proceed();
        }

        // Caches the PropertyChangedEventArgs
        private PropertyChangedEventArgs retrievePropertyChangedArg(String methodName)
        {
            PropertyChangedEventArgs arg = null;
            NotifierInterceptor._cache.TryGetValue(methodName, out arg);
            if (arg == null)
            {
                arg = new PropertyChangedEventArgs(methodName.Substring(4));
                NotifierInterceptor._cache.Add(methodName, arg);
            }
            return arg;
        }
    }

    public class NotifyPropertyChangedInterceptor : IInterceptor
    {
        private const string SET_PREFIX = "set_";

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var methodInfo = invocation.Method;

            if (ShouldSupressNotify(methodInfo))
            {
                return;
            }

            var notify = (IAutoNotifyPropertyChanged)invocation.Proxy;
            notify.NotifyPropertyChanged(invocation.Method.Name.Substring(SET_PREFIX.Length));
        }

        private bool ShouldSupressNotify(MethodInfo methodInfo)
        {
            if (!methodInfo.IsSpecialName || !methodInfo.Name.StartsWith(SET_PREFIX))
            {
                return true;
            }

            return false;
        }
    }

    public class MainWindowViewModel : IMainWindowViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public MainWindowViewModel()
        {
            timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TestValue = DateTime.Now.ToLongTimeString();
            TestValue2 = DateTime.Now.ToLongTimeString();
        }

        //public string testValue;

        //public string TestValue
        //{
        //    get { return testValue; }

        //    set { testValue = value; NotifyPropertyChanged(); }
        //}

        public virtual string TestValue2 { get; set; }

        private Timer timer;
    }
}
