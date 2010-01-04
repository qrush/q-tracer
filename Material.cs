using System;

namespace QTracer
{
   [Serializable]
   public abstract class Material
   {
      private Color3 _Ambient, _Diffuse, _Specular;
      private double _Ka, _Kd, _Ks, _Ke, _Kr, _Kt;

      public Color3 Ambient
      {
         get { return _Ambient; }
         set { _Ambient = value; }
      }
      public Color3 Diffuse
      {
         get { return _Diffuse; }
         set { _Diffuse = value; }
      }
      public Color3 Specular
      {
         get { return _Specular; }
         set { _Specular = value; }
      }
      public double Ka
      {
         get { return _Ka; }
         set { _Ka = value; }
      }
      public double Kd
      {
         get { return _Kd; }
         set { _Kd = value; }
      }
      public double Ks
      {
         get { return _Ks; }
         set { _Ks = value; }
      }
      public double Ke
      {
         get { return _Ke; }
         set { _Ke = value; }
      }
      public double Kt
      {
         get { return _Kt; }
         set { _Kt = value; }
      }
      public double Kr
      {
         get { return _Kr; }
         set { _Kr = value; }
      }
   }

   [Serializable()]
   public class SphereMaterial : Material
   {
      private Point3 _Center;
      private double _Radius;

      public Point3 Center
      {
         get { return _Center; }
         set { _Center = value; }
      }
      public double Radius
      {
         get { return _Radius; }
         set { _Radius = value; }
      }
   }

   [Serializable()]
   public class PlaneMaterial : Material
   {
      private Vector3 _Normal;
      private double _Distance;

      public Vector3 Normal
      {
         get { return _Normal; }
         set { _Normal = value; }
      }
      public double Distance
      {
         get { return _Distance; }
         set { _Distance = value; } 
      }
   }
}
