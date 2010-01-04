using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;

namespace QTracer
{
   public class Window : Form
   {
      private Pixel[] pixels;

      public Window()
      {
         this.SuspendLayout();
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(Constants.WIN_X, Constants.WIN_Y);
         this.Text = "Q-Tracer Checkpoint 6";
         this.Icon = QTracer.Properties.Resources.tophat; 
         this.Load += new System.EventHandler(this.Form_Load);
         this.KeyDown += new KeyEventHandler(Window_KeyDown);
         this.ResumeLayout(false);
      }

      void Window_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Q || e.KeyCode == Keys.Escape)
         {
            Application.Exit();
         }
      }

      private void Form_Load(object sender, EventArgs e)
      {
         pixels = Tracer.start(); 
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         Graphics g = e.Graphics;

         for (int y = 0; y < Constants.WIN_Y; y++)
         {
            for (int x = 0; x < Constants.WIN_X; x++)
            {
               Pixel p = pixels[y * Constants.WIN_X + x];
               
               g.FillRectangle(new SolidBrush(p.color.toColor()), p.rect);
            }
         }
      }
   }
}