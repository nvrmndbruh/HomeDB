using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;

namespace HomeDB.ViewModels
{
    public partial class CategoryViewModel : ObservableObject
    {
        public DatabaseContext _context = new();

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        [ObservableProperty]
        public TreeNode selectedNode;

        public CategoryViewModel()
        {
            Nodes = new ObservableCollection<TreeNode>();
            LoadRoot();
        }

        [RelayCommand]
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

        private async Task LoadRoot()
        {
            var categories = await _context.GetCategories();
            var itemCategories = await _context.GetItemCategories();

            foreach (var category in categories)
            {
                var hasChildren = itemCategories.Any(it => it.CategoryId == category.Id);
                var node = new TreeNode
                {
                    Id = category.Id,
                    Icon = "tag.png",
                    Type = nameof(Category),
                    Name = category.Name,
                    IsLeaf = !hasChildren,
                    Parent = null
                };
                Nodes.Add(node);
            }
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
                var category = await _context.GetCategory(SelectedNode.Id);
                await Shell.Current.GoToAsync($"{nameof(EditCategoryPage)}", new Dictionary<string, object>
                {
                    ["SelectedCategory"] = category,
                    ["Node"] = SelectedNode,
                    ["Nodes"] = Nodes
                });
            }
            SelectedNode = null;
        }

        [RelayCommand]
        async Task Add()
        {
            var category = new Category
            {
                Name = "Новая категория",
                Description = "Создано автоматически"
            };

            await _context.InsertCategory(category);

            Nodes.Add(new TreeNode
            {
                Id = category.Id,
                Name = category.Name,
                Icon = "tag.png",
                Type = nameof(Category),
                Parent = null,
                IsLeaf = true
            });
        }
    }
}
