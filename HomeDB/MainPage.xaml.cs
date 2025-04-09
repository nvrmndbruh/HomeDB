using HomeDB.Data;

namespace HomeDB
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _database;

        public MainPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
        }

        //public MainPage()
        //{
        //    InitializeComponent();
        //}

        //private async void OnCounterClicked(object sender, EventArgs e)
        //{
            
        //}
    }
}
