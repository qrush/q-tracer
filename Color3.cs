using System;
using System.Collections.Generic;

namespace QTracer
{
   public struct Color3
   {
      private double r, g, b;
      public const int MAX = 255;

      #region "  Instance Methods  "
      public Color3(double r, double g, double b)
      {
         this.r = r;
         this.g = g;
         this.b = b;
      }

      public double R
      {
         get { return r; }
         set { r = value; }
      }

      public double G
      {
         get { return g; }
         set { g = value; }
      }

      public double B
      {
         get { return b; }
         set { b = value; }
      }

      public System.Drawing.Color toColor()
      {
         System.Drawing.Color c;
         try
         {
            c = System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
         }
         catch (Exception)
         {
            c = System.Drawing.Color.Black;
         }
         return c;
      }

      public override bool Equals(object obj)
      {
         if (obj is Color3)
         {
            Color3 other = (Color3)obj;

            if (other.R == this.R && other.B == this.B && other.G == this.G)
               return true;
            else
               return false;
         }
         else
            return false;
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public bool channelMaxed()
      {
         if (this.R == MAX || this.G == MAX || this.B == MAX)
            return true;
         else
            return false;
      }

      public void ScaleAndCheck()
      {
         r *= MAX;
         g *= MAX;
         b *= MAX;

         if (r > MAX)
            r = MAX;
         if (g > MAX)
            g = MAX;
         if (b > MAX)
            b = MAX;
      }

      #endregion

      #region "   Operators   "


      public static Color3 operator +(Color3 c1, Color3 c2)
      {
         double r = ((c1.R / MAX) + (c2.R / MAX));
         double g = ((c1.G / MAX) + (c2.G / MAX));
         double b = ((c1.B / MAX) + (c2.B / MAX));

         Color3 c = new Color3(r, b, g);
         c.ScaleAndCheck();

         return c;
      }

      public static Color3 operator *(Color3 c1, Color3 c2)
      {
         double r = ((c1.R / MAX) * (c2.R / MAX));
         double g = ((c1.G / MAX) * (c2.G / MAX));
         double b = ((c1.B / MAX) * (c2.B / MAX));

         Color3 c = new Color3(r, b, g);
         c.ScaleAndCheck();

         return c;
      }

      public static Color3 operator *(Color3 c1, double coef)
      {
         double r = ((c1.R / MAX) * (coef));
         double g = ((c1.G / MAX) * (coef));
         double b = ((c1.B / MAX) * (coef));

         Color3 c = new Color3(r, b, g);
         c.ScaleAndCheck();

         return c;
      }

      #endregion

      #region "  Static Methods   "

      public static Color3 shade(Color3 existing, double factor)
      {
         Color3 shade = Black;

         shade.R = existing.R / factor;
         shade.G = existing.G / factor;
         shade.B = existing.B / factor;

         return shade;
      }

      public static Color3 ambient(Shape s)
      {
         Color3 ambient = Black;
         Material m = s.Material;

         // ambient 
         ambient.R = ((Constants.AMBIENT.R / MAX) * (m.Ambient.R / MAX)) * m.Ka;
         ambient.G = ((Constants.AMBIENT.G / MAX) * (m.Ambient.G / MAX)) * m.Ka;
         ambient.B = ((Constants.AMBIENT.B / MAX) * (m.Ambient.B / MAX)) * m.Ka;

         return ambient;
      }

      public static Color3 diffuse(Hit h, Shape s, Light bulb)
      {
         Color3 diffuse = Black;
         Vector3 normal = s.calcNormal(h);
         Vector3 lightDir = Point3.vectorize(bulb.Location, h.intersect);
         double diffuseDot = Vector3.DotProduct(lightDir, normal);

         if (diffuseDot >= 0)
            diffuse = (bulb.Color * s.Material.Diffuse) * diffuseDot;

         return diffuse;
      }

      public static Color3 specular(Ray r, Hit h, Shape s, Light bulb)
      {
         Color3 specular = Black;
         Vector3 normal = s.calcNormal(h);

         Vector3 lightDir = Point3.vectorize(bulb.Location, h.intersect);
         Vector3 refDir = lightDir - (2 * (Vector3.DotProduct(lightDir, normal) / Math.Pow(normal.Abs(), 2)) * normal);
         double specularDot = Vector3.DotProduct(r.direction, refDir);

         if (specularDot >= 0)
            specular = (bulb.Color * s.Material.Specular) * Math.Pow(specularDot, s.Material.Ke);

         return specular;
      }

      public static Color3 floor(Color3 existing, Hit h, Plane p)
      {
         return Color3.Green;
      }

      #endregion

      #region "  Named Colors  "

      public static Color3 Black
      {
         get { return new Color3(0, 0, 0); }
      }
      public static Color3 White
      {
         get { return new Color3(255, 255, 255); }
      }
      public static Color3 Red
      {
         get { return new Color3(255, 0, 0); }
      }
      public static Color3 Green
      {
         get { return new Color3(0, 255, 0); }
      }
      public static Color3 Blue
      {
         get { return new Color3(0, 0, 255); }
      }
      public static Color3 Yellow
      {
         get { return new Color3(255, 255, 0); }
      }
      public static Color3 Orange
      {
         get { return new Color3(255, 69, 0); }
      }

      #endregion

   }
}
