using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ink;
using System.Runtime.InteropServices;


namespace InkOnScreen
{
    public partial class Form1 : Form
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest,
            int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
            int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
            int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobjBmp);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public InkPicture mInkPicture = null;
        public bool mStopRefreshImage = false;
        public Rectangle mRcSelected = Rectangle.Empty;
        public bool mHasGrid = false;
        public bool mUseDesktopImgAsBG = false;
        NotifyIcon mNotifyIcon = null;
        Palette mPaletteDlg = null;
        Bitmap mBmpBGImg = null;
        private bool mIsClosing = false;

        /*
         * To-do
         *   Erase
         */
        public Form1()
        {
            InitializeComponent();

            mInkPicture = new InkPicture();
            this.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
            //mInkPicture.Width = this.ClientSize.Width;
            //mInkPicture.Height = this.ClientSize.Height;

            mInkPicture.DefaultDrawingAttributes.Color = Color.Black;
            mInkPicture.DefaultDrawingAttributes.Width = 100;
            mInkPicture.DefaultDrawingAttributes.Height = 100;
            mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;

            //mInkPicture.Stroke += new InkCollectorStrokeEventHandler(InkPicture_Stroke);
            //ShowPalette();
            
            // Create the NotifyIcon.
            this.components = new System.ComponentModel.Container();
            this.mNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            // The Icon property sets the icon that will appear
            // in the systray for this application.
            mNotifyIcon.Icon = global::InkOnScreen.Properties.Resources.Icon1;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            mNotifyIcon.ContextMenuStrip = this.contextMenuStrip1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            mNotifyIcon.Text = "Ink On Screen";
            mNotifyIcon.Visible = true;

            mNotifyIcon.MouseClick += new MouseEventHandler(NotifyIcon_MouseClick);

        }

        void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //this.Activate();
                ShowPalette();
            }
        }

        /*
        void NotifyIcon_Click(object sender, EventArgs e)
        {
            System.Drawing.Size windowSize =
                SystemInformation.PrimaryMonitorMaximizedWindowSize;
            System.Drawing.Point menuPoint =
                new System.Drawing.Point(windowSize.Width - 180,
                windowSize.Height - 5);
            menuPoint = this.PointToClient(menuPoint);
            mNotifyIcon.ContextMenuStrip.Show(this, menuPoint);
            
        }
         */
        /*
        void InkPicture_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            throw new NotImplementedException();
        }
        */
        public void Undo()
        {
            int count = mInkPicture.Ink.Strokes.Count;
            if (count > 0)
            {
                Stroke lastStroke = mInkPicture.Ink.Strokes[count - 1];
                mInkPicture.Ink.DeleteStroke(lastStroke);
                mInkPicture.Refresh();
            }
        }
        public void CopyDesktopImage()
        {
            Console.WriteLine("CopyDesktopImage");
            mStopRefreshImage = true;

            this.Visible = false;
            //this.WindowState = FormWindowState.Minimized;
            HidePalette();

            int screenX;
            int screenY;
            IntPtr hBmp;
            IntPtr hdcScreen = GetDC(GetDesktopWindow());
            IntPtr hdcCompatible = CreateCompatibleDC(hdcScreen);

            screenX = GetSystemMetrics(0);
            screenY = GetSystemMetrics(1);
            hBmp = CreateCompatibleBitmap(hdcScreen, screenX, screenY);

            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = (IntPtr)SelectObject(hdcCompatible, hBmp);
                BitBlt(hdcCompatible, 0, 0, screenX, screenY, hdcScreen, 0, 0, 13369376);

                SelectObject(hdcCompatible, hOldBmp);
                DeleteDC(hdcCompatible);
                ReleaseDC(GetDesktopWindow(), hdcScreen);

                if (mBmpBGImg != null) mBmpBGImg.Dispose();

                mBmpBGImg = System.Drawing.Image.FromHbitmap(hBmp);

                DeleteObject(hBmp);
                GC.Collect();
                
                //draw grid
                if (mHasGrid)
                {
                    DrawGrid(mBmpBGImg);
                }
                mInkPicture.BackgroundImage = mBmpBGImg;
            }
            this.Visible = true;
            //this.WindowState = FormWindowState.Maximized;
            ShowPalette();
            mStopRefreshImage = false;
        }
        public void NewEmptyImage()
        {
            Console.WriteLine("NewEmptyImage");
            if (mBmpBGImg != null) mBmpBGImg.Dispose();
            mBmpBGImg = new Bitmap(mInkPicture.ClientSize.Width, mInkPicture.ClientSize.Height);
            Graphics g = Graphics.FromImage(mBmpBGImg);
            Brush whiteBrush = new SolidBrush(Color.White);
            g.FillRectangle(whiteBrush, mInkPicture.ClientRectangle);
            whiteBrush.Dispose();
            g.Dispose();
            mInkPicture.BackgroundImage = mBmpBGImg;
            mInkPicture.Refresh();
        }

        public void DrawSelRect()
        {
            //Graphics g = Graphics.FromImage(mInkPicture.BackgroundImage);
            //Pen pen = new Pen(Color.Blue);
            //g.DrawRectangle(pen, this.mRcSelected);
            //mInkPicture.Refresh();
            //pen.Dispose();

            // draw xor rect
            Color colorOrg = Color.Empty;
            Color colorXor = Color.Empty;
            for (int x = mRcSelected.Left; x < mRcSelected.Right; x++)
            {
                colorOrg = mBmpBGImg.GetPixel(x, mRcSelected.Top);
                colorXor = Color.FromArgb(255 - colorOrg.A, 255 - colorOrg.R, 255 - colorOrg.G, 255 - colorOrg.B);
                mBmpBGImg.SetPixel(x, mRcSelected.Top, colorXor);
            }
            for (int x = mRcSelected.Left; x < mRcSelected.Right; x++)
            {
                colorOrg = mBmpBGImg.GetPixel(x, mRcSelected.Bottom);
                colorXor = Color.FromArgb(255 - colorOrg.A, 255 - colorOrg.R, 255 - colorOrg.G, 255 - colorOrg.B);
                mBmpBGImg.SetPixel(x, mRcSelected.Bottom, colorXor);
            }
            for (int y = mRcSelected.Top; y < mRcSelected.Bottom; y++)
            {
                colorOrg = mBmpBGImg.GetPixel(mRcSelected.Left, y);
                colorXor = Color.FromArgb(255 - colorOrg.A, 255 - colorOrg.R, 255 - colorOrg.G, 255 - colorOrg.B);
                mBmpBGImg.SetPixel(mRcSelected.Left, y, colorXor);
            }
            for (int y = mRcSelected.Top; y < mRcSelected.Bottom; y++)
            {
                colorOrg = mBmpBGImg.GetPixel(mRcSelected.Right, y);
                colorXor = Color.FromArgb(255 - colorOrg.A, 255 - colorOrg.R, 255 - colorOrg.G, 255 - colorOrg.B);
                mBmpBGImg.SetPixel(mRcSelected.Right, y, colorXor);
            }
            mInkPicture.Refresh();
        }
        public void DrawGrid()
        {
            DrawGrid(mInkPicture.BackgroundImage);
            mInkPicture.Refresh();
        }
        private void DrawGrid(Image img)
        {
            Graphics g = Graphics.FromImage(img);
            Pen pen = new Pen(Color.Green);
            for (int x = 0; x < img.Width; x += 50)
            {
                g.DrawLine(pen, x, 0, x, img.Height);
            }
            for (int y = 0; y < img.Height; y += 50)
            {
                g.DrawLine(pen, 0, y, img.Width, y);
            }
            pen.Dispose();
            g.Dispose();
        }

        private void HidePalette()
        {
            Console.WriteLine("HidePalette");
            if (mPaletteDlg != null && mPaletteDlg.IsDisposed == false)
            {
                mPaletteDlg.WindowState = FormWindowState.Minimized;
            }
        }
        public void ShowPalette()
        {
            Console.WriteLine("ShowPalette");
            if (mPaletteDlg == null || mPaletteDlg.IsDisposed)
            {
                mPaletteDlg = null;
                mPaletteDlg = new Palette();
                mPaletteDlg.Show(this);
            }
            else
            {
                //if (mPaletteDlg.Visible == false)
                {
                    //mPaletteDlg.Visible = true;
                    mPaletteDlg.WindowState = FormWindowState.Normal;
                }
            }
            this.Activate();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_Activated isCopyingDesktopImage:" + mStopRefreshImage + " isClosing:" + mIsClosing + " visible:" + this.Visible);
            mRcSelected = Rectangle.Empty;
            if (mStopRefreshImage == false && mIsClosing == false)
            {
                //                CopyDesktopImage();
                //if (this.Visible && mPaletteDlg.Visible == false)
                if (this.Visible)
                {
                    if (mUseDesktopImgAsBG)
                    {
                        CopyDesktopImage();
                    }
                    else
                    {
                        NewEmptyImage();
                        ShowPalette();
                    }
                    //ShowPalette();
                    //paletteDlg.Show(this);
                }
            }
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_VisibleChanged visible:" + Visible);
            
            if (this.Visible)
            {
                //ShowPalette();
                //paletteDlg.Show(this);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Form1_FormClosing");
            mIsClosing = true;
            if (mPaletteDlg != null && mPaletteDlg.IsDisposed == false)
            {
                mPaletteDlg.Close();
            }
        }

        public void SaveImage()
        {
            Console.WriteLine("SaveImage");
            Rectangle rc = mRcSelected;
            if (rc.IsEmpty)
            {
                rc = new Rectangle(0, 0, mInkPicture.Width, mInkPicture.Height);
            }
            mStopRefreshImage = true;
            HidePalette();
            SaveImage(rc);
            ShowPalette();
            mStopRefreshImage = false;
        }
        private void SaveImage(Rectangle rc)
        {
            Console.WriteLine("SaveImage rect x:{0} y:{1} width:{2} height:{3}", rc.X, rc.Y, rc.Width, rc.Height);
            Bitmap bmp = new Bitmap(rc.Width, rc.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rc.Location, new Point(0, 0), rc.Size);

            System.IO.Stream imgStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((imgStream = saveFileDialog1.OpenFile()) != null)
                {
                    bmp.Save(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgStream.Close();
                }
            }
            //mInkPicture.Image.Save("test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public void CopyToClipboard()
        {
            Console.WriteLine("CopyToClipboard");
            Rectangle rc = mRcSelected;
            if (rc.IsEmpty)
            {
                rc = new Rectangle(0, 0, mInkPicture.Width, mInkPicture.Height);
            }
            mStopRefreshImage = true;
            HidePalette();
            CopyToClipboard(rc);
            ShowPalette();
            mStopRefreshImage = false;
        }
        private void CopyToClipboard(Rectangle rc)
        {
            Console.WriteLine("CopyToClipboard rect x:{0} y:{1} width:{2} height:{3}", rc.X, rc.Y, rc.Width, rc.Height);
            Bitmap bmp = new Bitmap(rc.Width, rc.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rc.Location, new Point(0, 0), rc.Size);
            Clipboard.SetImage(bmp);
        }

        public void Clear()
        {
            if (this.mRcSelected == Rectangle.Empty)
            {
                // remove all
                foreach (Stroke st in mInkPicture.Ink.Strokes)
                {
                    mInkPicture.Ink.DeleteStroke(st);
                }
            }
            else
            {
                // remove all strokes inside the selected rect
                Graphics g = Graphics.FromImage(this.mBmpBGImg);
                Point[] pts = new Point[2];
                pts[0] = new Point(mRcSelected.Left, mRcSelected.Top);
                pts[1] = new Point(mRcSelected.Right, mRcSelected.Bottom);
                mInkPicture.Renderer.PixelToInkSpace(g, ref pts);
                Rectangle rcSelInk = new Rectangle(pts[0].X, pts[0].Y, pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
                foreach (Stroke st in mInkPicture.Ink.Strokes)
                {
                    Rectangle rcBound = st.GetBoundingBox();
                    if (rcSelInk.Contains(rcBound))
                    {
                        mInkPicture.Ink.DeleteStroke(st);
                    }
                }
            }
            mInkPicture.Refresh();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Activate();
            ShowPalette();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_Load");
            Visible = false;
            //WindowState = FormWindowState.Minimized;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C))
            {
                CopyToClipboard();
            }
            else if (e.Control && (e.KeyCode == Keys.S))
            {
                SaveImage();
            }
            else if (e.Control && (e.KeyCode == Keys.Z))
            {
                Undo();
            }
            else if (e.KeyCode == Keys.F5)
            {
                if (mUseDesktopImgAsBG)
                {
                    CopyDesktopImage();
                }
            }
        }

    }
}
