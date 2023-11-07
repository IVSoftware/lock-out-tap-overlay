This answer shows how to lock out the [Click Me] button on the .NET Maui Default App until some interval (in this case 2 seconds) from the last click. For the restartable time interval, some kind of watchdog timer is needed. A NuGet for a suitable timer can be added to project using instructions below. The second element of this solution is to have an overlay frame that covers the entire screen. Ordinarily, the `IsVisible` property is `false`, and the overlay doesn't interfere with taps. Tap five times, you get a count of five.

[![Modified .NET MAUI default app][1]][1]

When the overlay is visible, the appearance is set to `BackgroundColor="DarkGray"` and `Opacity="0.25"` to give a "disabled" appearance to the screen when enabled. The `InputTransparent="False"` prevents the [Click Me] button from recognizing the taps, but only when the overly is set to `IsVisible`.

```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="lock_out_tap_overlay.MainPage">

    <Grid>
        <ScrollView>
            <VerticalStackLayout
                Spacing="25"
                Padding="30,0"
                VerticalOptions="Center">

                <Image
                    Source="dotnet_bot.png"
                    SemanticProperties.Description="Cute dot net bot waving hi to you!"
                    HeightRequest="200"
                    HorizontalOptions="Center" />

                <Label
                    Text="Hello, World!"
                    SemanticProperties.HeadingLevel="Level1"
                    FontSize="32"
                    HorizontalOptions="Center" />

                <Label
                    Text="Welcome to .NET Multi-platform App UI"
                    SemanticProperties.HeadingLevel="Level2"
                    SemanticProperties.Description="Welcome to dot net Multi platform App U I"
                    FontSize="18"
                    HorizontalOptions="Center" />

                <Button
                    x:Name="CounterBtn"
                    Text="Click me"
                    SemanticProperties.Hint="Counts the number of times you click"
                    Clicked="OnCounterClicked"
                    HorizontalOptions="Center" />
                <HorizontalStackLayout HorizontalOptions="Center">
                    <CheckBox 
                        x:Name="checkboxIsLockOutMechanismEnabled"
                        IsChecked="False" 
                        Margin="10" 
                        HorizontalOptions="Start"
                        VerticalOptions="Center"/>
                    <Label 
                        Text="Enable Lock Out Mechanism" 
                        VerticalTextAlignment="Center"
                        HorizontalOptions="Start" />
                </HorizontalStackLayout>

            </VerticalStackLayout>
        </ScrollView>
        <~--Overlay-->
        <Frame
            BackgroundColor="DarkGray" 
            Opacity="0.25"
            InputTransparent="False"
            IsVisible="{Binding IsLockedOut}">
            <Frame.GestureRecognizers>
                <TapGestureRecognizer Tapped="OnOverlayTapped"/>
            </Frame.GestureRecognizers>
        </Frame>
    </Grid>

</ContentPage>
```
___

C#

We use `{Binding IsLockedOut}` to hook this property up to the watchdog timer.

```
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
            ExtendLockout();
        }
        count++;
        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
    WatchdogTimer _wdtOverlay = new WatchdogTimer();

    private void ExtendLockout()
    {
        _wdtOverlay.StartOrRestart(
            initialAction: () => IsLockedOut = true,
            completeAction: () => IsLockedOut = false);
    }

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

    private void OnOverlayTapped(object sender, TappedEventArgs e)
    {
        ExtendLockout();
    }
}
```

The result is that the UI will not be responsive for one second from the last time the screen was tapped. To be clear, if you sit and tap the screen every 1/2 second, you will never see a second tap for all eternity.

___

**Adding WatchdogTimer to Project**

There are various ways to make a watchdog timer. Here's the one I tested this answer with.

[![watchdog timer on NuGet][2]][2]


  [1]: https://i.stack.imgur.com/4XzGA.png
  [2]: https://i.stack.imgur.com/k0f0x.png