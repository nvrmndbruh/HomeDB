using System.Collections.ObjectModel;
using UraniumUI;

namespace HomeDB.Models
{
    public class TreeNode : UraniumBindableObject
    {
        private bool isLeaf;

        public int Id { get; set; }
        public ChildType Type { get; set; }
        public string Name { get; set; } // Название узла
        public string Icon { get; set; } // Путь к иконке (опционально)
        public bool IsLeaf { get => isLeaf; set => SetProperty(ref isLeaf, value); }
        public ObservableCollection<TreeNode> Children { get; } = new(); // Дочерние узлы
    }
}