using HomeDB.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UraniumUI;

namespace HomeDB.ViewModel
{
    public class MainViewModel : BindableObject
    {
        private readonly HomeDbContext _context;

        public ObservableCollection<TreeNode> Nodes { get; private set; }

        public ICommand LoadChildrenCommand { get; set; }

        public MainViewModel(HomeDbContext context)
        {
            _context = context;
            LoadRoot();
            LoadChildrenCommand = new Command<TreeNode>((node) =>
            {
                foreach (var connection in _context.Hierarchies.Where(h => h.ParentId == node.Id).ToList())
                {
                    if (connection.ChildType == ChildType.Container)
                    {
                        var childContainer = _context.Containers.Find(connection.ChildId);
                        if (childContainer != null)
                        {
                            node.Children.Add(new TreeNode
                            {
                                Id = childContainer.Id,
                                Type = connection.ChildType,
                                Name = childContainer.Name,
                                Icon = childContainer.Icon,
                                IsLeaf = false
                            });
                        }
                    }
                    else if (connection.ChildType == ChildType.Item)
                    {
                        var item = _context.Items.Find(connection.ChildId);
                        if (item != null)
                        {
                            node.Children.Add(new TreeNode
                            {
                                Id = item.Id,
                                Type = connection.ChildType,
                                Name = item.Name,
                                Icon = item.Icon,
                                IsLeaf = true
                            });
                        }
                    }
                }

                if (node.Children.Count == 0)
                {
                    node.IsLeaf = true;
                }
            });
        }

        void LoadRoot()
        {
            var root = _context.Containers
                .Where(c => !_context.Hierarchies.Any(h => h.ParentId == c.Id))
                .ToList();

            foreach (var container in root)
            {
                var node = new TreeNode
                {
                    Id = container.Id,
                    Type = ChildType.Container,
                    Name = container.Name,
                    Icon = container.Icon,
                    IsLeaf = false
                };

                Nodes.Add(node);
            }
        }


        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}