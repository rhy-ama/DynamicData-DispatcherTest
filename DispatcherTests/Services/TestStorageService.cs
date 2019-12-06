using DispatcherTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.Services
{
    class TestStorageService : IStorageService
    {
        public IObservable<T> Load<T>() where T: class, new()
        {
            if (typeof(TestRow) == typeof(T))
            {
                return Observable.Create<T>(async (obs, cancel) =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    obs.OnNext(new TestRow() { Id = 0, Name = "Line_1" } as T);

                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    obs.OnNext(new TestRow() { Id = 1, Name = "Line_2" } as T);

                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    obs.OnNext(new TestRow() { Id = 2, Name = "Line_3" } as T);

                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    obs.OnNext(new TestRow() { Id = 3, Name = "Line_4" } as T);

                    await Task.Delay(TimeSpan.FromMilliseconds(2000));
                    obs.OnCompleted();
                });
            }
            else if (typeof(TestLayout) == typeof(T))
            {

                return Observable.Create<T>(async (obs, cancel) =>
                {
                    //Subscriber will have enough time to run
                    await Task.Delay(TimeSpan.FromMilliseconds(5000));
                    obs.OnCompleted();
                    //return Disposable.Empty;
                });
            }
            else
                throw new System.NotImplementedException();
        }

        public bool Save<T>(IEnumerable<T> update)
        {
            throw new NotImplementedException();
        }
    }
}
