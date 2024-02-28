using System.Reactive.Disposables;

namespace RealTimeApp;

public static class Extensions
{
    public static T AddToDisposables<T>(this T disposable, CompositeDisposable coll)
        where T: IDisposable
    {
        coll.Add(disposable);
        return disposable;
    }
}