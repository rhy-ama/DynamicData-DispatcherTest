using DispatcherTests.Models;
using DispatcherTests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.ViewModels
{
    public class LayoutsDataSource : GenericDataStore<TestLayout>
    {
        public LayoutsDataSource(IStorageService storageService) : base(storageService)
        {

        }

        private int GetLayoutCount()
        {
            lock (_collectionLock)
            {
                if (_internalCache.Items.Any())
                    return _internalCache.Items.Max(i => i.Position);
                else
                    return 0;
            }
        }
        public TestLayout InsertNew(int id)
        {
            lock (_collectionLock)
            {
                TestLayout newItem = new TestLayout() {Id = id, Position= GetLayoutCount() + 1 };

                _internalCache.Edit(f =>
                {
                    f.AddOrUpdate(newItem);
                });

                return newItem;
            }
        }
    }
}
