using BinaryTree.Implementations.Business;

namespace BinaryTree.Business.Interfaces.Business
{
    /// <summary>
    /// B-tree or Binary Tree. Searching algorithms
    /// </summary>
    public interface IBinaryTreeSearch
    {
        /// <summary>
        /// Find the (node) value in the BST that the node's value equals val and return the subtree (values) rooted with that node (value). If such a node (value) does not exist, return null.
        /// </summary>
        /// <param name="tree">Your B-tree or Binary Tree</param>
        /// <param name="target">Value what you're searching for</param>
        /// <returns>Time complicity O(N), Space Complicity C# alocate additional memory for Recurcive STACK (LIFO) calls of funcs</returns>
        int? DepthFirsSearch(TreeNode tree, int target);

        /// <summary>
        /// You are given the root of a binary search tree (BFS) and an integer val.
        /// Find the node in the BFS that the node's value equals val and return
        /// the subtree rooted with that node. If such a node does not exist, return null.
        /// </summary>
        /// <param name="tree">Your B-tree or Binary Tree</param>
        /// <param name="target">Value what you're searching for</param>
        /// <returns>Time complicity O(N), Space Complicity O(N) cos Data Structure Queue FIFO</returns>
        int? BreadthFirstSearch(TreeNode tree, int target);
    }
}
