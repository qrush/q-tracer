using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QTracer
{
   [Serializable]
   public class Camera
   {
      public Point3 Eye;
      public Point3 Lookat;
      public Vector3 Orient;

      private Vector3 _View, _Up, _Normal;

      public void setup()
      {
         _Normal = Point3.vectorize(Lookat, Eye);
         _Up = Vector3.CrossProduct(Orient, Normal);
         _View = Vector3.CrossProduct(Normal, Up);
      }

      [XmlIgnore]
      public Vector3 Normal
      {
         get { return _Normal; }
      }

      [XmlIgnore]
      public Vector3 Up
      {
         get { return _Up; }
      }

      [XmlIgnore]
      public Vector3 View
      {
         get { return _View; }
      }
   }
}
