using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using SQLitePCL;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HomeDB.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public DatabaseContext _context = new();

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        [ObservableProperty]
        public TreeNode selectedNode;

        [ObservableProperty]
        public TreeNode movingNode;

        [ObservableProperty]
        public string searchText;

        [ObservableProperty]
        public ObservableCollection<string> searchResults;

        public MainViewModel()
        {
            Nodes = new ObservableCollection<TreeNode>();
            SearchResults = new();
            Init();
        }

        public async Task Init()
        {
            if (!File.Exists(DatabaseContext.DbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("HomeDB.db").GetAwaiter().GetResult();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    File.WriteAllBytes(DatabaseContext.DbPath, memoryStream.ToArray());
                }
            }

            await LoadRoot();
        }

        private async Task LoadRoot()
        {
            var containers = await _context.GetContainers();
            var items = await _context.GetItems();
            var hierarchies = await _context.GetHierarchies();

            var root = containers
                .Where(c => !hierarchies.Any(h => h.ChildId == c.Id && h.ChildType == nameof(Container)))
                .ToList();

            foreach (var container in root)
            {
                var hasChildren = hierarchies.Any(h => h.ParentId == container.Id);
                var node = new TreeNode
                {
                    Id = container.Id,
                    Type = nameof(Container),
                    Name = container.Name,
                    Icon = container.Icon,
                    IsLeaf = !hasChildren,
                    Parent = null
                };
                Nodes.Add(node);
            }

            var standalone = items
                .Where(i => !hierarchies.Any(h => h.ChildId == i.Id && h.ChildType == nameof(Item)))
                .ToList();

            foreach (var item in standalone)
            {
                var node = new TreeNode
                {
                    Id = item.Id,
                    Type = nameof(Item),
                    Name = item.Name,
                    Icon = item.Icon,
                    IsLeaf = true,
                    Parent = null
                };
                Nodes.Add(node);
            }
        }

        [RelayCommand]
        private async Task LoadChildren(TreeNode parent)
        {
            var containers = await _context.GetContainers();
            var hierarchies = await _context.GetHierarchies();
            var items = await _context.GetItems();

            foreach (var hierarchy in hierarchies.Where(h => h.ParentId == parent.Id).ToList())
            {
                if (hierarchy.ChildType == nameof(Container))
                {
                    var container = containers.FirstOrDefault(c => c.Id == hierarchy.ChildId);
                    if (container != null)
                    {
                        var hasChildren = hierarchies.Any(h => h.ParentId == container.Id);
                        parent.Children.Add(new TreeNode
                        {
                            Id = container.Id,
                            Name = container.Name,
                            Icon = container.Icon,
                            Type = nameof(Container),
                            IsLeaf = !hasChildren,
                            Parent = parent
                        });
                    }
                }
                else if (hierarchy.ChildType == nameof(Item))
                {
                    var item = items.FirstOrDefault(i => i.Id == hierarchy.ChildId);
                    if (item != null)
                    {
                        parent.Children.Add(new TreeNode
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Icon = item.Icon,
                            Type = nameof(Item),
                            IsLeaf = true,
                            Parent = parent
                        });
                    }
                }
            }
            if (parent.Children.Count == 0)
            {
                parent.IsLeaf = true;
            }
        }

        [RelayCommand]
        async Task Search()
        {
            var items = await _context.GetItems();
            var containers = await _context.GetContainers();

            var search = items.Where(i => i.Name == string.Concat(SearchText)).ToList();

            if (search == null)
                App.Current.MainPage.DisplayAlert("Ошибка", "Вещь с таким именем не найдена", "ОК");
            
            foreach(var item in search)
            {
                string path = "";
                var childHierarchy = await _context.GetChildrenHierarchy(item.Id, nameof(Item));

                if (childHierarchy  == null)
                {
                    SearchResults.Add(item.Name);
                    break;
                }

                var parent = await _context.GetContainer(childHierarchy.ParentId);
                path = parent.Name + "/" + path;

                while (parent != null)
                {
                    childHierarchy = await _context.GetChildrenHierarchy(parent.Id, nameof(Container));
                    if (childHierarchy == null)
                        break;
                    parent = await _context.GetContainer(childHierarchy.ParentId);
                    path = parent.Name + "/" + path;
                }
                SearchResults.Add(path);
            }

            if (SearchResults.Count == 0)
                App.Current.MainPage.DisplayAlert("Ошибка", "Вещь с таким именем не найдена", "ОК");
        }

        [RelayCommand]
        async Task Edit()
        {
            if (SelectedNode.Children.Count == 0)
                await LoadChildren(SelectedNode);

            if (SelectedNode.Type == nameof(Item))
            {
                var item = await _context.GetItem(SelectedNode.Id);
                await Shell.Current.GoToAsync($"{nameof(EditItemPage)}", new Dictionary<string, object>
                {
                    ["Item"] = item,
                    ["Node"] = SelectedNode,
                    ["Nodes"] = Nodes
                });
            }
            else
            {
                var container = await _context.GetContainer(SelectedNode.Id);
                await Shell.Current.GoToAsync($"{nameof(EditContainerPage)}", new Dictionary<string, object>
                {
                    ["SelectedContainer"] = container,
                    ["Node"] = SelectedNode,
                    ["Nodes"] = Nodes
                });
            }
            SelectedNode = null;
        }

        [RelayCommand]
        async Task PickUp()
        {
            var hierarchy = await _context.GetChildrenHierarchy(SelectedNode.Id, SelectedNode.Type);
            if (hierarchy != null)
                await _context.DeleteHierarchy(hierarchy.Id);

            if (SelectedNode.Parent != null)
            {
                SelectedNode.Parent.Children.Remove(SelectedNode);
                if (SelectedNode.Parent.Children.Count() == 0)
                {
                    SelectedNode.Parent.IsLeaf = true;
                }
                SelectedNode.Parent = null;
            }
            else
            {
                Nodes.Remove(SelectedNode);
            }
            MovingNode = SelectedNode;
            SelectedNode = null;
        }

        [RelayCommand]
        async Task DropDown()
        {
            if (SelectedNode !=  null)
            {
                if (SelectedNode.Type != nameof(Item))
                {
                    MovingNode.Parent = SelectedNode;
                    var hierarchy = new Hierarchy
                    {
                        ParentId = SelectedNode.Id,
                        ChildId = MovingNode.Id,
                        ChildType = MovingNode.Type
                    };
                    await _context.InsertHierarchy(hierarchy);
                    SelectedNode.Children.Clear();
                    SelectedNode.IsLeaf = false;
                    await LoadChildren(SelectedNode);
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Ошибка", "Вы не можете поместить в вещь другую вещь", "ОК");
                }
            }
            else
            {
                Nodes.Add(MovingNode);
            }

            MovingNode = null;
        }

        [RelayCommand]
        async Task Add()
        {
            var select = await Application.Current.MainPage.DisplayActionSheet(
                "Выберите элемент",
                "Отмена",
                null,
                "Вещь",
                "Контейнер");

            if (select == "Вещь")
            {
                var item = new Item
                {
                    Name = "Новая вещь",
                    Description = "Создано автоматически",
                    Icon = "it_abstract.png",
                    Photo = null,
                    Price = null
                };

                await _context.InsertItem(item);

                Nodes.Add(new TreeNode
                {
                    Id = item.Id,
                    Name = item.Name,
                    Icon = item.Icon,
                    Type = nameof(Item),
                    Parent = null,
                    IsLeaf = true
                });
            }
            else if (select == "Контейнер")
            {
                var container = new Container
                {
                    Name = "Новая вещь",
                    Icon = "ct_abstract.png",
                };

                await _context.InsertContainer(container);

                Nodes.Add(new TreeNode
                {
                    Id = container.Id,
                    Name = container.Name,
                    Icon = container.Icon,
                    Type = nameof(Container),
                    Parent = null,
                    IsLeaf = true
                });
            }
        }
    }
}