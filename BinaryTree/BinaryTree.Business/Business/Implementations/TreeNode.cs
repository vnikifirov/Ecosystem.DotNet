using System.Collections.Generic;
using BinaryTree.Business.Interfaces.Business;

namespace BinaryTree.Implementations.Business
{
    /// <inheritdoc/>
    public class TreeNode : IBinaryTreeSearch
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        private static Queue<TreeNode> _bfsQueue = new Queue<TreeNode>();

        /// <summary>
        /// Create instance of your B-tree or Binary Tree
        /// </summary>
        /// <param name="val">Value of node in B-tree or Binary Tree</param>
        /// <param name="left">Left node in B-tree or Binary Tree</param>
        /// <param name="right">Rigth in B-tree or Binary Tree</param>
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }

        /// <inheritdoc/>
        public int? DepthFirsSearch(TreeNode tree, int target)
        {
            if (tree is null)
                return null;

            if (tree.val == target)
                return target;

            var resultLeftNodes = DepthFirsSearch(tree.left, target);
            if (resultLeftNodes == target)
                return target;

            var resultRigthNodes = DepthFirsSearch(tree.right, target);
            if (resultRigthNodes == target)
                return target;

            return null;
        }

        /// <inheritdoc/>
        public int? BreadthFirstSearch(TreeNode tree, int target)
        {
            if (tree is null)
                return null;

            if (tree.val == target)
                return target;

            _bfsQueue.Enqueue(tree);

            while (_bfsQueue.Count > 0)
            {
                // FIFO
                var current = _bfsQueue.Dequeue();
                if (current.val == target)
                    return target;

                if (current.left is not null)
                    _bfsQueue.Enqueue(current.left);

                if (current.right is not null)
                    _bfsQueue.Enqueue(current.right);
            }

            return null;
        }
    }
}
