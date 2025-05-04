using HomeDB.Models;
using HomeDB.ViewModel;

namespace HomeDB
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(new HomeDbContext());
        }
    }
}
