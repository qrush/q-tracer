using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace QTracer
{
   public static class TextureLoader
   {
      private static Bitmap texture;
 
      static TextureLoader()
      {
         texture = new Bitmap(Constants.TEXTURE);
      }

      public static Bitmap Texture
      {
         get { return texture; }
      }
   }
}
