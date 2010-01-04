using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace QTracer
{
   [Serializable]
   public class Scene
   {
      public Camera Cam;
      public List<SphereMaterial> Spheres;
      public List<PlaneMaterial> Planes;
      public List<Light> Lights;

      [XmlIgnore]
      public List<Shape> Shapes;

      public Scene()
      {
         Spheres = new List<SphereMaterial>();
         Planes = new List<PlaneMaterial>();
         Lights = new List<Light>();
         Shapes = new List<Shape>();
         Cam = new Camera();
      }
   }
}
