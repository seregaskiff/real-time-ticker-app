using System.Reactive.Disposables;

namespace RealTimeApp;

public static class Extensions
{
    public static IDisposable AddToDisposables(this IDisposable disposable, CompositeDisposable coll)
    {
        coll.Add(disposable);
        return disposable;
    }
}