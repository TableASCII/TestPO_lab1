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
		public void Insert_Called_Once_With_Expected_Value()
		{
			var mock = new Mock<IRbTree<int>>();

			mock.Object.Insert(42);

			mock.Verify(tree => tree.Insert(42), Times.Once);
		}

		[Test]
		public void Select_Returns_Expected_Filtered_Values()
		{
			var mock = new Mock<IRbTree<int>>();
			mock.Setup(tree => tree.Select(It.IsAny<Func<int, bool>>()))
				.Returns<Func<int, bool>>(predicate => new List<int> { 1, 2, 3, 4, 5 }.Where(predicate));

			var result = mock.Object.Select(x => x > 2).ToList();

			Assert.AreEqual(new List<int> { 3, 4, 5 }, result);
		}

		[Test]
		public void GetLeftChildValue_Returns_Expected_Value()
		{
			var mock = new Mock<IRbTree<string>>();
			mock.Setup(tree => tree.GetLeftChildValue("parent")).Returns("left-child");

			var result = mock.Object.GetLeftChildValue("parent");

			Assert.AreEqual("left-child", result);
		}

		[Test]
		public void GetRightChildValue_Throws_Exception_When_Not_Found()
		{
			var mock = new Mock<IRbTree<int>>();
			mock.Setup(tree => tree.GetRightChildValue(99)).Throws(new KeyNotFoundException("Right child not found"));

			Assert.Throws<KeyNotFoundException>(() => mock.Object.GetRightChildValue(99));
		}
	}
}
