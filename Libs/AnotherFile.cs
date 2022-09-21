using System;
using System.Linq;
using NodaTime;
using NodaTime.Extensions;

namespace Libs; 

/// <summary></summary>
public sealed class MockRegistryConsumerLibs : IDisposable {
    private readonly ServiceRegistryEntryDummy _entryDummy;
    private readonly ADisposable               _aDisposable;

    /// <summary></summary>
    public MockRegistryConsumerLibs(MockServiceRegistry registry) {
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

public sealed class ADisposable : IDisposable {
    /// <inheritdoc />
    public void Dispose() { }
}

public sealed class ServiceRegistryEntryDummy : IDisposable {
    private ADisposable? _aDisposable;

    public ServiceRegistryEntryDummy() {
        ServiceId      = "x";
        Enabled        = false;
        DesignatedHost = "X";
        HostName       = "x";
        LastUpdate     = DateTime.UtcNow.ToInstant();
        Status         = ServiceStatusDummy.None;
    }

    public ServiceStatusDummy StatusDummy { get; } = ServiceStatusDummy.None;

    public ADisposable ReturnADisposable() {
        // return new ADisposable(); // YES ERROR
        _aDisposable = new ADisposable();
        return _aDisposable; // NO ERROR
    }

    public string             ServiceId      { get; set; }
    public bool               Enabled        { get; set; }
    public string             DesignatedHost { get; set; }
    public string             HostName       { get; set; }
    public Instant             LastUpdate     { get; set; }
    public ServiceStatusDummy Status         { get; set; }

    /// <inheritdoc />
    public void Dispose() {
        _aDisposable?.Dispose();
    }
}

public interface IRegisteredServiceDummy {
    public void SomeMethod();
}

public enum ServiceStatusDummy {
    None
}

public sealed class MockServiceRegistry : System.IDisposable {
    private          ServiceRegistryEntryDummy?                                 _foo;
    private readonly System.Collections.Generic.List<ServiceRegistryEntryDummy> _serviceRegistryEntries = new System.Collections.Generic.List<ServiceRegistryEntryDummy>();

    public ServiceRegistryEntryDummy RegisterService<TRegisteredService>(ServiceStatusDummy statusDummy) where TRegisteredService : IRegisteredServiceDummy {
        // this._foo = 
        //     new ServiceRegistryEntry(
        //     TRegisteredService.ServiceId,
        //     lastUpdate: this._clock.GetCurrentInstant(),
        //     enabled: false, // if the service entry wasn't found, the default is that the service is NOT enabled.
        //     hostName: _hostName,
        //     status: status
        // );
        _foo = this.addServiceAsLocallyRegisteredIfNotPresent(
            this.findRegisteredServiceEntry<TRegisteredService>(statusDummy) is { } existingEntry
                ? this.registerExistingService(existingEntry)
                : this.registerNewService()
        );
        // return _serviceRegistryEntries.Single();
        return _foo;
    }

    private ServiceRegistryEntryDummy registerExistingService(ServiceRegistryEntryDummy serviceRegistryEntryDummy) {
        _foo = serviceRegistryEntryDummy;
        _serviceRegistryEntries.Add(serviceRegistryEntryDummy);
        return serviceRegistryEntryDummy;
    }

    private ServiceRegistryEntryDummy registerNewService() {
        // return new ServiceRegistryEntryDummy();
        // this._serviceRegistryEntries.Add(new ServiceRegistryEntry());
        // return this._serviceRegistryEntries.Last();
        this._foo = new ServiceRegistryEntryDummy();
        return _foo;
    }

    private ServiceRegistryEntryDummy addServiceAsLocallyRegisteredIfNotPresent(ServiceRegistryEntryDummy serviceRegistryEntryDummy) =>
        serviceRegistryEntryDummy;

    private ServiceRegistryEntryDummy findRegisteredServiceEntry<TRegisteredService>(ServiceStatusDummy statusDummy) =>
        _serviceRegistryEntries.Single(s => s.StatusDummy == statusDummy && s is TRegisteredService);


    /// <inheritdoc />
    public void Dispose() {
        _foo?.Dispose();
    }
}

public class SomeServiceDummy : IRegisteredServiceDummy {
    public void SomeMethod() { }
}
