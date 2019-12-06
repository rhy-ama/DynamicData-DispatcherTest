using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherTests.Models
{
    public class TestLayout : ReactiveObject, IKeyable<int>
    {
        private int _id;
        private int _position;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public int Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        public int getKey() => _id;

    }
}
