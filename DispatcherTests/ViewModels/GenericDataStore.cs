using DispatcherTests.Models;
using DispatcherTests.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.ViewModels
{
    public class GenericDataStore<T> : ReactiveObject, IDisposable
        where T : class, IKeyable<int>, new()
    {
        protected readonly object _collectionLock = new object();
        protected IStorageService _storageService;

        //flags
        protected bool _isInitialized = false;
        protected bool _isDisposed = false;

        //datastore
        protected readonly SourceCache<T, int> _internalCache = new SourceCache<T, int>(i => i.getKey());

        //GUI databinding
        public ObservableCollectionExtended<T> BindingObject { get; } = new ObservableCollectionExtended<T>();

        //Databinding
        public IObservableCollection<T> GetDataBinding() => BindingObject;
        IDisposable _dataBindingDisposable;

        public IObservableCache<T, int> ObservableObject { get => _internalCache.AsObservableCache(); }

        //initialization monitoring
        private Subject<bool> _initializationMonitor = new Subject<bool>();
        public IObservable<bool> isInitialized;
        SingleAssignmentDisposable _initDisposable;

        protected void SignalInitialized(bool value)
        {
            _isInitialized = value;
            _initializationMonitor.OnNext(value);
        }

        public GenericDataStore(IStorageService storageService)
        {
            isInitialized = _initializationMonitor.AsObservable().Publish().RefCount();
            _storageService = storageService;

            _dataBindingDisposable = _internalCache
                                        .Connect()
                                        .Synchronize()
                                        .ObserveOn(Scheduler.Default)
                                        .Bind(BindingObject)
                                        //.DisposeMany() not needed in this example
                                        .Subscribe();

            Load();
        }

        protected virtual void Load()
        {
            lock (_collectionLock)
            {
                _initDisposable = new SingleAssignmentDisposable();
                _internalCache.Edit(f =>
                {
                    if (_internalCache.Count > 0)
                        f.Clear();

                    _initDisposable.Disposable = _storageService.Load<T>()
                                            .ObserveOn(Scheduler.Default)
                                            .Subscribe(i =>
                                            {
                                                f.AddOrUpdate(i);
                                            },
                                            err =>
                                            {
                                                SignalInitialized(false);
                                            },
                                            () =>
                                            {
                                                SignalInitialized(true);
                                            });
                });
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //free managed resources
                    _dataBindingDisposable?.Dispose();
                    _dataBindingDisposable = null;

                    _initDisposable?.Dispose();
                    _initDisposable = null;

                    _initializationMonitor?.OnCompleted();
                    _initializationMonitor?.Dispose();
                    _initializationMonitor = null;
                }
                _isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
