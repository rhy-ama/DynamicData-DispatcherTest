using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePropertiesTests
{
    public class TestViewModel : ReactiveObject
    {
        private string _input1;
        private string _input2;
        private bool _switchInputs = false;
        
        private readonly ObservableAsPropertyHelper<bool> _isInput1Visible;
        private readonly ObservableAsPropertyHelper<bool> _isInput2Visible;


        public bool SwitchInputs
        {
            get => _switchInputs;
            set => this.RaiseAndSetIfChanged(ref _switchInputs, value);
        }

        public string Input1Text
        {
            get => _input1;
            set => this.RaiseAndSetIfChanged(ref _input1, value);
        }

        public string Input2Text
        {
            get => _input2;
            set => this.RaiseAndSetIfChanged(ref _input2, value);
        }

        public bool IsInput1Visible => _isInput1Visible.Value;
        public bool IsInput2Visible => _isInput2Visible.Value;

        public TestViewModel()
        {
            _isInput1Visible = this
                .WhenAnyValue(x => x.SwitchInputs)
                .Select(v => v)
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.IsInput1Visible);

            _isInput2Visible = this
                .WhenAnyValue(x => x.SwitchInputs)
                .Select(v => !v)
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.IsInput2Visible);
        }
    }

}
