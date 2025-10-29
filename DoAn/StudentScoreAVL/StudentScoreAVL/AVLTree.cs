using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentScoreAVL
{
    public class AVLNode
    {
        public Student Data;
        public AVLNode Left;
        public AVLNode Right;
        public int Height;

        public AVLNode(Student data)
        {
            Data = data;
            Height = 1;
        }
    }

    public class AVLTree
    {
        public AVLNode Root;

        private int Height(AVLNode node)
        {
            return node == null ? 0 : node.Height;
        }

        private int GetBalance(AVLNode node)
        {
            return node == null ? 0 : Height(node.Left) - Height(node.Right);
        }

        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            return x;
        }

        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            return y;
        }

        public void Insert(Student data)
        {
            Root = Insert(Root, data);
        }

        private AVLNode Insert(AVLNode node, Student data)
        {
            if (node == null)
                return new AVLNode(data);

            
            if (data.MathScore < node.Data.MathScore)
                node.Left = Insert(node.Left, data);
            else if (data.MathScore > node.Data.MathScore)
                node.Right = Insert(node.Right, data);
            else
                return node;

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            // Left Left
            if (balance > 1 && data.MathScore < node.Left.Data.MathScore)
                return RightRotate(node);

            // Right Right
            if (balance < -1 && data.MathScore > node.Right.Data.MathScore)
                return LeftRotate(node);

            // Left Right
            if (balance > 1 && data.MathScore > node.Left.Data.MathScore)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left
            if (balance < -1 && data.MathScore < node.Right.Data.MathScore)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        // In-order traversal trả về danh sách Student (tăng dần theo MathScore)
        public List<Student> InOrderTraversal()
        {
            List<Student> result = new List<Student>();
            InOrder(Root, result);
            return result;
        }

        private void InOrder(AVLNode node, List<Student> list)
        {
            if (node != null)
            {
                InOrder(node.Left, list);
                list.Add(node.Data);
                InOrder(node.Right, list);
            }
        }
    }
}
