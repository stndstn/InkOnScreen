using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ink;

namespace InkOnScreen
{
    public partial class Palette : Form
    {
        //public bool isClosing = false;
        public Palette()
        {
            InitializeComponent();
        }

        private void Palette_Load(object sender, EventArgs e)
        {

        }

        private void Palette_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("Palette_FormClosed");
            //isClosing = false;
            /*
            if (isClosing == false)
            {
                isClosing = true;
                this.Owner.Close();
            }
             */
        }

        private void Palette_VisibleChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_VisibleChanged");
            Form1 fm = (Form1)this.Owner;

            if (this.Visible == false && fm.mStopCopyDesktopImage == false)
            {
                //isClosing = true;
            }
            /*
            if (this.Visible)
            {
                this.Owner.Show();
            }
            else
            {
                this.Owner.Hide();
            }
             * */
        }

        private void Palette_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_Activated");
            //Form1 fm = (Form1)this.Owner;
            /*
            if (fm.isCopyingDesktopImage == false)
            {
                fm.CopyDesktopImage();
            }
             */
            this.Owner.WindowState = FormWindowState.Maximized;
        }

        private void Palette_Deactivate(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_Deactivate");
            //this.Owner.Hide();
        }

        private void panelColorRed_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Red;
            ActivatePaintingScreen();
        }

        private void panelColorBlue_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Blue;
            ActivatePaintingScreen();
        }

        private void panelColorYellow_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Yellow;
            ActivatePaintingScreen();
        }

        private void panelColorGreen_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Lime;
            ActivatePaintingScreen();
        }

        private void panelColorWhite_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.White;
            ActivatePaintingScreen();
        }

        private void panelColorBlack_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Black;
            ActivatePaintingScreen();
        }

        private void Palette_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Palette_FormClosing");
            if (this.Owner.WindowState != FormWindowState.Minimized)
            {
                this.Owner.WindowState = FormWindowState.Minimized;
                //e.Cancel = true;
                //this.Hide();
            }
        }

        private void pictSave_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.SaveImage();
            ActivatePaintingScreen();
        }

        private void pictCopy_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.CopyToClipboard();
            ActivatePaintingScreen();
        }

        private void pictSelect_Click(object sender, EventArgs e)
        {
            SelDesktopRectScreen s = new SelDesktopRectScreen();
            s.ShowDialog();
            Form1 fm = (Form1)this.Owner;
            fm.mRcSelected = s.SelectedRect;
        }

        private void pictDelete_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.Clear();
            ActivatePaintingScreen();
        }

        private void ActivatePaintingScreen()
        {
            Form1 fm = (Form1)this.Owner;
            fm.mStopCopyDesktopImage = true;
            fm.Activate();
            fm.mStopCopyDesktopImage = false;
        }

        private void pictPenBallS_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 100;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 100;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            ActivatePaintingScreen();
        }

        private void pictPenBallL_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 200;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 200;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            ActivatePaintingScreen();
        }

        private void pictPenRectH_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 200;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 100;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            ActivatePaintingScreen();
        }

        private void pictPenRectV_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 100;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 200;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            ActivatePaintingScreen();
        }

    }
}
