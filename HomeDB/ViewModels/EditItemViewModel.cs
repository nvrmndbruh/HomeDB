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

        [ObservableProperty]
        public string? priceInput;

        public EditItemViewModel()
        {
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Item) && Item != null)
                {
                    PriceInput = Item.Price?.ToString();
                }
            };
        }

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
