using IVSoftware.Portable;

namespace lock_out_tap_overlay
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
            if (checkboxIsLockOutMechanismEnabled.IsChecked)
            {
                _wdtOverlay.StartOrRestart(
                    initialAction: () => IsLockedOut = true,
                    completeAction: () => IsLockedOut = false);
            }
            count++;
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        WatchdogTimer _wdtOverlay = new WatchdogTimer();

        public bool IsLockedOut
        {
            get => _isLockedOut;
            set
            {
                if (!Equals(_isLockedOut, value))
                {
                    _isLockedOut = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isLockedOut = false;
    }
}