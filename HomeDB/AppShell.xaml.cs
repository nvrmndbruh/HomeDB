namespace HomeDB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditItemPage), typeof(EditItemPage));
            Routing.RegisterRoute(nameof(EditContainerPage), typeof(EditContainerPage));
            Routing.RegisterRoute(nameof(EditCategoryPage), typeof(EditCategoryPage));
            Routing.RegisterRoute(nameof(ItemListPage), typeof(ItemListPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        }
    }
}
