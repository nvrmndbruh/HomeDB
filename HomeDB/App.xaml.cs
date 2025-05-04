using HomeDB.Models;

namespace HomeDB
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();
            DatabaseInitializer.Initialize();
            MainPage = mainPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}