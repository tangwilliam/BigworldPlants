//#define DEBUG_SIMPLE_TREE
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CommonLibs.Utility
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent;
        public T Val;
        public TreeNode<T> LChild;
        public TreeNode<T> RChild;
        public float PosX;
        public float PosY;
        public int Depth;

        public override string ToString()
        {
            return string.Format("{{{0},{1}}}", PosX, PosY);
        }
    }

    public class SimpleKDTree<T>
    {
        private int XComparer(TreeNode<T> a, TreeNode<T> b)
        {
            if (a.PosX == b.PosX)
            {
                return 0;
            }
            return a.PosX > b.PosX ? -1 : 1;
        }

        private int YComparer(TreeNode<T> a, TreeNode<T> b)
        {
            if (a.PosY == b.PosY)
            {
                return 0;
            }
            return a.PosY > b.PosY ? -1 : 1;
        }

        //List<T> m_list = new List<T>();
        List<TreeNode<T>> m_nodes = new List<TreeNode<T>>();
        public TreeNode<T> m_root;
        //private static int m_curIndex = 0;

        public void Insert(T node, float x, float y)
        {
            m_nodes.Add(new TreeNode<T>
            {
                Val = node,
                PosX = x,
                PosY = y,
            });
        }

        public void Build()
        {
            m_root = KDTreeUtility.FindMid(m_nodes, 0, m_nodes.Count, XComparer);
            if (null == m_root)
            {
                DebugL8.LogError("kdTree build failed");
                return;
            }
            m_root.Depth = 1;
            AddChild(m_nodes, m_root, 0, m_nodes.Count, 1);
        }

        public TreeNode<T> Find(float x, float y)
        {
            m_loopCount = 0;
            if (null == m_root)
            {
                DebugL8.LogError("KDTree build failed and can't do finding");
                return null;
            }
            //二分法找到可能最近的叶子节点
            TreeNode<T> nearestNode = FindLeaf(m_root, x, y);
            float nearestDistance = (nearestNode.PosX - x) * (nearestNode.PosX - x) + (nearestNode.PosY - y) * (nearestNode.PosY - y);
            //FindDown(m_root, x, y, ref nearestNode, ref nearestDistance);
            //向上找实际最近的叶子节点
            FindUp(nearestNode, x, y, ref nearestNode, ref nearestDistance);
#if DEBUG_SIMPLE_TREE
            System.IO.File.WriteAllText(Application.dataPath + "/../SimpleKDTree.txt", m_stringBuilder.ToString());
            m_stringBuilder.Length = 0;
#endif
            return nearestNode;
        }

        private bool IsCompareX(int depth)
        {
            return depth % 2 == 1;
        }


        private const int maxLoopCount = 1000;
        private int m_loopCount = 0;
        private void FindUp(TreeNode<T> node, float x, float y, ref TreeNode<T> nearestNode, ref float nearestDistance)
        {
            if (m_loopCount++ > maxLoopCount)
            {
                DebugL8.LogError("max loop count reached");
                return;
            }
            //DebugLog("FindUp {0}", node.ToString());
            var upNode = node.Parent;
            if (null == upNode)
            {
                //DebugLog("upnode for {0} is null", node.ToString());
                return;
            }
            if (IsCompareX(upNode.Depth))
            {
                if (upNode.PosX > node.PosX)
                {
                    if (null != upNode.RChild && upNode.PosX <= nearestNode.PosX + nearestDistance)
                    {
                        FindDown(upNode.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    CalculateNearestDistance(upNode, x, y, ref nearestNode, ref nearestDistance);
                    FindUp(upNode, x, y, ref nearestNode, ref nearestDistance);
                }
                else
                {
                    if (null != upNode.LChild && upNode.PosX >= nearestNode.PosX - nearestDistance)
                    {
                        FindDown(upNode.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    CalculateNearestDistance(upNode, x, y, ref nearestNode, ref nearestDistance);
                    FindUp(upNode, x, y, ref nearestNode, ref nearestDistance);
                }
            }
            else
            {
                if (upNode.PosY > node.PosY)
                {
                    if (null != upNode.RChild && upNode.PosY <= nearestNode.PosY + nearestDistance)
                    {
                        FindDown(upNode.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    CalculateNearestDistance(upNode, x, y, ref nearestNode, ref nearestDistance);
                    FindUp(upNode, x, y, ref nearestNode, ref nearestDistance);
                }
                else
                {
                    if (null != upNode.LChild && upNode.PosY >= nearestNode.PosY - nearestDistance)
                    {
                        FindDown(upNode.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    CalculateNearestDistance(upNode, x, y, ref nearestNode, ref nearestDistance);
                    FindUp(upNode, x, y, ref nearestNode, ref nearestDistance);
                }
            }
        }

        private TreeNode<T> FindLeaf(TreeNode<T> node, float x, float y)
        {
            if (null == node)
            {
                //DebugLog("node is null");
                return null;
            }
            var depth = node.Depth;
            if (IsCompareX(depth))
            {
                if (node.PosX <= x)
                {
                    if (null != node.RChild)
                    {
                        return FindLeaf(node.RChild, x, y);
                    }
                    //如果没有对应的节点，找到另外一个叶子节点，因为是平衡二叉树，所以一定是叶子节点
                    if(null != node.LChild)
                    {
                        return node.LChild;
                    }
                    else
                    {
                        return node;
                    }
                }
                else
                {
                    if (null != node.LChild)
                    {
                        return FindLeaf(node.LChild, x, y);
                    }
                    else if(null != node.RChild)
                    {
                        return node.RChild;
                    }
                    else
                    {
                        return node;
                    }
                }
            }
            else
            {
                if (y >= node.PosY)
                {
                    if (null != node.RChild)
                    {
                        return FindLeaf(node.RChild, x, y);
                    }
                    else
                    {
                        return node;
                    }
                }
                else
                {
                    if (null != node.LChild)
                    {
                        return FindLeaf(node.LChild, x, y);
                    }
                    else
                    {
                        return node;
                    }
                }
            }
        }

        private void CalculateNearestDistance(TreeNode<T> node, float x, float y, ref TreeNode<T> nearestNode, ref float nearestDistance)
        {
            if (null == nearestNode)
            {
                nearestNode = node;
                nearestDistance = (x - nearestNode.PosX) * (x - nearestNode.PosX) + (y - nearestNode.PosY) * (y - nearestNode.PosY);
                //DebugLog("new nearest node {0}", node.ToString());
            }
            else
            {
                var curDistance = (x - node.PosX) * (x - node.PosX) + (y - node.PosY) * (y - node.PosY);
                if (curDistance < nearestDistance)
                {
                    nearestNode = node;
                    nearestDistance = curDistance;
                    //DebugLog("new nearest node {0}", node.ToString());
                }
            }
        }

        private void FindDown(TreeNode<T> node, float x, float y, ref TreeNode<T> nearestNode, ref float nearestDistance)
        {
            if (null == node)
            {
                //DebugLog("node is null");
                return;
            }
            //DebugLog("FindDown {0}", node.ToString());
            var depth = node.Depth;
            CalculateNearestDistance(node, x, y, ref nearestNode, ref nearestDistance);
            if (IsCompareX(depth))
            {
                if (x >= node.PosX)
                {
                    if (null != node.RChild)
                    {
                        FindDown(node.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    if (null != node.LChild && node.LChild.PosX > x - nearestDistance)
                    {
                        FindDown(node.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                }
                else
                {
                    if (null != node.LChild)
                    {
                        FindDown(node.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    if (null != node.RChild && node.RChild.PosX < x + nearestDistance)
                    {
                        FindDown(node.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                }
            }
            else
            {
                if (y >= node.PosY)
                {
                    if (null != node.RChild)
                    {
                        FindDown(node.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    if(null != node.LChild && node.LChild.PosY > y - nearestDistance)
                    {
                        FindDown(node.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                }
                else
                {
                    if (null != node.LChild)
                    {
                        FindDown(node.LChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                    if(null != node.RChild && node.RChild.PosY < y + nearestDistance)
                    {
                        FindDown(node.RChild, x, y, ref nearestNode, ref nearestDistance);
                    }
                }
            }
        }

        Queue<TreeNode<T>> m_forceFindQueue = new Queue<TreeNode<T>>();
        public TreeNode<T> ForceFindNode(TreeNode<T> root, float x, float y)
        {
            TreeNode<T> nearestNode = null;
            float nearestDistance = float.MaxValue;
            m_forceFindQueue.Enqueue(root);
            while(m_forceFindQueue.Count > 0)
            {
                var node = m_forceFindQueue.Dequeue();
                CalculateNearestDistance(node, x, y, ref nearestNode, ref nearestDistance);
                if(null != node.LChild)
                {
                    m_forceFindQueue.Enqueue(node.LChild);
                }
                if(null != node.RChild)
                {
                    m_forceFindQueue.Enqueue(node.RChild);
                }
            }
            return nearestNode;
        }

        private void AddChild(List<TreeNode<T>> nodes, TreeNode<T> curNode, int start, int end, int depth)
        {
            if (null == curNode)
            {
                return;
            }
            if (start == end)
            {
                return;
            }
            var mid = (start + end) / 2;
            Func<TreeNode<T>, TreeNode<T>, int> comparer = null;
            if ((depth + 1) % 2 == 1)
            {
                comparer = XComparer;
            }
            else
            {
                comparer = YComparer;
            }
            if (mid + 1 < end)
            {
                curNode.LChild = KDTreeUtility.FindMid(nodes, mid + 1, end, comparer);
                curNode.LChild.Depth = depth + 1;
                curNode.LChild.Parent = curNode;
            }
            if (start < mid)
            {
                curNode.RChild = KDTreeUtility.FindMid(nodes, start, mid, comparer);
                curNode.RChild.Depth = depth + 1;
                curNode.RChild.Parent = curNode;
            }
            var nextDepth = depth + 1;
            AddChild(nodes, curNode.LChild, mid + 1, end, nextDepth);
            AddChild(nodes, curNode.RChild, start, mid, nextDepth);
        }

#if DEBUG_SIMPLE_TREE
        private System.Text.StringBuilder m_stringBuilder = new System.Text.StringBuilder();
#endif
        void DebugLog(string val, params object[] args)
        {
#if DEBUG_SIMPLE_TREE
            m_stringBuilder.AppendLine(string.Format(val, args));
            DebugL8.Log(val, args);
#endif
        }
    }
}
