using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.Models
{
    public class GUIModel : ReactiveObject, IKeyable<int>
    {
        private TestRow _row;
        private TestLayout _layout;
        private int _id;

        public TestRow Row
        {
            get => _row;
            set => this.RaiseAndSetIfChanged(ref _row, value);
        }

        public TestLayout Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public GUIModel(TestRow row, TestLayout layout)
        {
            this.Id = row.Id;
            this.Row = row;
            this.Layout = layout;
        }

        public int getKey() => _id;

    }
}
