using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicProxyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = new Foo(10);

            var proxyGen = new ProxyGenerator();

            var newFoo = proxyGen.CreateInterfaceProxyWithTargetInterface<IFoo>(foo, new Interceptor());

            newFoo.Test();

            Console.ReadKey();
        }
    }

    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("before execution");

            invocation.Proceed();

            Console.WriteLine("after execution");
        }
        
    }
}
