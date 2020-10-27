using System;
using System.Linq;

namespace Arduino.Firmata
{
    public static class ObservableExtensions
    {
        public static void Monitor<T>(this IObservable<T> observable, Action<T> action, Action actionCompleted = null)
        {
            observable.Subscribe(new MyObservable<T>(action, actionCompleted));
        }

        public class MyObservable<T> : IObserver<T>
        {
            private Action<T> action;
            private Action actionCompleted;

            public MyObservable(Action<T> action, Action actionCompleted)
            {
                this.action = action;
                this.actionCompleted = actionCompleted;
            }

            public void OnCompleted()
            {
                actionCompleted?.Invoke(); ;
            }

            public void OnNext(T value)
            {
                action?.Invoke(value);
            }
            public void OnError(Exception error) { }
        }
    }
}