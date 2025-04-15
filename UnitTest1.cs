using NUnit.Framework;
using System.Linq;
using Moq;
using RedBlackTree2; 


namespace RedBlackTree2.Tests
{

	public class InsertElements
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Tree_Is_Empty_After_Creating()
		{
			var tree = new RbTree<int>();
			var result = tree.Select(x => true);
			//проверяем, что дерево пустое при созданиии
			Assert.IsEmpty(result);
			//Assert.Pass();
		}
		[Test]
		public void Tree_Is_Not_Empty_After_Add_Elements()
		{
			var tree = new RbTree<int>();

			tree.Insert(1);
			var result = tree.Select(x => true);
			//Проверяем, что дерево  не пустое после добавления элемента
			Assert.IsNotEmpty(result);
		}
		[Test]
		public void Insert_Duplicates_Should_Not_Increase_Count()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(1);

			var result = tree.Select(x => true).Count();
			Assert.AreEqual(1, result);
		}

	}
	public class SelectElements
	{
		[Test]
		public void Select_Returns_Correct_Elements()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(2);
			tree.Insert(3);
			tree.Insert(4);
			tree.Insert(5);

			var result = tree.Select(x => x>3).ToList();
			Assert.That(result[0],Is.EqualTo(4));
			Assert.That(result[1], Is.EqualTo(5));
		}

		[Test]
		public void Select_Returns_Correct_Number_Of_Elements()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(2);
			tree.Insert(3);
			tree.Insert(4);
			tree.Insert(5);

			var result = tree.Select(x => x > 3).Count();
			Assert.AreEqual(2, result);
		}
		[Test]
		public void Select_Returns_Empty_For_Non_Matching_Condition()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(2);
			tree.Insert(3);

			var result = tree.Select(x => x > 3);
			Assert.That(result, Is.Empty);
		}

	}
	public class RBTreeProperties
	{
		[Test]
		public void Is_Root_Black_After_Insert()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(2);
			tree.Insert(3);

			var result = tree.GetRootColor();
			//Assert.That(result, Is.EqualTo(NodeColor.Black);
			Assert.AreEqual(result, RbTree<int>.NodeColor.Black);

		}
		[Test]
		public void Empty_Tree_Throws_Exeption()
		{
			var tree = new RbTree<int>();

			Assert.Throws<InvalidOperationException>(()=>tree.GetRootColor());
		}
		[Test]
		public void Element_Should_be_Red()
		{
			var tree = new RbTree<int>();
			tree.Insert(1);
			tree.Insert(2);

			var selectRes = tree.Select(x => true).ToList();
			var secondNodeColor = tree.GetNodeColor(selectRes[1]);
			Assert.AreEqual(secondNodeColor, RbTree<int>.NodeColor.Red);
			
		}
	}
	public class RotationElements
	{
		[Test]
		public void Rotation_Changes_Structure_Correctly()
		{
			var tree = new RbTree<int>();

			tree.Insert(10);
			tree.Insert(20);
			tree.Insert(30);

			var result = tree.Select(x=>true).ToList();
			var rootColor = tree.GetNodeColor(result[1]);
			// Проверяем, что корень изменился 
			Assert.AreEqual(20, tree.GetRootValue());
			Assert.AreEqual(rootColor, RbTree<int>.NodeColor.Black);
		}
		[Test]
		public void RotateLeft_Should_Change_Structure_Correctly()
		{
			var tree = new RbTree<int>();
			tree.Insert(10); // Корень
			tree.Insert(20); // Правая ветка
			tree.Insert(30); // Вызовет RotateLeft

			Assert.AreEqual(20, tree.GetRootValue()); 
			Assert.AreEqual(10, tree.GetLeftChildValue(20)); 
			Assert.AreEqual(30, tree.GetRightChildValue(20)); 
		}

		[Test]
		public void RotateRight_Should_Change_Structure_Correctly()
		{
			var tree = new RbTree<int>();
			tree.Insert(30); // Корень
			tree.Insert(20); // Левая ветка
			tree.Insert(10); // Вызовет RotateRight

			Assert.AreEqual(20, tree.GetRootValue()); 
			Assert.AreEqual(10, tree.GetLeftChildValue(20)); 
			Assert.AreEqual(30, tree.GetRightChildValue(20)); 
		}
		[Test]
		public void Multiple_Insertions_Should_Trigger_Correct_Rotations()
		{
			var tree = new RbTree<int>();
			var values = new[] { 20, 30, 25, 40, 22};

			foreach (var value in values)
			{
				tree.Insert(value);
			}

			Assert.AreEqual(25, tree.GetRootValue()); 
			Assert.AreEqual(20, tree.GetLeftChildValue(25)); 
			Assert.AreEqual(30, tree.GetRightChildValue(25)); 
			Assert.AreEqual(40, tree.GetRightChildValue(30)); 
			Assert.AreEqual(22, tree.GetRightChildValue(20)); 

		}

		//[Test]
		//public void After_Rotation_Tree_Maintain_Balance()
		//{
		//	var tree = new RbTree<int>();
		//	tree.Insert(50);
		//	tree.Insert(30);
		//	tree.Insert(70);
		//	tree.Insert(60);
		//	tree.Insert(80);

		//	tree.Insert(90);

		//	Assert.AreEqual(70, tree.GetRootValue());
		//	Assert.AreEqual(50, tree.GetLeftChildValue(70));
		//	Assert.AreEqual(90, tree.GetRightChildValue(70));
		//	Assert.AreEqual(30, tree.GetLeftChildValue(50));
		//	Assert.AreEqual(60, tree.GetRightChildValue(50));
		//	Assert.AreEqual(90, tree.GetRightChildValue(80));
		//}
	}
	public class MOQ_Tests
	{
		[Test]
		public void Should_Not_Increase_Count_When_Inserter_Returns_Duplicate()
		{
			var mockInserter = new Mock<INodeInserter<int>>();
			var tree = new RbTree<int>(mockInserter.Object);
			bool insertedFalse = false;
			bool insertedTrue = true;

			mockInserter.Setup(x => x.Insert(It.IsAny<RbTree<int>.Node>(), It.IsAny<RbTree<int>.Node>(), out insertedFalse))
					   .Returns((RbTree<int>.Node root, RbTree<int>.Node n, out bool ins) =>
					   {
						   ins = false; // Симулируем дубликат
						   return root ?? n;
					   });

			// Все инсерты теперь считаются дубликатами
			//All insert are duplicates
			tree.Insert(10); 
			tree.Insert(10); 
			tree.Insert(10); 


			Assert.AreEqual(1, tree.Select(x => true).Count());
		}
		[Test]
		public void Select_Should_Filter_Nodes_Correctly_On_MOQ_Tree()
		{
			var mockInserter = new Mock<INodeInserter<int>>();
			bool inserted;
			var tree = new RbTree<int>(mockInserter.Object);
			var root = new RbTree<int>.Node(10)
			{
				Color = RbTree<int>.NodeColor.Black,
				Left = new RbTree<int>.Node(5)
				{
					Color = RbTree<int>.NodeColor.Red,
					Left = new RbTree<int>.Node(3),
				},
				Right = new RbTree<int>.Node(15)
				{
					Color = RbTree<int>.NodeColor.Red,
					Left = new RbTree<int>.Node(12),
				}
			};
			// При вызове Insert возвращается наше МОК-дерево
			mockInserter.Setup(x => x.Insert(
				It.IsAny<RbTree<int>.Node>(),
				It.IsAny<RbTree<int>.Node>(),
				out inserted))
				.Returns(root);

			tree.Insert(10);

			var result = tree.Select(x => x > 5).ToList();

			Assert.AreEqual(3, result.Count); // 10, 12, 15, 
			Assert.Contains(10, result);
			Assert.Contains(12, result);
			Assert.Contains(15, result);
		}
		[Test]
		public void Select_Returns_Empty_For_Non_Matching_Condition_On_MOQ_Tree()
		{
			var mockInserter = new Mock<INodeInserter<int>>();
			bool inserted;
			var tree = new RbTree<int>(mockInserter.Object);
			var root = new RbTree<int>.Node(10)
			{
				Color = RbTree<int>.NodeColor.Black,
				Left = new RbTree<int>.Node(5)
				{
					Color = RbTree<int>.NodeColor.Red,
					Left = new RbTree<int>.Node(3),
				},
				Right = new RbTree<int>.Node(15)
				{
					Color = RbTree<int>.NodeColor.Red,
					Left = new RbTree<int>.Node(12),
				}
			};
			// При вызове Insert возвращается наше МОК-дерево
			mockInserter.Setup(x => x.Insert(
				It.IsAny<RbTree<int>.Node>(),
				It.IsAny<RbTree<int>.Node>(),
				out inserted))
				.Returns(root);

			tree.Insert(10);

			var result = tree.Select(x => x > 50).ToList();

			Assert.AreEqual(0, result.Count);
			Assert.That(result, Is.Empty);

		}
		[Test]
		public void Select_Should_Return_Elements_In_Correct_Order()
		{
			var mockInserter = new Mock<INodeInserter<int>>();
			var tree = new RbTree<int>(mockInserter.Object);

			var root = new RbTree<int>.Node(4)
			{
				Left = new RbTree<int>.Node(2)
				{
					Left = new RbTree<int>.Node(1),
					Right = new RbTree<int>.Node(3),
					Color = RbTree<int>.NodeColor.Red
				},
				Right = new RbTree<int>.Node(6)
				{
					Left = new RbTree<int>.Node(5),
					Right = new RbTree<int>.Node(7),
					Color = RbTree<int>.NodeColor.Red
				},
				Color = RbTree<int>.NodeColor.Black
			};

			root.Left.Parent = root;
			root.Right.Parent = root;
			root.Left.Left.Parent = root.Left;
			root.Left.Right.Parent = root.Left;
			root.Right.Left.Parent = root.Right;
			root.Right.Right.Parent = root.Right;

			bool inserted;
			mockInserter.Setup(x => x.Insert(It.IsAny<RbTree<int>.Node>(), It.IsAny<RbTree<int>.Node>(), out inserted))
					   .Returns(root);

			tree.Insert(4);

			var result = tree.Select(x => true).ToList();

			// проверяем порядок элементов (in-order traversal)
			var expectedOrder = new[] { 1, 2, 3, 4, 5, 6, 7 };
			CollectionAssert.AreEqual(expectedOrder, result,
				"Elements should be returned in sorted order");
		}

	}
}

