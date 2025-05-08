using CommunityToolkit.Mvvm.ComponentModel;
using HomeDB.Models;
using System.Collections.ObjectModel;
using UraniumUI;

namespace HomeDB.Data
{
    public partial class TreeNode : ObservableObject
    {
        private bool isLeaf;

        public int Id { get; set; }
        public ChildType Type { get; set; }
        public string Name { get; set; } // Название узла
        public string Icon { get; set; } // Путь к иконке (опционально)
        public bool IsLeaf { get => isLeaf; set => SetProperty(ref isLeaf, value); }
        public ObservableCollection<TreeNode> Children { get; } = new(); // Дочерние узлы
        public TreeNode? Parent { get; set; }
    }
}