using System;
using System.Runtime.CompilerServices;

// [MethodImpl(MethodImplOptions.NoInlining)]


for (int i = 0; i < 100; i++)
{
    // Insert method call here (do not remove Thread.Sleep)
    DoWork(new ClassA());
    DoWork(new ClassA());
    DoWork(new ClassB());

    Thread.Sleep(10);
}

[MethodImpl(MethodImplOptions.NoInlining)]
static int DoWork(ITest d)
{
    return d.Test();
}

interface ITest
{
    public int Test();
}

class ClassA : ITest
{
    public int Test() => 8;
}

class ClassB : ITest
{
    public int Test() => 9;
}
