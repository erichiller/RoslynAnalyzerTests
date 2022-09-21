// See https://aka.ms/new-console-template for more information

using System;
using Libs;
using MainLibs;

namespace RoslynAnalyzerTests ;

public static class Program {

    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        var x = new MockRegistryConsumerMainLibs1(new MockServiceRegistry());
        x.DoSomething();
        var mrcl = new MockRegistryConsumerLibs(new MockServiceRegistry());
        x.DoSomething();
        var mrcml = new MockRegistryConsumerMainLibs2(new MockServiceRegistry());
        x.DoSomething();
    }
}


public sealed class FooResource : IDisposable {
    public void Dispose() { }
}

public sealed class FooOwner : IDisposable {
    private readonly FooResource  _fooResource = new FooResource();
    private          FooResource? _fooResource2;

    public void Dispose() {
        this._fooResource.Dispose();
        this._fooResource2?.Dispose();
    }

    public FooResource FooResource                       => _fooResource;
    public FooResource CreateNewFooResource              => new FooResource();
    public FooResource ReturnExistingFooResource()       => _fooResource;
    public FooResource CreateFooResourceAndSaveToField() => _fooResource2 ??= new FooResource();

    public bool TryGetNewFooResource(out FooResource fooResource) {
        fooResource = this._fooResource;
        return true;
    }
}

public sealed class FooReceiver : IDisposable {
    private readonly FooResource _fooResource;

    public FooReceiver(FooResource fooResource) {
        this._fooResource = fooResource;
    }

    public void Dispose() { }
}

public sealed class FooReceiver2 : IDisposable {
    private readonly FooResource _fooResource;

    public FooReceiver2(FooOwner fooOwner) {
        ArgumentNullException.ThrowIfNull(fooOwner);
        this._fooResource = fooOwner.FooResource;
    }

    public void Dispose() { }
}

public sealed class FooReceiver3 : IDisposable {
    private readonly FooResource _fooResource;

    public FooReceiver3(FooOwner fooOwner) {
        ArgumentNullException.ThrowIfNull(fooOwner);
        this._fooResource = fooOwner.CreateNewFooResource;
    }

    public void Dispose() { }
}

public sealed class FooReceiver4 : IDisposable {
    /* 
     * TESTING
     * https://github.com/dotnet/roslyn-analyzers/blob/main/docs/Analyzer%20Configuration.md#configure-dispose-ownership-transfer-for-disposable-objects-passed-as-arguments-to-method-calls
     */
    private readonly FooResource _fooResource; // NOTE: this is only an error when not set to ContextSensitive! (Both NonContextSensitive and None set this)

    public FooReceiver4(FooOwner fooOwner) {
        ArgumentNullException.ThrowIfNull(fooOwner);
        this._fooResource = fooOwner.ReturnExistingFooResource();
    }

    public void Dispose() { }
}

public sealed class FooReceiver5 : IDisposable {
    private readonly FooResource _fooResource;

    public FooReceiver5(FooOwner fooOwner) {
        ArgumentNullException.ThrowIfNull(fooOwner);
        fooOwner.TryGetNewFooResource(out this._fooResource);
    }

    public void Dispose() { }
}

public sealed class FooReceiver6 : IDisposable {
    private readonly FooResource _fooResource;

    public FooReceiver6(FooOwner fooOwner) {
        ArgumentNullException.ThrowIfNull(fooOwner);
        this._fooResource = fooOwner.CreateFooResourceAndSaveToField();
    }

    public void Dispose() { }
}


/*
 * URGENT: it says analyzers are installed! even without the package.
 * 1. ... BUT I *DID* have it installed, is it somehow cached? TRY A NEW PROJECT
 * 2. See if I disable the .editorconfig, does it still say "Roslyn Analyzers are present in this solution?"
 * 3. Try Analysis Level
 */