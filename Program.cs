using System;
using System.Collections.Generic;
using System.Linq;

public interface IRbTree<T> where T : IComparable<T>
{
	void Insert(T item);
	IEnumerable<T> Select(Func<T, bool> condition);

	T GetLeftChildValue(T parent);
	T GetRightChildValue(T parent);
}

public interface INodeInserter<T> where T : IComparable<T>
{
	RbTree<T>.Node Insert(RbTree<T>.Node root, RbTree<T>.Node newNode, out bool inserted);
}

public class RbTree<T> : IRbTree<T> where T : IComparable<T>
{
	private readonly INodeInserter<T> _inserter;
	public RbTree() : this(new DefaultInserter<T>()) { }
	public RbTree(INodeInserter<T> inserter)
	{
		_inserter = inserter;
	}
	public enum NodeColor { Red, Black }

	public T GetLeftChildValue(T parent)
	{
		var node = FindNode(parent);
		return node.Left != null ? node.Left.Data : throw new KeyNotFoundException("Left child not found");
	}

	public T GetRightChildValue(T parent)
	{
		var node = FindNode(parent);
		return node.Right != null ? node.Right.Data : throw new KeyNotFoundException("Right child not found");
	}
	private Node FindNode(T value)
	{
		var current = _root;
		while (current != null)
		{
			int cmp = value.CompareTo(current.Data);
			if (cmp == 0) return current;
			current = cmp < 0 ? current.Left : current.Right;
		}
		throw new KeyNotFoundException("Value not found in tree");
	}

	public NodeColor GetRootColor()
	{
		if (_root == null)
		{
			throw new InvalidOperationException("Tree is empty");
		}
		return _root.Color;
	}
	public T GetRootValue()
	{
		if (_root == null)
		{
			throw new InvalidOperationException("Tree is empty");
		}
		return _root.Data;
	}

	public NodeColor GetNodeColor(T item)
	{
		if (_root == null)
		{
			throw new InvalidOperationException("Tree is empty");
		}

		Node current = _root;
		while (current != null)
		{
			int comparison = item.CompareTo(current.Data);
			if (comparison == 0)
			{
				return current.Color;
			}
			current = comparison < 0 ? current.Left : current.Right;
		}

		throw new KeyNotFoundException($"Item {item} not found in tree");
	}

	public T GetNodeValue(T item)
	{
		if (_root == null)
		{
			throw new InvalidOperationException("Tree is empty");
		}

		Node current = _root;
		while (current != null)
		{
			int comparison = item.CompareTo(current.Data);
			if (comparison == 0)
			{
				return current.Data;
			}
			current = comparison < 0 ? current.Left : current.Right;
		}

		throw new KeyNotFoundException($"Item {item} not found in tree");
	}

	public class Node
	{
		public T Data { get; set; }
		public Node Left { get; set; }
		public Node Right { get; set; }
		public Node Parent { get; set; }
		public NodeColor Color { get; set; }

		public Node(T data)
		{
			Data = data;
			Color = NodeColor.Red;
		}
	}

	private Node _root;
	private int _count;

	public void Insert(T item)
	{
		var newNode = new Node(item);
		bool inserted;

		_root = _inserter.Insert(_root, newNode, out inserted);
		if (inserted)
		{
			FixViolations(newNode);
			_count++;
		}
	}

