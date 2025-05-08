using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;

namespace HomeDB.ViewModels
{
    [QueryProperty("Node", "Node")]
    [QueryProperty(nameof(HomeDB.Models.Item), nameof(HomeDB.Models.Item))]
    public partial class EditItemViewModel : ObservableObject
    {
        [ObservableProperty]
        public TreeNode node;

        [ObservableProperty]
        public Item item;

        DatabaseContext _context = new();

        public void Refresh(TreeNode node)
        {
            if (Node.Parent != null)
            {
                var parent = Node.Parent;
                parent.Children.Remove(Node);
                parent.Children.Add(node);
            }
        }

        [RelayCommand]
        async Task SelectPhoto()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Выберите фото"
            });
            if (result != null)
            {
                Item.Photo = result.FullPath;
            }
        }

        [RelayCommand]
        async Task Save()
        {
            await _context.UpdateItem(Item);
            var newNode = new TreeNode
            {
                Id = Node.Id,
                Type = Node.Type,
                Name = Item.Name,
                Icon = Item.Icon,
                IsLeaf = Node.IsLeaf,
                Parent = Node.Parent
            };
            Refresh(newNode);
            await Shell.Current.GoToAsync("..");
        }
    }
}
