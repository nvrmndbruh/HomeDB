using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;

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

        public MainViewModel()
        {
            Nodes = new ObservableCollection<TreeNode>();
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
            var itemContainers = await _context.GetItemContainers();

            var root = containers
                .Where(c => !hierarchies.Any(h => h.ChildId == c.Id))
                .ToList();

            foreach (var container in root)
            {
                var hasChildren = hierarchies.Any(h => h.ParentId == container.Id)
                    || itemContainers.Any(ic => ic.ContainerId == container.Id);
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
                .Where(i => !itemContainers.Any(h => h.ItemId == i.Id))
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
            var itemContainers = await _context.GetItemContainers();

            foreach (var hierarchy in hierarchies.Where(h => h.ParentId == parent.Id).ToList())
            {
                var container = containers.FirstOrDefault(c => c.Id == hierarchy.ChildId);
                if (container != null)
                {
                    var hasChildren = hierarchies.Any(h => h.ParentId == container.Id)
                        || itemContainers.Any(ic => ic.ContainerId == container.Id); ;
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
            foreach (var itemContainer in itemContainers.Where(ic => ic.ContainerId == parent.Id))
            {
                var item = await _context.GetItem(itemContainer.ItemId);
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
            if (parent.Children.Count == 0)
            {
                parent.IsLeaf = true;
            }
        }

        [RelayCommand]
        async Task Search()
        {
            await Shell.Current.GoToAsync(nameof(SearchPage));
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
            if (SelectedNode.Type == nameof(Container))
            {
                var hierarchy = await _context.GetChildrenHierarchy(SelectedNode.Id);
                if (hierarchy != null)
                    await _context.DeleteHierarchy(hierarchy.Id);
            }
            else
            {
                var itemContainer = await _context.GetItemContainerByItem(SelectedNode.Id);
                if (itemContainer != null)
                    await _context.DeleteItemContainer(itemContainer.Id);
            }


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
            if (SelectedNode != null)
            {
                if (SelectedNode.Type != nameof(Item))
                {
                    MovingNode.Parent = SelectedNode;
                    if (MovingNode.Type == nameof(Container))
                    {
                        var hierarchy = new ContainerHierarchy
                        {
                            ParentId = SelectedNode.Id,
                            ChildId = MovingNode.Id,
                        };
                        await _context.InsertHierarchy(hierarchy);
                    }
                    else
                    {
                        var itemContainer = new ItemContainer
                        {
                            ContainerId = SelectedNode.Id,
                            ItemId = MovingNode.Id,
                        };
                        await _context.InsertItemContainer(itemContainer);
                    }
                    SelectedNode.Children.Clear();
                    SelectedNode.IsLeaf = false;
                    await LoadChildren(SelectedNode);
                    MovingNode = null;
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Ошибка", "Вы не можете поместить в вещь другую вещь", "ОК");
                }
            }
            else
            {
                Nodes.Add(MovingNode);
                MovingNode = null;
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
                var placeholder = new Item
                {
                    Name = "Новая вещь",
                    Description = "Создано автоматически",
                    Icon = "it_abstract.png",
                    Photo = null,
                    Price = null
                };

                await _context.InsertItem(placeholder);

                if (SelectedNode == null)
                    Nodes.Add(new TreeNode
                    {
                        Id = placeholder.Id,
                        Name = placeholder.Name,
                        Icon = placeholder.Icon,
                        Type = nameof(Item),
                        Parent = null,
                        IsLeaf = true
                    });

                else
                {
                    if (SelectedNode.Type == nameof(Item))
                    {
                        App.Current.MainPage.DisplayAlert("Ошибка", "Вы не можете поместить в вещь другую вещь", "ОК");
                        return;
                    }

                    SelectedNode.IsLeaf = false;
                    await _context.InsertItemContainer(new ItemContainer
                    {
                        ContainerId = SelectedNode.Id,
                        ItemId = placeholder.Id,
                    });
                    SelectedNode.Children.Clear();
                    await LoadChildren(SelectedNode);
                }
            }
            else if (select == "Контейнер")
            {
                var placeholder = new Container
                {
                    Name = "Новая вещь",
                    Icon = "ct_abstract.png",
                };

                await _context.InsertContainer(placeholder);

                if (SelectedNode == null)
                    Nodes.Add(new TreeNode
                    {
                        Id = placeholder.Id,
                        Name = placeholder.Name,
                        Icon = placeholder.Icon,
                        Type = nameof(Container),
                        Parent = null,
                        IsLeaf = true
                    });
                else
                {
                    if (SelectedNode.Type == nameof(Item))
                    {
                        App.Current.MainPage.DisplayAlert("Ошибка", "Вы не можете поместить в вещь другую вещь", "ОК");
                        return;
                    }
                    SelectedNode.IsLeaf = false;
                    await _context.InsertHierarchy(new ContainerHierarchy
                    {
                        ParentId = SelectedNode.Id,
                        ChildId = placeholder.Id,
                    });
                    SelectedNode.Children.Clear();
                    await LoadChildren(SelectedNode);
                }
            }

        }
    }
}