using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SocketsPrototype.Services;
using SocketsPrototype.Views;

namespace SocketsPrototype
{
    public partial class App : Application
    {

        public static bool UseMockDataStore = true;

        public App()
        {
            InitializeComponent();
            DependencyService.Register<MainPage>();
            DependencyService.Register<SocketService>();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();

            MainPage = DependencyService.Get<MainPage>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
