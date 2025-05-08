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

        public ICommand LoadChildrenCommand { get; }

        public MainViewModel()
        {
            _context = new DatabaseContext();
            Init();
            LoadChildrenCommand = new AsyncRelayCommand<TreeNode>(async (node) => { await LoadChildrenAsync(node); });
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
            var hierarchies = await _context.GetHierarchies();

            var root = containers
                .Where(c => !hierarchies.Any(h => h.ChildId == c.Id && h.ChildType == ChildType.Container))
                .ToList();

            foreach (var container in root)
            {
                var node = new TreeNode
                {
                    Id = container.Id,
                    Type = ChildType.Container,
                    Name = container.Name,
                    Icon = container.Icon,
                    IsLeaf = false,
                    Parent = null
                };

                Nodes.Add(node);
            }
        }

        private async Task LoadChildrenAsync(TreeNode parent)
        {
            var containers = await _context.GetContainers();
            var hierarchies = await _context.GetHierarchies();
            var items = await _context.GetItems();

            foreach (var hierarchy in hierarchies.Where(h => h.ParentId == parent.Id).ToList())
            {
                if (hierarchy.ChildType == ChildType.Container)
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
                            Type = ChildType.Container,
                            IsLeaf = !hasChildren,
                            Parent = parent
                        });
                    }
                }
                else if (hierarchy.ChildType == ChildType.Item)
                {
                    var item = items.FirstOrDefault(i => i.Id == hierarchy.ChildId);
                    if (item != null)
                    {
                        parent.Children.Add(new TreeNode
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Icon = item.Icon,
                            Type = ChildType.Item,
                            IsLeaf = true,
                            Parent = parent
                        });
                    }
                }
            }

        }


        [RelayCommand]
        async Task Edit(TreeNode node)
        {
            if (node.Type == ChildType.Item)
            {
                var item = await _context.GetItem(node.Id);
                await Shell.Current.GoToAsync($"{nameof(EditItemPage)}", new Dictionary<string, object>
                {
                    ["Item"] = item,
                    ["Node"] = node
                });
            }
        }
    }
}