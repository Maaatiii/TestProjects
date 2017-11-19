using System;

namespace DynamicProxyTest
{
    public interface IFoo
    {
        void Test();
    }

    public class Foo : IFoo
    {
        private readonly int initValue;

        public Foo(int initValue)
        {
            this.initValue = initValue;
        }

        public void Test()
        {
            Console.WriteLine($"Hello from foo {initValue}");
        }
    }
}
