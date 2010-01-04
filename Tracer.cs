using System;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace QTracer
{
   public class Tracer
   {
      private static Scene layout;

      public static Pixel[] start()
      {
         Pixel[] pixels = new Pixel[Constants.WIN_X * Constants.WIN_Y];
         Point3 viewPlane = new Point3(-Constants.WIN_X / 2, -Constants.WIN_X / 2, 0);

         createScene();

         for (int y = 0; y < Constants.WIN_Y; y++)
         {
            for (int x = 0; x < Constants.WIN_X; x++)
            {
               Pixel p = new Pixel();
               p.rect.X = x;
               p.rect.Y = Constants.WIN_Y - 1 - y;

               Point3 viewPixel = new Point3();
               viewPixel.x = viewPlane.x + x;
               viewPixel.y = viewPlane.y + y;
               viewPixel.z = viewPlane.z;

               Ray r = new Ray();
               r.direction = Point3.vectorize(layout.Cam.Eye, viewPixel);
               r.start = layout.Cam.Eye;

               p.color = fireRay(r, Constants.MIN_DEPTH, null);

               pixels[y * Constants.WIN_X + x] = p;
            }
         }

         return pixels;
      }

      private static Color3 fireRay(Ray incomingRay, int depth, Shape fromShape)
      {
         double closest = Constants.FAR_AWAY;
         Color3 retColor = Constants.BGCOLOR;
         int whichObject = 0, hitObject = 0;
         Hit finalHit = new Hit();

         foreach (Shape s in layout.Shapes)
         {
            Hit rayHit = s.intersect(incomingRay);
            whichObject++;

            if (rayHit.intersect.z == Constants.FAR_AWAY)
               continue;

            double dist = Point3.distance(layout.Cam.Eye, rayHit.intersect);

            if (dist < closest && !s.Equals(fromShape))
            {
               closest = dist;
               hitObject = whichObject;
               finalHit = rayHit;
            }
         }

         if (hitObject <= 0)
            return Constants.BGCOLOR;

         Shape hitShape = layout.Shapes[hitObject - 1];
         retColor = Color3.ambient(hitShape);

         // phongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphongphong
         Color3 diffuseColor = Color3.Black, specularColor = Color3.Black;
         foreach (Light bulb in layout.Lights)
         {
            if (spawnShadow(bulb, finalHit, hitShape, fromShape))
            {
               retColor = hitShape.colorShadow(retColor, finalHit, layout.Lights.Count * 3);
            }
            else
            {
               if (hitShape is Plane)
               {
                  retColor = hitShape.colorNormal(retColor, incomingRay, finalHit, hitShape, bulb);
               }
               else
               {
                  diffuseColor += Color3.diffuse(finalHit, hitShape, bulb);
                  specularColor += Color3.specular(incomingRay, finalHit, hitShape, bulb);
               }
            }
         }

         if (hitShape is Sphere)
         {
            retColor += diffuseColor * hitShape.Material.Kd;
            retColor += specularColor * hitShape.Material.Ks;
         }

         if (depth < Constants.MAX_DEPTH)
         {
            Color3 reflectColor = Color3.Black;

            if (hitShape.Material.Kr > 0)
            {
               Ray reflectRay = new Ray();
               reflectRay.start = finalHit.intersect;

               Vector3 normalVec = hitShape.calcNormal(finalHit);

               double c = -Vector3.DotProduct(normalVec, incomingRay.direction);
               reflectRay.direction = -(incomingRay.direction + (2 * normalVec * c));
               reflectRay.direction.Normalize();
               reflectColor = fireRay(reflectRay, depth + 1, hitShape);

               retColor += reflectColor * hitShape.Material.Kr;
            }

            if (hitShape.Material.Kt > 0)
            {
               Ray transRay = new Ray();
               double indexRefract;
               Vector3 normalVec = Vector3.FaceForward(hitShape.calcNormal(finalHit), -incomingRay.direction);

               if (Vector3.DotProduct(-incomingRay.direction, hitShape.calcNormal(finalHit)) < 0)
                  indexRefract = Constants.REFRACTION_INDEX_SPHERE / Constants.REFRACTION_INDEX_AIR;
               else
                  indexRefract = Constants.REFRACTION_INDEX_AIR / Constants.REFRACTION_INDEX_SPHERE;

               double discrim = 1 + (Math.Pow(indexRefract, 2) * (Math.Pow(Vector3.DotProduct(-incomingRay.direction, normalVec), 2) - 1));

               // Total internal reflection!
               if (discrim < 0)
               {
                  retColor += reflectColor * hitShape.Material.Kt;
               }
               else
               {
                  discrim = indexRefract * Vector3.DotProduct(-incomingRay.direction, normalVec) - Math.Sqrt(discrim);

                  transRay.direction = (indexRefract * incomingRay.direction) + (discrim * normalVec);
                  transRay.start = finalHit.intersect;

                  Color3 transColor = fireRay(transRay, depth + 1, hitShape);
                  retColor += transColor * hitShape.Material.Kt;
               }
            }
         }

         return retColor;
      }

      // spawn a shadow ray from the intersection point to the light source.
      private static bool spawnShadow(Light bulb, Hit finalHit, Shape hitShape, Shape fromShape)
      {
         Ray shadowRay = new Ray();
         shadowRay.start = finalHit.intersect;
         shadowRay.direction = Point3.vectorize(shadowRay.start, bulb.Location);
         double shadowDist = Point3.distance(shadowRay.start, bulb.Location);

         foreach (Shape s in layout.Shapes)
         {
            // If this is the object we're checking from, ignore. Duh.
            if (s.Equals(hitShape) || s.Equals(fromShape) || s.Material.Kt > 0)
               continue;

            Hit shadowHit = s.intersect(shadowRay);

            if (shadowHit.intersect.z == Constants.FAR_AWAY)
               continue;

            // We need to check if hitShape object is in FRONT OF the current one. if it is, we need to ignore this current shape.
            Vector3 frontTest = Point3.vectorize(shadowHit.intersect, finalHit.intersect);

            if (frontTest.X > 0 && frontTest.Y > 0 && frontTest.Z > 0)
               continue;

            // something has to come between the light and the current shape.
            double shapeDist = Point3.distance(shadowRay.start, shadowHit.intersect);
            if (shapeDist < shadowDist)
            {
               return true;
            }
         }

         return false;
      }

      private static void createScene()
      {
         XmlSerializer sceneSer = new XmlSerializer(typeof(Scene));

         try
         {
            using (FileStream xmlStream = File.OpenRead(Constants.SCENE))
            {
               layout = (Scene)sceneSer.Deserialize(xmlStream);
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            return;
         }

         foreach (SphereMaterial mat in layout.Spheres)
         {
            layout.Shapes.Add(new Sphere(mat));
         }

         foreach (PlaneMaterial mat in layout.Planes)
         {
            layout.Shapes.Add(new Plane(mat, Constants.FLOOR_SHADING));
         }

         layout.Cam.setup();
      }
   }
}
