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
    }
}
