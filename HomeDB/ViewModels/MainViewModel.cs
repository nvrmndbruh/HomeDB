using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HomeDB.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public DatabaseContext _context = new();

        public ObservableCollection<TreeNode> Nodes { get; set; } = new();

        //public ICommand LoadChildrenCommand { get; }

        public MainViewModel()
        {
            Init();
        }

        public async Task Init()
        {
            //if (!File.Exists(DatabaseContext.DbPath))
            //{
                using var stream = FileSystem.OpenAppPackageFileAsync("HomeDB.db").GetAwaiter().GetResult();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    File.WriteAllBytes(DatabaseContext.DbPath, memoryStream.ToArray());
                }
            //}

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
        async Task Edit(TreeNode node)
        {
            if (node.Children.Count == 0)
                await LoadChildren(node);

            if (node.Type == nameof(Item))
            {
                var item = await _context.GetItem(node.Id);
                await Shell.Current.GoToAsync($"{nameof(EditItemPage)}", new Dictionary<string, object>
                {
                    ["Item"] = item,
                    ["Node"] = node,
                    ["Nodes"] = Nodes
                });
            }
            else
            {
                var container = await _context.GetContainer(node.Id);
                await Shell.Current.GoToAsync($"{nameof(EditContainerPage)}", new Dictionary<string, object>
                {
                    ["SelectedContainer"] = container,
                    ["Node"] = node,
                    ["Nodes"] = Nodes
                });
            }
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