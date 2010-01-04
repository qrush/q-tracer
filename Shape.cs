using System;
using System.Collections.Generic;

namespace QTracer
{
   public abstract class Shape
   {
      private Material _Mat;

      public abstract Hit intersect(Ray r);
      public abstract Vector3 calcNormal(Hit h);
      public abstract Color3 colorNormal(Color3 existing, Ray r, Hit h, Shape s, Light bulb);
      public abstract Color3 colorShadow(Color3 existing, Hit h, double factor);

      public Shape(Material mat)
      {
         _Mat = mat;
      }
      public Material Material
      {
         get { return _Mat; }
      }
   }

   public class Sphere : Shape
   {
      public Point3 center;
      public double radius;
      private Dictionary<Point3, Vector3> normals;

      public Sphere(SphereMaterial sp)
         : base(sp)
      {
         this.center = sp.Center;
         this.radius = sp.Radius;
         normals = new Dictionary<Point3, Vector3>();
      }

      public override Hit intersect(Ray r)
      {
         Hit h = new Hit();

         double b = 2 * (r.direction.X * (r.start.x - center.x) +
           r.direction.Y * (r.start.y - center.y) +
           r.direction.Z * (r.start.z - center.z));

         double c = Math.Pow(r.start.x - center.x, 2) +
            Math.Pow(r.start.y - center.y, 2) +
            Math.Pow(r.start.z - center.z, 2) -
            Math.Pow(radius, radius);

         double disc = Math.Pow(b, 2) - 4 * c;

         if (disc < 0)
         {
            h.intersect.z = Constants.FAR_AWAY;
         }
         else
         {
            h.omega = (-b + Math.Sqrt(disc)) / 2;
            h.intersect.x = r.start.x + r.direction.X * h.omega;
            h.intersect.y = r.start.y + r.direction.Y * h.omega;
            h.intersect.z = r.start.z + r.direction.Z * h.omega;
         }

         return h;
      }

      public override Vector3 calcNormal(Hit h)
      {
         Vector3 v;

         if (normals.TryGetValue(h.intersect, out v))
         {
            return v;
         }
         else
         {
            v.X = (h.intersect.x - center.x) / radius;
            v.Y = (h.intersect.y - center.y) / radius;
            v.Z = (h.intersect.z - center.z) / radius;
            v.Normalize();

            normals.Add(h.intersect, v);

            return v;
         }

      }

      public override Color3 colorNormal(Color3 existing, Ray r, Hit h, Shape s, Light bulb)
      {
         return Color3.Black;
      }

      public override Color3 colorShadow(Color3 existing, Hit h, double factor)
      {
         return Color3.shade(existing, factor);
      }
   }

   public enum ProcShading
   {
      Square,
      Circle,
      Texture
   }

   public class Plane : Shape
   {
      public ProcShading shader;
      public Vector3 normal;
      public double distance;

      public Plane(PlaneMaterial pp, ProcShading shader)
         : base(pp)
      {
         this.normal = pp.Normal;
         this.distance = pp.Distance;
         this.shader = shader;
      }

      public override Hit intersect(Ray r)
      {
         Hit h = new Hit();

         double denominator =
            Vector3.DotProduct(normal, r.direction);

         if (denominator == 0.0)
         {
            h.intersect.z = Constants.FAR_AWAY;
         }
         else
         {
            double numerator = normal.X * r.start.x +
               normal.Y * r.start.y +
               normal.Z * r.start.z +
               distance;

            h.omega = -numerator / denominator;

            if (h.omega < 0)
            {
               h.intersect.z = Constants.FAR_AWAY;
            }
            else
            {
               h.intersect.x = r.start.x + r.direction.X * h.omega;
               h.intersect.y = r.start.y + r.direction.Y * h.omega;
               h.intersect.z = r.start.z + r.direction.Z * h.omega;

               // Boundary hit testing.
               if (h.intersect.x > Constants.RIGHT_PLANE_BOUNDARY || h.intersect.x < Constants.LEFT_PLANE_BOUNDARY)
               {
                  h.intersect.x = h.intersect.y = h.intersect.z = Constants.FAR_AWAY;
               }
            }

         }


         return h;
      }

      public override Vector3 calcNormal(Hit h)
      {
         return normal;
      }

      private Color3 mapSquare(Hit h)
      {
         Color3 shadeColor;
         int row = (int)Math.Round(h.intersect.x / Constants.CHECKSIZE);
         int col = (int)Math.Round(h.intersect.z / Constants.CHECKSIZE);

         if (row % 2 == 0 && col % 2 == 0)
            shadeColor = Color3.Red;
         else if (row % 2 == 0)
            shadeColor = Color3.Yellow;
         else if (col % 2 == 0)
            shadeColor = Color3.Yellow;
         else
            shadeColor = Color3.Red;

         return shadeColor;
      }

      private Color3 mapCircle(Hit h)
      {
         Color3 shadeColor;
         const double SPHERESIZE = Constants.SPHERESIZE;

         int row = (int)Math.Round(h.intersect.x / SPHERESIZE);
         int col = (int)Math.Round(h.intersect.z / SPHERESIZE);

         double centerX = row * SPHERESIZE + SPHERESIZE / 4;
         double centerY = col * SPHERESIZE + SPHERESIZE / 4;
         double r = SPHERESIZE / 4;

         if (h.intersect.x <= centerX + r && h.intersect.x >= centerX - r &&
           h.intersect.z <= centerY + r && h.intersect.z >= centerY - r)
         {
            double circle = Math.Pow(h.intersect.x - centerX, 2) + Math.Pow(h.intersect.z - centerY, 2);

            if (circle <= Math.Pow(r, 2))
               shadeColor = Color3.Yellow;
            else
               shadeColor = Color3.Red;
         }
         else
         {
            shadeColor = Color3.Red;
         }

         return shadeColor;
      }

      private Color3 mapTexture(Hit h)
      {
         Color3 shadeColor = Color3.Black;

         int texWidth = TextureLoader.Texture.Width;
         int texHeight = TextureLoader.Texture.Height;
         
         int row = (int)Math.Round(h.intersect.x / texWidth);
         int col = (int)Math.Round(h.intersect.z / texHeight);

         int startX = row * texWidth;
         int startY = col * texHeight;


         int texX = (int) Math.Round(Math.Abs(h.intersect.x - startX));
         int texY = (int) Math.Round(Math.Abs(h.intersect.z - startY));

         if (texX < TextureLoader.Texture.Width && texY < TextureLoader.Texture.Height)
         {
            System.Drawing.Color texColor = TextureLoader.Texture.GetPixel(texX, texY);
            shadeColor = new Color3(texColor.R, texColor.G, texColor.B);
         }


         return shadeColor;
      }

      public override Color3 colorNormal(Color3 existing, Ray r, Hit h, Shape s, Light bulb)
      {
         switch (shader)
         {
            case ProcShading.Circle:
               return mapCircle(h);

            case ProcShading.Texture:
               return mapTexture(h);

            default:
               return mapSquare(h);
         }
      }

      public override Color3 colorShadow(Color3 existing, Hit h, double factor)
      {
         switch (shader)
         {
            case ProcShading.Circle:
               return Color3.shade(mapCircle(h), factor);

            case ProcShading.Texture:
               return Color3.shade(mapTexture(h), factor);

            default:
               return Color3.shade(mapSquare(h), factor);
         }
      }
   }
}