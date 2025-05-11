using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace HomeDB.Data
{
    public partial class TreeNode : ObservableObject
    {
        private bool isLeaf;

        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; } // Название узла
        public string? Icon { get; set; } // Путь к иконке (опционально)
        public bool IsLeaf { get => isLeaf; set => SetProperty(ref isLeaf, value); }
        public ObservableCollection<TreeNode> Children { get; } = new(); // Дочерние узлы
        public TreeNode? Parent { get; set; }
    }
}