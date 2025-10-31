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
        public int Key => Data.ID;
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


            if (data.ID < node.Data.ID)
                node.Left = Insert(node.Left, data);
            else if (data.ID > node.Data.ID)
                node.Right = Insert(node.Right, data);
            else
                return node;

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            // Left Left
            if (balance > 1 && data.ID < node.Left.Data.ID)
                return RightRotate(node);

            // Right Right
            if (balance < -1 && data.ID > node.Right.Data.ID)
                return LeftRotate(node);

            // Left Right
            if (balance > 1 && data.ID > node.Left.Data.ID)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left
            if (balance < -1 && data.ID < node.Right.Data.ID)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        //LNR
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
        //tổng số node
        public int CountNodes()
        {
            return CountNodes(Root);
        }
        private int CountNodes(AVLNode node)
        {
            if (node == null) return 0;
            return 1 + CountNodes(node.Left) + CountNodes(node.Right);
        }
        //chiều cao
        public int GetTreeHeight()
        {
            return GetTreeHeight(Root);
        }
        private int GetTreeHeight(AVLNode node)
        {
            if (node == null) return 0;
            return 1 + Math.Max(GetTreeHeight(node.Left), GetTreeHeight(node.Right));
        }
        // Đếm số nút (0,1,2) con
        public (int leafNodes, int one, int two) CountNodeTypes()
        {
            int leaf = 0, one = 0, two = 0;
            CountNodeTypes(Root, ref leaf, ref one, ref two);
            return (leaf, one, two);
        }

        private void CountNodeTypes(AVLNode node, ref int leaf, ref int one, ref int two)
        {
            if (node == null) return;

            int childCount = 0;
            if (node.Left != null) childCount++;
            if (node.Right != null) childCount++;

            if (childCount == 0) leaf++;
            else if (childCount == 1) one++;
            else two++;

            CountNodeTypes(node.Left, ref leaf, ref one, ref two);
            CountNodeTypes(node.Right, ref leaf, ref one, ref two);
        }
        //Tìm kiếm
        public Student Search(int id)
        {
            return Search(Root, id);
        }

        private Student Search(AVLNode node, int id)
        {
            if (node == null)
                return null;

            if (id == node.Data.ID)
                return node.Data;
            else if (id < node.Data.ID)
                return Search(node.Left, id);
            else
                return Search(node.Right, id);
        }
        // Xóa
        public void Delete(int id)
        {
            Root = Delete(Root, id);
        }

        private AVLNode Delete(AVLNode root, int id)
        {
            if (root == null)
                return root;
            if (id < root.Data.ID)
                root.Left = Delete(root.Left, id);
            else if (id > root.Data.ID)
                root.Right = Delete(root.Right, id);
            else
            {
                // 1 or 0 con
                if ((root.Left == null) || (root.Right == null))
                {
                    AVLNode temp = root.Left ?? root.Right;

                    // 0
                    if (temp == null)
                    {
                        root = null;
                    }
                    else
                    {
                        root = temp;
                    }
                }
                else
                {
                    //2 con:node nhỏ nhất bên phải
                    AVLNode temp = MinValueNode(root.Right);
                    root.Data = temp.Data;
                    root.Right = Delete(root.Right, temp.Data.ID);
                }
            }
            if (root == null)
                return root;
            root.Height = 1 + Math.Max(Height(root.Left), Height(root.Right));
            int balance = GetBalance(root);
            if (balance > 1 && GetBalance(root.Left) >= 0)
                return RightRotate(root);

            if (balance > 1 && GetBalance(root.Left) < 0)
            {
                root.Left = LeftRotate(root.Left);
                return RightRotate(root);
            }

            if (balance < -1 && GetBalance(root.Right) <= 0)
                return LeftRotate(root);

            if (balance < -1 && GetBalance(root.Right) > 0)
            {
                root.Right = RightRotate(root.Right);
                return LeftRotate(root);
            }

            return root;
        }

        private AVLNode MinValueNode(AVLNode node)
        {
            AVLNode current = node;
            while (current.Left != null)
                current = current.Left;
            return current;
        }
        public bool Sua(Student updatedStudent)
        {
            return Sua(Root, updatedStudent);
        }

        private bool Sua(AVLNode node, Student updatedStudent)
        {
            if (node == null)
                return false;

            if (updatedStudent.ID < node.Data.ID)
                return Sua(node.Left, updatedStudent);
            else if (updatedStudent.ID > node.Data.ID)
                return Sua(node.Right, updatedStudent);
            else
            {
                node.Data.Gender = updatedStudent.Gender;
                node.Data.RaceEthnicity = updatedStudent.RaceEthnicity;
                node.Data.ParentalEducation = updatedStudent.ParentalEducation;
                node.Data.Lunch = updatedStudent.Lunch;
                node.Data.TestPreparationCourse = updatedStudent.TestPreparationCourse;
                node.Data.MathScore = updatedStudent.MathScore;
                node.Data.ReadingScore = updatedStudent.ReadingScore;
                node.Data.WritingScore = updatedStudent.WritingScore;
                return true;
            }
        }

    }
}
