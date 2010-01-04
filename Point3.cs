using System;
using System.Collections.Generic;
using System.Text;

namespace QTracer
{
   public struct Point3
   {
      public double x, y, z;

      public Point3(double x, double y, double z)
      {
         this.x = x;
         this.y = y;
         this.z = z;
      }

      #region "   Static Methods   "

      public static double distance(Point3 p1, Point3 p2)
      {
         return Math.Sqrt(
            Math.Pow(p1.x - p2.x, 2) +
            Math.Pow(p1.y - p2.y, 2) +
            Math.Pow(p1.z - p2.z, 2)
         );
      }

      public static Vector3 vectorize(Point3 p1, Point3 p2)
      {
         Vector3 v = new Vector3();

         v.X = p2.x - p1.x;
         v.Y = p2.y - p1.y;
         v.Z = p2.z - p1.z;
         v.Normalize();

         return v;
      }
      #endregion

      #region "   Equality   "

      public static bool operator !=(Point3 p1, Point3 p2)
      {
         if (p1.x != p2.x || p1.y != p2.y || p1.z != p2.z)
            return true;
         else
            return false;
      }

      public static bool operator ==(Point3 p1, Point3 p2)
      {
         if (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z)
            return true;
         else
            return false;
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      #endregion
   }
}