	private void FixViolations(Node node)
	{
		Node parent = null;
		Node grandParent = null;

		while (node != _root && node.Color == NodeColor.Red && node.Parent.Color == NodeColor.Red)
		{
			parent = node.Parent;
			grandParent = parent.Parent;

			if (parent == grandParent.Left)
			{
				Node uncle = grandParent.Right;
				if (uncle != null && uncle.Color == NodeColor.Red)
				{
					grandParent.Color = NodeColor.Red;
					parent.Color = NodeColor.Black;
					uncle.Color = NodeColor.Black;
					node = grandParent;
				}
				else
				{
					if (node == parent.Right)
					{
						RotateLeft(parent);
						node = parent;
						parent = node.Parent;
					}
					RotateRight(grandParent);
					SwapColors(parent, grandParent);
					node = parent;
				}
			}
			else
			{
				Node uncle = grandParent.Left;
				if (uncle != null && uncle.Color == NodeColor.Red)
				{
					grandParent.Color = NodeColor.Red;
					parent.Color = NodeColor.Black;
					uncle.Color = NodeColor.Black;
					node = grandParent;
				}
				else
				{
					if (node == parent.Left)
					{
						RotateRight(parent);
						node = parent;
						parent = node.Parent;
					}
					RotateLeft(grandParent);
					SwapColors(parent, grandParent);
					node = parent;
				}
			}
		}

		_root.Color = NodeColor.Black;
	}

	private void RotateLeft(Node node)
	{
		Node rightChild = node.Right;
		node.Right = rightChild.Left;

		if (node.Right != null)
			node.Right.Parent = node;

		rightChild.Parent = node.Parent;

		if (node.Parent == null)
			_root = rightChild;
		else if (node == node.Parent.Left)
			node.Parent.Left = rightChild;
		else
			node.Parent.Right = rightChild;

		rightChild.Left = node;
		node.Parent = rightChild;
	}

	private void RotateRight(Node node)
	{
		Node leftChild = node.Left;
		node.Left = leftChild.Right;

		if (node.Left != null)
			node.Left.Parent = node;

		leftChild.Parent = node.Parent;

		if (node.Parent == null)
			_root = leftChild;
		else if (node == node.Parent.Left)
			node.Parent.Left = leftChild;
		else
			node.Parent.Right = leftChild;

		leftChild.Right = node;
		node.Parent = leftChild;
	}

	private void SwapColors(Node node1, Node node2)
	{
		var temp = node1.Color;
		node1.Color = node2.Color;
		node2.Color = temp;
	}

	public IEnumerable<T> Select(Func<T, bool> condition)
	{
		if (_root == null)
			yield break;

		var stack = new Stack<Node>();
		var current = _root;

		while (stack.Count > 0 || current != null)
		{
			if (current != null)
			{
				stack.Push(current);
				current = current.Left;
			}
			else
			{
				current = stack.Pop();
				if (condition(current.Data))
				{
					yield return current.Data;
				}
				current = current.Right;
			}
		}
	}
}

public class DefaultInserter<T> : INodeInserter<T> where T : IComparable<T>
{
	public RbTree<T>.Node Insert(RbTree<T>.Node root, RbTree<T>.Node newNode, out bool inserted)
	{
		inserted = false;
		if (root == null)
		{
			inserted = true;
			return newNode;
		}

		int comparison = newNode.Data.CompareTo(root.Data);
		if (comparison < 0)
		{
			root.Left = Insert(root.Left, newNode, out inserted);
			if (inserted) root.Left.Parent = root;
		}
		else if (comparison > 0)
		{
			root.Right = Insert(root.Right, newNode, out inserted);
			if (inserted) root.Right.Parent = root;
		}

		return root;
	}
}

public static class RbTreeExtensions
{
	public static IEnumerable<T> Select<T>(this IEnumerable<T> sequence, Func<T, bool> condition) where T : IComparable<T>
	{
		var tree = new RbTree<T>();

		foreach (var item in sequence)
		{
			tree.Insert(item);
		}

		return tree.Select(condition);
	}
}

class Program
{
	static void Main(string[] args)
	{
		var testData = Enumerable.Range(0, 50).Select(x => x * 2);

		Console.WriteLine("Starting filtering.hhh..");

		var filtered = testData.Select(x => x > 49 && x % 2 == 0);

		Console.WriteLine("Results:");
		foreach (var item in filtered.Take(10))
		{
			Console.WriteLine(item);
		}

		Console.WriteLine("Completed. Press any key to exit...");
		Console.ReadKey();
	}
}