using CommunityToolkit.Mvvm.ComponentModel;
using HomeDB.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HomeDB.ViewModels
{
    [QueryProperty("Nodes", "Nodes")]
    public partial class SearchViewModel : ObservableObject
    {
        public class SearchResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        [ObservableProperty]
        public string searchText;

        [ObservableProperty]
        public ObservableCollection<SearchResult> searchResults;

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        private CancellationTokenSource _searchCancellationTokenSource;

        public SearchViewModel()
        {
            SearchResults = new ObservableCollection<SearchResult>();
            PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchText))
            {
                _searchCancellationTokenSource?.Cancel();
                _searchCancellationTokenSource = new CancellationTokenSource();
                var token = _searchCancellationTokenSource.Token;

                try
                {
                    await Task.Delay(300, token); // Задержка 300 мс
                    await Search(token);
                }
                catch (TaskCanceledException) { }
            }
        }

        async Task Search(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            var items = await DatabaseContext.Items.GetAllAsync();
            var containers = await DatabaseContext.Containers.GetAllAsync();
            var search = items.Where(i => !string.IsNullOrEmpty(i.Name) && i.Name.StartsWith(SearchText, StringComparison.OrdinalIgnoreCase))?.ToList();
            
            if (search == null || search.Count == 0 || string.IsNullOrEmpty(SearchText))
            {
                SearchResults.Clear();
                return;
            }
                

            SearchResults.Clear();
            foreach (var item in search)
            {
                string path = "";
                var startPlace = await DatabaseContext.ItemContainers.GetByChild(item.Id);

                if (startPlace == null)
                {
                    SearchResults.Add(new SearchResult { Name = item.Name, Path = item.Name });
                    break;
                }


                var parent = await DatabaseContext.Containers.GetAsync(startPlace.ContainerId);
                path = parent.Name + "/" + path;

                while (parent != null)
                {
                    var childHierarchy = await DatabaseContext.Hierarchies.GetByChild(parent.Id);
                    if (childHierarchy == null)
                        break;
                    parent = await DatabaseContext.Containers.GetAsync(childHierarchy.ParentId);
                    path = parent.Name + "/" + path;
                }
                path += item.Name;
                SearchResults.Add(new SearchResult { Name = item.Name, Path = path});
            }
        }
    }
}
