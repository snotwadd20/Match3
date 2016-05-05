using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Useless
{
    //Point - Only deals with integers, not floats. Use Vector2 for floats.
    //This is compatible with Vector2, and will drop the decimals if any are
    //present.

    [Serializable]
    public class UPoint : IEquatable<UPoint>
    {
        public int x;
        public int y;
       
        //------------------------
        //CONSTRUCTORS
        //------------------------
        public UPoint(float x, float y)
        {
            this.x = Mathf.RoundToInt(x);
            this.y = Mathf.RoundToInt(y); 
        }//constructor

        public UPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }//constructor

        public UPoint(UPoint point)
        {
            this.x = point.x;
            this.y = point.y;
        }//constructor

        public UPoint(Vector2 vector)
        {
            this.x = Mathf.RoundToInt(vector.x);
            this.y = Mathf.RoundToInt(vector.y);
        }//constructor

        public UPoint(Vector3 vector)
        {
            this.x = Mathf.RoundToInt(vector.x);
            this.y = Mathf.RoundToInt(vector.y);
        }//constructor

        //------------------------
        //EQUATING
        //------------------------
        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;

            UPoint other = (UPoint)obj;
            if (other == null)
                return false;

            return other.x == this.x && other.y == this.y;
        }

        public bool Equals(UPoint other)
        {
            if (other == null)
                return false;

            return other.x == this.x && other.y == this.y;
        }//Equals

        public override int GetHashCode()
        {
            return (int)x ^ (int)y;
        }

        //------------------------
        //OPERATORS
        //------------------------
        static public implicit operator UPoint(Vector2 vector)
        {
            return new UPoint(vector);
        }//static

        static public implicit operator UPoint(Vector3 vector)
        {
            return new UPoint(vector);
        }//static

        static public implicit operator Vector2(UPoint point)
        {
            return new Vector2(point.x, point.y);
        }//static

        static public implicit operator Vector3(UPoint point)
        {
            return new Vector3(point.x, point.y);
        }//static

        static public UPoint operator *(UPoint point1, UPoint point2)
        {
            return new UPoint(point1.x * point2.x, point1.y * point2.y);
        }//*

        static public UPoint operator *(UPoint point1, int scalar)
        {
            return new UPoint(point1.x * scalar, point1.y * scalar);
        }//*

        static public UPoint operator +(UPoint point1, UPoint point2)
        {
            return new UPoint(point1.x + point2.x, point1.y + point2.y);
        }//+

        static public UPoint operator +(UPoint point1, Vector2 point2)
        {
            return new UPoint(point1.x + point2.x, point1.y + point2.y);
        }//+

        static public UPoint operator +(Vector2 point1, UPoint point2)
        {
            return new UPoint(point1.x + point2.x, point1.y + point2.y);
        }//+

        static public UPoint operator +(UPoint point1, Vector3 point2)
        {
            return new UPoint(point1.x + point2.x, point1.y + point2.y);
        }//+

        static public UPoint operator +(Vector3 point1, UPoint point2)
        {
            return new UPoint(point1.x + point2.x, point1.y + point2.y);
        }//+

        static public UPoint operator -(UPoint point1, UPoint point2)
        {
            return new UPoint(point1.x - point2.x, point1.y - point2.y);
        }//-

        static public UPoint operator -(UPoint point1, Vector2 point2)
        {
            return new UPoint(point1.x - point2.x, point1.y - point2.y);
        }//-

        //------------------------
        //TOSTRING
        //------------------------
        public override string ToString()
        {
            return "[" + x + "," + y + "]";
        }//ToString
    }//UPoint
}//namespace