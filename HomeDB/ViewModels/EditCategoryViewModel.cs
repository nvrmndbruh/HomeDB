using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;

namespace HomeDB.ViewModels
{
    [QueryProperty("SelectedCategory", "SelectedCategory")]
    [QueryProperty("Node", "Node")]
    [QueryProperty("Nodes", "Nodes")]
    public partial class EditCategoryViewModel : ObservableObject
    {
        public DatabaseContext _context = new();

        [ObservableProperty]
        public TreeNode node;

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        [ObservableProperty]
        public Category selectedCategory;

        private List<string> ValidateModel()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(SelectedCategory.Name))
            {
                errors.Add("Название обязательно");
            }

            return errors;
        }

        public async Task LoadChildren(TreeNode parent)
        {
            var itemCategories = await _context.GetItemCategories();
            var children = itemCategories.Where(h => h.CategoryId == parent.Id).ToList();
            if (children != null)
            {
                foreach (var child in children)
                {
                    var item = await _context.GetItem(child.ItemId);
                    parent.Children.Add(new TreeNode
                    {
                        Id = item.Id,
                        Type = nameof(Item),
                        Name = item.Name,
                        Icon = item.Icon,
                        IsLeaf = true,
                        Parent = parent
                    });
                }
            }
        }

        public async Task Refresh(TreeNode node)
        {
            Nodes.Remove(Node);
            Nodes.Add(node);
            await LoadChildren(node);
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
                await _context.UpdateCategory(SelectedCategory);
                var newNode = new TreeNode
                {
                    Id = Node.Id,
                    Type = Node.Type,
                    Name = SelectedCategory.Name,
                    Icon = Node.Icon,
                    IsLeaf = Node.IsLeaf,
                    Parent = Node.Parent
                };
                await Refresh(newNode);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task Delete()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Подтверждение удаления",
            "Вы уверены, что хотите удалить эту категорию? Это действие нельзя отменить.",
            "Да",
            "Нет");

            if (confirm)
            {
                await _context.DeleteCategory(SelectedCategory);
                Nodes.Remove(Node);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task DeleteChild(TreeNode node)
        {
            var itemCategories = await _context.GetItemCategories();
            var delete = itemCategories
                .FirstOrDefault(d => d.ItemId == node.Id && d.CategoryId == Node.Id);
            await _context.DeleteItemCategory(delete.Id);
            node.Parent = null;

            Node.Children.Clear();
            await LoadChildren(Node);
            if (Node.Children.Count() == 0)
                Node.IsLeaf = true;
        }

        [RelayCommand]
        async Task Add()
        {
            await Shell.Current.GoToAsync($"{nameof(ItemListPage)}", new Dictionary<string, object>
            {
                ["Node"] = Node
            });
        }
    }
}
