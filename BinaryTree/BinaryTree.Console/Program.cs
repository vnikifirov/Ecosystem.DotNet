using static System.Console;
using BinaryTree.Implementations.Business;

namespace BinaryTree.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new TreeNode(1, null, new TreeNode(2, new TreeNode(3), null));

            var resultBFS = tree.BreadthFirstSearch(tree, 3);
            var resultDFS = tree.DepthFirsSearch(tree, 3);

            WriteLine($"Result BFS: {resultBFS}");
            WriteLine($"Result BFS: {resultDFS}");
        }
    }
}
