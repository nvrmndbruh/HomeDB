using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;

namespace HomeDB.ViewModels
{
    [QueryProperty("Node", "Node")]
    public partial class ItemListViewModel : ObservableObject
    {
        DatabaseContext _context = new();

        [ObservableProperty]
        public ObservableCollection<Item> nodes;

        [ObservableProperty]
        public TreeNode node;

        public ItemListViewModel()
        {
            Nodes = new ObservableCollection<Item>();
            Init();
        }

        async Task Init()
        {
            var items = await _context.GetItems();
            var itemCategories = await _context.GetItemCategories();
            //var available = item.Where(i => !itemCategories.)

            foreach (var item in items
                .Where(i => !itemCategories.Any(ic => Node.Id == ic.CategoryId && i.Id == ic.ItemId))
                .ToList())
            {
                Nodes.Add(item);
            }
        }

        [RelayCommand]
        async Task Select(Item item)
        {
            var itemCategory = new ItemCategory
            {
                ItemId = item.Id,
                CategoryId = Node.Id
            };
            await _context.InsertItemCategory(itemCategory);
            Node.Children.Add(new TreeNode
            {
                Id = item.Id,
                Name = item.Name,
                Type = nameof(Item),
                Parent = Node,
                IsLeaf = true,
                Icon = item.Icon
            });
            Node.IsLeaf = false;
            await Shell.Current.GoToAsync("..");
        }
    }
}
