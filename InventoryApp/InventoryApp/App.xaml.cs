using Prism;
using Prism.Ioc;
using InventoryApp.ViewModels;
using InventoryApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InventoryApp
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
            //await NavigationService.NavigateAsync("MainPage/NavigationPage/InventoriesPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
            containerRegistry.RegisterForNavigation<InventoriesPage, InventoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<InventoryCollectionPage, InventoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanPage, ScanPageViewModel>();
            containerRegistry.RegisterForNavigation<InventoryPage, InventoryPageViewModel>();
            containerRegistry.RegisterForNavigation<LookupPickerPage, LookupPickerPageViewModel>();
            containerRegistry.RegisterForNavigation<ViabilityTestsPage, ViabilityTestsPageViewModel>();
            containerRegistry.RegisterForNavigation<ViabilityTestPage, ViabilityTestPageViewModel>();
            containerRegistry.RegisterForNavigation<ViabilityTestDataPage, ViabilityTestDataPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangeLocationPage, ChangeLocationPageViewModel>();
            containerRegistry.RegisterForNavigation<MovementPage, MovementPageViewModel>();
            containerRegistry.RegisterForNavigation<NewInventoryPage, NewInventoryPageViewModel>();
            containerRegistry.RegisterForNavigation<PrintingPage, PrintingPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanReaderPage, ScanReaderPageViewModel>();
            containerRegistry.RegisterForNavigation<SearchAccessionPage, SearchAccessionPageViewModel>();
            containerRegistry.RegisterForNavigation<SearchInventoriesPage, SearchInventoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<PreviousInventoriesPage, PreviousInventoriesPageViewModel>();
        }
    }
}
