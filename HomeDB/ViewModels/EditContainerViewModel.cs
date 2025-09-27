using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
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

        private async Task LoadChildren(TreeNode parent)
        {
            var containers = await DatabaseContext.Containers.GetAllAsync();;
            var hierarchies = await DatabaseContext.Hierarchies.GetAllAsync();
            var items = await DatabaseContext.Items.GetAllAsync();
            var itemContainers = await DatabaseContext.ItemContainers.GetAllAsync();

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
                await DatabaseContext.Containers.UpdateAsync(SelectedContainer);
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
                var item = await DatabaseContext.Items.GetAsync(node.Id);
                await Shell.Current.GoToAsync($"{nameof(EditItemPage)}", new Dictionary<string, object>
                {
                    ["Item"] = item,
                    ["Node"] = node
                });
            }
            else
            {
                var container = await DatabaseContext.Containers.GetAsync(node.Id);
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
                    await DatabaseContext.Items.DeleteAsync(node.Id);
                    Node = await RefreshChildren(Node, node, true);
                }
                else
                {
                    var container = await DatabaseContext.Containers.GetAsync(node.Id);
                    var parent = await DatabaseContext.Hierarchies.GetByParent(node.Id);
                    var child = await DatabaseContext.Hierarchies.GetByChild(node.Id);

                    foreach (Hierarchy h in parent)
                    {
                        h.ParentId = child.ParentId;
                        await DatabaseContext.Hierarchies.UpdateAsync(h);
                    }

                    var childItems = await DatabaseContext.ItemContainers.GetByParent(node.Id);

                    foreach(ItemContainer it in childItems)
                    {
                        it.ContainerId = Node.Id;
                        await DatabaseContext.ItemContainers.UpdateAsync(it);
                    }

                    await DatabaseContext.Containers.DeleteAsync(container.Id);
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
                var parent = await DatabaseContext.Hierarchies.GetByParent(SelectedContainer.Id);

                if (Node.Parent != null)
                {
                    var child = await DatabaseContext.Hierarchies.GetByChild(SelectedContainer.Id);
                    var childItems = await DatabaseContext.ItemContainers.GetByParent(Node.Id);

                    foreach (Hierarchy h in parent)
                    {
                        h.ParentId = child.ParentId;
                        await DatabaseContext.Hierarchies.UpdateAsync(h);
                    }

                    foreach (ItemContainer it in childItems)
                    {
                        it.ContainerId = child.ParentId;
                        await DatabaseContext.ItemContainers.UpdateAsync(it);
                    }

                    await DatabaseContext.Containers.DeleteAsync(SelectedContainer.Id);
                    Node = await RefreshChildren(Node.Parent, Node);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DatabaseContext.Containers.DeleteAsync(SelectedContainer.Id);

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
