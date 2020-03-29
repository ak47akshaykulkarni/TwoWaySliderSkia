using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TwoWaySliderSkia.Services;
using TwoWaySliderSkia.Views;

namespace TwoWaySliderSkia
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
