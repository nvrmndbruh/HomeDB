using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeDB.Data;
using HomeDB.Models;
using System.Collections.ObjectModel;

namespace HomeDB.ViewModels
{
    [QueryProperty("Node", "Node")]
    [QueryProperty(nameof(HomeDB.Models.Item), nameof(HomeDB.Models.Item))]
    [QueryProperty("Nodes", "Nodes")]
    public partial class EditItemViewModel : ObservableObject
    {
        [ObservableProperty]
        public TreeNode node;

        [ObservableProperty]
        public Item item;

        [ObservableProperty]
        public ObservableCollection<TreeNode> nodes;

        DatabaseContext _context = new();

        [ObservableProperty]
        public string? priceInput;

        [ObservableProperty]
        public string? photo;

        public EditItemViewModel()
        {
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Item) && Item != null)
                {
                    PriceInput = Item.Price?.ToString();
                    Photo = Item.Photo;
                }
            };
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

        public void DeleteNode(TreeNode node)
        {
            var parent = Node.Parent;
            if (parent != null)
            {
                parent?.Children.Remove(Node);
                if (parent?.Children.Count == 0)
                {
                    parent.IsLeaf = true;
                }
            }
            else
            {
                Nodes.Remove(node);
            }
        }

        private List<string> ValidateModel()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Item.Name))
            {
                errors.Add("Название обязательно");
            }

            if (string.IsNullOrWhiteSpace(Item.Icon))
            {
                errors.Add("Иконка обязательна");
            }

            if (!string.IsNullOrWhiteSpace(PriceInput))
            {
                if (!double.TryParse(PriceInput, out var price) || price <= 0)
                {
                    errors.Add("Цена должна быть числом больше 0");
                }
            }

            return errors;
        }

        [RelayCommand]
        async Task SelectPhoto()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Выберите действие",
            null,
            "Выбрать фото",
            "Очистить фото");

            if (confirm)
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите фото"
                });
                if (result != null)
                {
                    Photo = result.FullPath;
                }
            }
            else
            {
                Photo = null;
            }
        }

        [RelayCommand]
        async void Save()
        {
            var errors = ValidateModel();
            if (errors.Count > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", string.Join("\n", errors), "ОК");
                return;
            }
            else
            {
                Item.Price = string.IsNullOrEmpty(PriceInput) ? null : decimal.Parse(PriceInput);
                Item.Photo = Photo;
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
                var items = await _context.GetItems();
                var itemContainers = await _context.GetItemContainers();
                DeleteNode(Node);
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
