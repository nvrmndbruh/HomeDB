using CommunityToolkit.Mvvm.ComponentModel;
using HomeDB.Data;
using HomeDB.Models;
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

        DatabaseContext _context = new();

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

            var items = await _context.GetItems();
            var containers = await _context.GetContainers();
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
                var startPlace = await _context.GetItemContainerByItem(item.Id);

                if (startPlace == null)
                {
                    SearchResults.Add(new SearchResult { Name = item.Name, Path = item.Name });
                    break;
                }


                var parent = await _context.GetContainer(startPlace.ContainerId);
                path = parent.Name + "/" + path;

                while (parent != null)
                {
                    var childHierarchy = await _context.GetChildrenHierarchy(parent.Id);
                    if (childHierarchy == null)
                        break;
                    parent = await _context.GetContainer(childHierarchy.ParentId);
                    path = parent.Name + "/" + path;
                }
                path += item.Name;
                SearchResults.Add(new SearchResult { Name = item.Name, Path = path});
            }
        }
    }
}
