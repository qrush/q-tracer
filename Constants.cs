using System;
using System.IO;

namespace QTracer
{
   public struct Constants
   {
      public const int WIN_X = 800;
      public const int WIN_Y = 600;
      public const int MIN_DEPTH = 1;
      public const int MAX_DEPTH = 20;

      public const double FAR_AWAY = 10000;
      public const double LEFT_PLANE_BOUNDARY = -500;
      public const double RIGHT_PLANE_BOUNDARY = 170;
      public const double REFRACTION_INDEX_SPHERE = 0.95;
      public const double REFRACTION_INDEX_AIR = 1;
      public const double CHECKSIZE = 70;
      public const double SPHERESIZE = 150;

      public static readonly Color3 BGCOLOR = Color3.Blue;
      public static readonly Color3 AMBIENT = Color3.White;

      public static readonly string SCENE = Path.Combine(Environment.CurrentDirectory, "scene.xml");
      public static readonly string TEXTURE = Path.Combine(Environment.CurrentDirectory, "texture.jpg");

      public const ProcShading FLOOR_SHADING = ProcShading.Square;
   }
}
