using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI;

namespace HomeDB.ViewModels
{
    public class MainViewModel : UraniumBindableObject
    {
        public DatabaseContext _context = new();
        public ObservableCollection<TreeNode> Nodes { get; set; } = new();

        public ICommand LoadChildrenCommand { get; set; }
        public ICommand InitCommand { get; set; }

        public MainViewModel()
        {
            _context = new DatabaseContext();
            InitCommand = new AsyncRelayCommand(async () => await Init());
            LoadChildrenCommand = new AsyncRelayCommand<TreeNode>(async (node) =>
            {
                var containers = await _context.GetContainers();
                var hierarchies = await _context.GetHierarchies();
                var items = await _context.GetItems();

                foreach (var hierarchy in hierarchies.Where(h => h.ParentId == node.Id).ToList())
                {
                    if (hierarchy.ChildType == ChildType.Container)
                    {
                        var container = containers.FirstOrDefault(c => c.Id == hierarchy.ChildId);
                        if (container != null)
                        {
                            var hasChildren = hierarchies.Any(h => h.ParentId == container.Id);
                            node.Children.Add(new TreeNode
                            {
                                Id = container.Id,
                                Name = container.Name,
                                Icon = container.Icon,
                                Type = ChildType.Container,
                                IsLeaf = !hasChildren
                            });
                        }
                    }
                    else if (hierarchy.ChildType == ChildType.Item)
                    {
                        var item = items.FirstOrDefault(i => i.Id == hierarchy.ChildId);
                        if (item != null)
                        {
                            node.Children.Add(new TreeNode
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Icon = item.Icon,
                                Type = ChildType.Item,
                                IsLeaf = true
                            });
                        }
                    }
                }
            });
        }

        public async Task Init()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("HomeDB.db").GetAwaiter().GetResult();
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                File.WriteAllBytes(DatabaseContext.DbPath, memoryStream.ToArray());
            }

            await LoadRoot();
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
                            IsLeaf = !hasChildren
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
                            IsLeaf = true
                        });
                    }
                }
            }

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
                //var hasChildren = hierarchies.Any(h => h.ParentId == container.Id);
                var node = new TreeNode
                {
                    Id = container.Id,
                    Type = ChildType.Container,
                    Name = container.Name,
                    Icon = container.Icon,
                    IsLeaf = false
                };

                Nodes.Add(node);
            }
        }
    }
}