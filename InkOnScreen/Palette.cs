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
        }

        private void Palette_VisibleChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_VisibleChanged");
        }

        private void Palette_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_Activated");
            this.Owner.Visible = true;
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
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlue_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Blue;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorYellow_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Yellow;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorGreen_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Lime;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorWhite_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.White;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlack_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Color = Color.Black;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void Palette_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Palette_FormClosing");
            //if (this.Owner.WindowState != FormWindowState.Minimized)
            if (this.Owner.Visible == true)
            {
                this.Owner.Visible = false;
                //this.Owner.WindowState = FormWindowState.Minimized;
                //e.Cancel = true;
                //this.Hide();
            }
        }

        private void pictSave_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.SaveImage();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictCopy_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.CopyToClipboard();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictSelect_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            Rectangle rc = fm.mRcSelected; // backup because it will be emptied after CopyDesktopImage() was called.
            if (rc != Rectangle.Empty)
            {
                if (fm.mUseDesktopImgAsBG)
                {
                    fm.CopyDesktopImage();
                    fm.mRcSelected = rc;
                }
            }
            SelDesktopRectScreen s = new SelDesktopRectScreen();
            s.ShowDialog();
            fm.mRcSelected = s.SelectedRect;
            fm.DrawSelRect();
        }

        private void pictDelete_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.Clear();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void ActivatePaintingScreen()
        {
            Form1 fm = (Form1)this.Owner;
            fm.mStopRefreshImage = true;
            fm.Activate();
            fm.mStopRefreshImage = false;
        }

        private void pictPenBallS_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 100;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 100;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenBallL_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 200;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 200;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenRectH_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 200;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 100;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenRectV_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.DefaultDrawingAttributes.Width = 100;
            fm.mInkPicture.DefaultDrawingAttributes.Height = 200;
            fm.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictGrid_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            if (fm.mHasGrid)
            {
                fm.mHasGrid = false;
                fm.Activate();
            }
            else
            {
                fm.mHasGrid = true;
                fm.DrawGrid();
                ActivatePaintingScreen();
                /*
                fm.mStopCopyDesktopImage = true;
                fm.DrawGrid();
                fm.Activate();
                fm.mStopCopyDesktopImage = false;
                 */
            }
        }

        private void Palette_KeyDown(object sender, KeyEventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            if (e.Control && (e.KeyCode == Keys.C))
            {
                fm.CopyToClipboard();
            }
            else if (e.Control && (e.KeyCode == Keys.S))
            {
                fm.SaveImage();
            }
        }

        private void pictUndo_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.Undo();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictRefreshBG_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mUseDesktopImgAsBG = true;
            fm.CopyDesktopImage();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictErase_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
            fm.mInkPicture.EraserMode = InkOverlayEraserMode.PointErase;
            ActivatePaintingScreen();
        }

        private void restoreInkMode()
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.EditingMode = InkOverlayEditingMode.Ink;
        }

        private void pictPick_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.EditingMode = InkOverlayEditingMode.Select;
        }

        private void pictCleaner_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
            fm.mInkPicture.EraserMode = InkOverlayEraserMode.StrokeErase;
        }

        private void pictNew_Click(object sender, EventArgs e)
        {
            Form1 fm = (Form1)this.Owner;
            fm.mUseDesktopImgAsBG = false;
            fm.NewEmptyImage();
            ActivatePaintingScreen();            
        }

    }
}
