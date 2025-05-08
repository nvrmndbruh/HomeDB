namespace HomeDB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditItemPage), typeof(EditItemPage));
            Routing.RegisterRoute(nameof(EditContainerPage), typeof(EditContainerPage));
        }
    }
}
