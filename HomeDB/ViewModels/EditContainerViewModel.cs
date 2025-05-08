using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Models;
using HomeDB.Data;
using System.Collections.ObjectModel;

namespace HomeDB.ViewModels
{
    [QueryProperty("SelectedContainer", "SelectedContainer")]
    [QueryProperty("Node", "Node")]
    [QueryProperty("Nodes", "Nodes")]
    public partial class EditContainerViewModel : ObservableObject
    {
        [ObservableProperty]
        public Container selectedContainer;

        [ObservableProperty]
        public TreeNode node;

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        DatabaseContext _context = new();

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

        public void Refresh(TreeNode node)
        {
            if (Node.Parent != null)
            {
                var parent = Node.Parent;
                parent.Children.Remove(Node);
                parent.Children.Add(node);
            }
            else
            {
                Nodes.Remove(Node);
                Nodes.Add(node);
            }
        }

        [RelayCommand]
        async Task Save()
        {
            await _context.UpdateContainer(SelectedContainer);
            var newNode = new TreeNode
            {
                Id = Node.Id,
                Type = Node.Type,
                Name = SelectedContainer.Name,
                Icon = SelectedContainer.Icon,
                IsLeaf = Node.IsLeaf,
                Parent = Node.Parent, 
            };
            Refresh(newNode);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task EditChild(TreeNode node)
        {
            if (node.Children.Count == 0)
                await LoadChildrenAsync(node);

            if (node.Type == ChildType.Item)
            {
                var item = await _context.GetItem(node.Id);
                await Shell.Current.GoToAsync($"{nameof(EditItemPage)}", new Dictionary<string, object>
                {
                    ["Item"] = item,
                    ["Node"] = node
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
    }
}
