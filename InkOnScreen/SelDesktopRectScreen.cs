using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InkOnScreen
{
    public partial class SelDesktopRectScreen : Form
    {
        Point mPtStart = Point.Empty;
        Rectangle mRc = Rectangle.Empty;
        public Rectangle SelectedRect
        {
            get { return mRc; }
        }
        /*
        Bitmap mScreenShotBMP = null;
        public Bitmap ScreenShotBmp
        {
            get { return mScreenShotBMP; }
        }
        */
        public SelDesktopRectScreen()
        {
            InitializeComponent();
        }

        private void DesktopScreen_MouseDown(object sender, MouseEventArgs e)
        {
            mPtStart = e.Location;
        }

        private void DesktopScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (mPtStart != Point.Empty)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Size s = new System.Drawing.Size(Math.Abs(mPtStart.X - e.X), Math.Abs(mPtStart.Y - e.Y));
                    Rectangle rc = new Rectangle(mPtStart, s);
                    Control c = (Control)sender;
                    c.Invalidate();
                    c.Update();
                    Graphics g = c.CreateGraphics();
                    Pen p = new Pen(Color.Blue);
                    g.DrawRectangle(p, rc);
                    g.Dispose();
                }
            }
        }

        private void DesktopScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (mPtStart != null)
            {
                this.Hide();
                Size s = new System.Drawing.Size(Math.Abs(mPtStart.X - e.X), Math.Abs(mPtStart.Y - e.Y));
                mRc = new Rectangle(mPtStart, s);
                /*
                mScreenShotBMP = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
                Graphics screenShotGraphics = Graphics.FromImage(mScreenShotBMP);
                screenShotGraphics.CopyFromScreen(mPtStart.X, mPtStart.Y, 0, 0, s, CopyPixelOperation.SourceCopy);
                screenShotGraphics.Dispose();
                 */
            }
            this.Close();
        }

        private void DesktopScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            mPtStart = Point.Empty;
            this.Close();
        }
    }
}
