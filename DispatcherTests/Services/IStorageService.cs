using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.Services
{
    public interface IStorageService
    {
        IObservable<T> Load<T>() where T : class, new();
        bool Save<T>(IEnumerable<T> update);
    }
}
