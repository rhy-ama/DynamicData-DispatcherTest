using DispatcherTests.Models;
using DispatcherTests.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.ViewModels
{
    public class RowsViewModel : ReactiveObject, IDisposable
    {
        //underlying data models sources
        private GenericDataStore<TestRow> _rows;
        private LayoutsDataSource _layouts;
        private IObservable<IChangeSet<TestLayout, int>> _layoutDataChangeset;

        //gui bound collection
        public IObservableCollection<GUIModel> Rows { get; } = new ObservableCollectionExtended<GUIModel>();
        public IDisposable _bindingDisposable;

        //Datasources initialization 
        protected SingleAssignmentDisposable _datasourcesInitWatchDisposable;

        private bool _isDisposed = false;

        public RowsViewModel(IStorageService storageService)
        {
            //initialize layouts
            _layouts = new LayoutsDataSource(storageService);
            _layoutDataChangeset = _layouts.ObservableObject.Connect();

            //initialize actual rows
            _rows = new GenericDataStore<TestRow>(storageService);
        }

        public void Start()
        {
            //monitor initialization and enable bindings once finished
            _datasourcesInitWatchDisposable = new SingleAssignmentDisposable();
            _datasourcesInitWatchDisposable.Disposable = 
                _rows.isInitialized
                    .CombineLatest(_layouts.isInitialized, (rowsStatus, layoutsStatus) => rowsStatus&& layoutsStatus)
                    .StartWith(false)
                    .DistinctUntilChanged()
                    .ObserveOn(Scheduler.Default) // ---> initialization could take long time so we spin it
                    .Subscribe(
                        i =>
                        {
                            if (i)
                            {
                                EnableBindings();
                                _datasourcesInitWatchDisposable.Dispose();
                            }
                        });
        }

        private void EnableBindings()
        {
            //monitor intervals list, merge it with layout and bind
            _bindingDisposable = _rows.ObservableObject
                        .Connect()
                        .AutoRefreshOnObservable(p => p.WhenAnyPropertyChanged())
                        .LeftJoin(_layoutDataChangeset, l => l.Id,
                            (id, row, layout) =>
                            {
                                if (layout.HasValue)
                                {
                                    return new GUIModel(row, layout.Value);
                                }
                                else
                                {
                                    //no stored layout for this row
                                    return new GUIModel(row, _layouts.InsertNew(row.Id));
                                }
                            })
                        .Sort(SortExpressionComparer<GUIModel>.Ascending(i => i.Layout.Position))
                        //GUIModel is UI bound model so we need to make sure it is processed on GUI thread
                        .ObserveOnDispatcher()
                        .Bind(Rows)
                        //.DisposeMany() //not necessary in this example
                        .Subscribe();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //free managed resources
                    _datasourcesInitWatchDisposable?.Dispose();
                    _datasourcesInitWatchDisposable = null;
                    _bindingDisposable?.Dispose();
                    _bindingDisposable = null;
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
