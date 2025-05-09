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

        public void RefreshNode(TreeNode node)
        {
            var parent = Node.Parent;
            parent?.Children.Remove(Node);
            parent?.Children.Add(node);
        }

        public void DeleteNode(TreeNode node)
        {
            var parent = Node.Parent;
            parent?.Children.Remove(Node);
            if (parent?.Children.Count == 0)
            {
                parent.IsLeaf = true;
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
            RefreshNode(newNode);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task Delete()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Подтверждение удаления",
            "Вы уверены, что хотите удалить эту вещь? Это действие нельзя отменить.",
            "Да",
            "Нет");

            if (confirm)
            {
                await _context.DeleteItem(Item);
                DeleteNode(Node);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
