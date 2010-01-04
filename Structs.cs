using System;
using System.Collections.Generic;
using System.Text;

namespace QTracer
{
   public struct Ray
   {
      public Point3 start;
      public Vector3 direction;
   }

   public struct Hit
   {
      public Point3 intersect;
      public int which;
      public double omega;
   }

   public class Pixel
   {
      public System.Drawing.Rectangle rect;
      public Color3 color = Color3.Black;

      public Pixel()
      {
         rect = new System.Drawing.Rectangle(0, 0, 1, 1);
         color = Color3.Black;
      }
   }

   public class Light
   {
      private Point3 _Location;
      private Color3 _Color;

      public Light()
      {
      }

      public Light(Point3 location, Color3 color)
      {
         this._Location = location;
         this._Color = color;
      }

      public Point3 Location
      {
         get { return _Location; }
         set { _Location = value; }
      }

      public Color3 Color
      {
         get { return _Color; }
         set { _Color = value; }
      }
   }  
}
