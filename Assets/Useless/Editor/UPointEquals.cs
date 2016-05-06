using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Useless.Match3;

using Useless;
public class UPointEquals {

    [Test]
    public void Equals()
    {

        UPoint pt1 = new UPoint(1, 2);

        UPoint pt2 = new UPoint(1, 2);

        //Assert
        //The object has a new name
        if(pt1 == pt2)
            Assert.Pass();
    }

    [Test]
    public void NotEquals()
    {

        UPoint pt1 = new UPoint(1, 2);

        UPoint pt2 = new UPoint(2, 2);

        //Assert
        //The object has a new name
        if (pt1 != pt2)
            Assert.Pass();
    }

    [Test]
    public void MoveEquals()
    {

        UPoint pt1 = new UPoint(1, 2);
        UPoint pt2 = new UPoint(2, 2);

        Match3.Move move1 = new Match3.Move(pt1, pt2);

        UPoint pt3 = new UPoint(2, 2);
        UPoint pt4 = new UPoint(1, 2);

        Match3.Move move2 = new Match3.Move(pt3, pt4);

        if (move1 == move2)
            Assert.Pass();
    }

    [Test]
    public void MoveNotEquals()
    {

        UPoint pt1 = new UPoint(1, 2);
        UPoint pt2 = new UPoint(2, 2);

        Match3.Move move1 = new Match3.Move(pt1, pt2);

        UPoint pt3 = new UPoint(1, 2);
        UPoint pt4 = new UPoint(10, 10);

        Match3.Move move2 = new Match3.Move(pt3, pt4);

        if (move1 != move2)
            Assert.Pass();
    }
}
