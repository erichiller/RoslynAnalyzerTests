using System;
using Libs;

namespace MainLibs;

/// <summary></summary>
[JetBrains.Annotations.PublicAPI]
public sealed class MockRegistryConsumerMainLibs1 : IDisposable {
    private readonly ServiceRegistryEntryDummy _entryDummy;
    private readonly ADisposable               _aDisposable;

    /// <summary></summary>
    public MockRegistryConsumerMainLibs1(MockServiceRegistry registry) {
        ArgumentNullException.ThrowIfNull(registry);
        _entryDummy = registry.RegisterService<SomeServiceDummy>(ServiceStatusDummy.None);
        Console.WriteLine(_entryDummy.StatusDummy);
        _aDisposable = _entryDummy.ReturnADisposable();
    }

    /// <summary></summary>
    public void DoSomething() {
        Console.WriteLine(_entryDummy.StatusDummy);
    }

    /// <inheritdoc />
    public void Dispose() { }
}