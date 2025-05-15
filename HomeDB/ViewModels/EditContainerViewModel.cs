using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
            foreach (var itemContainer in itemContainers.Where(ic => ic.ContainerId == parent.Id))
            {
                var item = items.FirstOrDefault(i => i.Id == itemContainer.ItemId);
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

        public async Task Refresh(TreeNode node)
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
                await LoadChildren(node);
            }
        }

        async Task<TreeNode> RefreshChildren(TreeNode parent, TreeNode node, bool childMode = false)
        {
            if (childMode)
            {
                parent.Children.Clear();
                await LoadChildren(parent);
                return parent;
            }

            parent.Children.Remove(node);
            foreach (var children in node.Children)
            {
                children.Parent = parent;
                parent.Children.Add(children);
            }
            if (parent.Children.Count == 0)
                parent.IsLeaf = true;

            return parent;
        }

        private List<string> ValidateModel()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(SelectedContainer.Name))
            {
                errors.Add("Название обязательно");
            }

            return errors;
        }

        [RelayCommand]
        async Task Save()
        {
            var errors = ValidateModel();
            if (errors.Count > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", string.Join("\n", errors), "ОК");
            }
            else
            {
                await _context.UpdateContainer(SelectedContainer);
                var newNode = new TreeNode
                {
                    Id = Node.Id,
                    Type = Node.Type,
                    Name = SelectedContainer.Name,
                    Icon = SelectedContainer.Icon,
                    IsLeaf = Node.IsLeaf,
                    Parent = Node.Parent
                };
                await Refresh(newNode);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task EditChild(TreeNode node)
        {
            if (node.Children.Count == 0)
                await LoadChildren(node);

            if (node.Type == nameof(Item))
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

        [RelayCommand]
        async Task DeleteChild(TreeNode node)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Подтверждение удаления",
                    "Вы уверены, что хотите удалить эту вещь? Это действие нельзя отменить.",
                    "Да",
                    "Нет");

            if (confirm)
            {
                if (node.Type == (nameof(Item)))
                {
                    var item = await _context.GetItem(node.Id);
                    await _context.DeleteItem(item);
                    Node = await RefreshChildren(Node, node, true);
                }
                else
                {
                    var container = await _context.GetContainer(node.Id);

                    var parent = await _context.GetParentHierarchies(node.Id);
                    var child = await _context.GetChildrenHierarchy(node.Id);
                    await _context.UpdateHierarchies(child.ParentId, parent);

                    var childItems = await _context.GetItemContainerByContainer(node.Id);
                    await _context.UpdateItemContainers(Node.Id, childItems);

                    await _context.DeleteContainer(container);
                    Node = await RefreshChildren(Node, node, true);
                }
            }
        }

        [RelayCommand]
        async Task Delete()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Подтверждение удаления",
            "Вы уверены, что хотите удалить этот контейнер? Это действие нельзя отменить.",
            "Да",
            "Нет");

            if (confirm)
            {
                if (Node.Parent != null)
                {
                    var parent = await _context.GetParentHierarchies(SelectedContainer.Id);
                    var child = await _context.GetChildrenHierarchy(SelectedContainer.Id);
                    var childItems = await _context.GetItemContainerByContainer(Node.Id);
                    await _context.UpdateHierarchies(child.ParentId, parent);
                    await _context.UpdateItemContainers(child.ParentId, childItems);
                    await _context.DeleteContainer(SelectedContainer);
                    Node = await RefreshChildren(Node.Parent, Node);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    var parent = await _context.GetParentHierarchies(SelectedContainer.Id);
                    await _context.DeleteContainer(SelectedContainer);

                    foreach (var children in Node.Children)
                    {
                        children.Parent = null;
                        Nodes.Add(children);
                    }
                    Nodes.Remove(Node);
                    await Shell.Current.GoToAsync("..");
                }
            }
        }
    }
}
