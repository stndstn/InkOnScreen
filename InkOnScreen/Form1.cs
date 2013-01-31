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

        NotifyIcon mNotifyIcon = null;
        Palette mPaletteDlg = null;
        public InkPicture mInkPicture = null;
        public bool mStopCopyDesktopImage = false;
        private bool mIsClosing = false;
        public Rectangle mRcSelected = Rectangle.Empty;


        /*
         * To-do
         *   Pen
         *     Color
         *     PenTip
         *     Height, Width
         *   Save as Jpeg
         *     Ctrl + S (whole screen)
         *     Specify the region and Ctrl + S
         *   Copy Bitmap to Clipboard
         *     Ctrl + C (whole screen)
         *     Specify the region and Ctrl + C
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
        public void CopyDesktopImage()
        {
            Console.WriteLine("CopyDesktopImage");
            mStopCopyDesktopImage = true;

            this.WindowState = FormWindowState.Minimized;
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

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBmp);

                DeleteObject(hBmp);
                GC.Collect();
                
                //draw grid
                Graphics g = Graphics.FromImage(bmp);
                for (int x = 0; x < bmp.Width; x += 50)
                {
                    g.DrawLine(new Pen(Color.Green), x, 0, x, bmp.Height);
                }
                for (int y = 0; y < bmp.Height; y += 50)
                {
                    g.DrawLine(new Pen(Color.Green), 0, y, bmp.Width, y);
                }
                g.Dispose();
                
                mInkPicture.BackgroundImage = bmp;
            }
            this.WindowState = FormWindowState.Maximized;
            ShowPalette();
            mStopCopyDesktopImage = false;
        }

        private void HidePalette()
        {
            if (mPaletteDlg != null && mPaletteDlg.IsDisposed == false)
            {
                mPaletteDlg.WindowState = FormWindowState.Normal;
            }
        }
        private void ShowPalette()
        {
            if (mPaletteDlg == null || mPaletteDlg.IsDisposed)
            {
                mPaletteDlg = null;
                mPaletteDlg = new Palette();
                mPaletteDlg.Show(this);
            }
            else
            {
                if (mPaletteDlg.Visible == false)
                {
                    mPaletteDlg.WindowState = FormWindowState.Normal;
                }
            }
            this.Activate();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_Activated isCopyingDesktopImage:" + mStopCopyDesktopImage + " isClosing:" + mIsClosing + " visible:" + this.Visible);
            mRcSelected = Rectangle.Empty;
            if (mStopCopyDesktopImage == false && mIsClosing == false)
            {
//                CopyDesktopImage();
                //if (this.Visible && mPaletteDlg.Visible == false)
                if (this.Visible)
                {
                    CopyDesktopImage();
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
            mPaletteDlg.Close();
            mNotifyIcon.Dispose();
        }

        public void SaveImage()
        {
            Rectangle rc = mRcSelected;
            if (rc.IsEmpty)
            {
                rc = new Rectangle(0, 0, mInkPicture.Width, mInkPicture.Height);
            }
            SaveImage(rc);
        }
        private void SaveImage(Rectangle rc)
        {
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
            Rectangle rc = mRcSelected;
            if (rc.IsEmpty)
            {
                rc = new Rectangle(0, 0, mInkPicture.Width, mInkPicture.Height);
            }
            CopyToClipboard(rc);
        }
        private void CopyToClipboard(Rectangle rc)
        {
            Bitmap bmp = new Bitmap(rc.Width, rc.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rc.Location, new Point(0, 0), rc.Size);
            Clipboard.SetImage(bmp);
        }

        public void Clear()
        {
            foreach (Stroke st in mInkPicture.Ink.Strokes)
            {
                mInkPicture.Ink.DeleteStroke(st);
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
            //Visible = false;
            WindowState = FormWindowState.Minimized;
        }
    }
}
