using System;
using System.Linq;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DotNetBejewelledBot
{
    class BejeweledWindowManager
    {
        const int touchDelay = 1000;
        private Rectangle m_Window;
        private Bitmap m_ScreenShot;
        private LockBitmap m_lockScreenShot;
        private Bitmap m_BejeweledImage;
        private LockBitmap m_lockBejeweledImage;
        private Bitmap m_ColourGrid;
        private Color[,] m_ColorMatrix;
        private long[,] lastTouched = new long[8, 8];

        private long moves3 = 0;
        private long moves4 = 0;
        private long moves5 = 0;
        private long moves6 = 0;

        private DxCapture dxCap;

        private enum ValidGemMoves
        {
            Up,
            Down,
            Left,
            Right
        }

        public Bitmap ScreenShot
        {
            get
            {
                return (Bitmap)m_BejeweledImage;
            }
        }

        public Bitmap ColourGrid
        {
            get
            {
                return m_ColourGrid;
            }
        }

        public BejeweledWindowManager()
        {
            m_Window = new Rectangle(new Point(0, 0), new Size(320, 320));
            m_ColorMatrix = new Color[8, 8];

            dxCap = new DxCapture();
            dxCap.Init();

            //GetScreenshot();
            //GetColourGrid();
        }

        public void GetScreenshot()
        {
            try
            {
                //m_ScreenShot = new Bitmap(2560, 1440, PixelFormat.Format32bppArgb);
                m_ScreenShot = dxCap.TakeScreenShot();

                //using (Graphics gfxScreenshot = Graphics.FromImage(m_ScreenShot))
                //{
                //    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                //                                 Screen.PrimaryScreen.Bounds.Y,
                //                                 0,
                //                                 0,
                //                                 Screen.PrimaryScreen.Bounds.Size,
                //                                 CopyPixelOperation.SourceCopy);
                //}
                m_lockScreenShot = new LockBitmap(m_ScreenShot);
            }
            catch
            { }

            m_BejeweledImage = m_ScreenShot.Clone(m_Window, PixelFormat.Format32bppArgb);
            m_lockBejeweledImage = new LockBitmap(m_BejeweledImage);
        }

        public void CalculateMoves()
        {
            long currentTicks = DateTime.Now.Ticks;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];

                        // T
                        if (
                            (x >= 3 && y >= 1 && y <= 6) &&
                            (m_ColorMatrix[x - 3, y] == currentColor && m_ColorMatrix[x - 2, y] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor)
                            )
                        {
                            MoveGem(x, y, ValidGemMoves.Left);
                            moves6++;
                        }
                        else if (
                            (x <= 4 && y >= 1 && y <= 6) &&
                            (m_ColorMatrix[x + 3, y] == currentColor && m_ColorMatrix[x + 2, y] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor)
                            )
                        {
                            MoveGem(x, y, ValidGemMoves.Right);
                            moves6++;
                        }
                        else if (
                            (x >= 1 && x <= 6 && y >= 3) &&
                            (m_ColorMatrix[x, y - 3] == currentColor && m_ColorMatrix[x, y - 2] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor)
                            )
                        {
                            MoveGem(x, y, ValidGemMoves.Up);
                            moves6++;
                        }
                        else if (
                            (x >= 1 && x <= 6 && y <= 4) &&
                            (m_ColorMatrix[x, y + 3] == currentColor && m_ColorMatrix[x, y + 2] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor)
                            )
                        {
                            MoveGem(x, y, ValidGemMoves.Down);
                            moves6++;
                        }
                        // L
                    }
                }
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];
                        // L

                        if (
                                // 5 row
                                ((y >= 1) && (x >= 2 && x <= 5) && (m_ColorMatrix[x - 2, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x + 2, y - 1] == currentColor))
                                )
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves5++;
                            }
                        }
                        else if (
                                // 5 row
                                ((y <= 6) && (x >= 2 && x <= 5) && (m_ColorMatrix[x - 2, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x + 2, y + 1] == currentColor))
                                )
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves5++;
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];
                        // L

                        if (
                                // 4 row
                                ((y >= 1) && (x >= 2 && x <= 6) && (m_ColorMatrix[x - 2, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) ||
                                  (y >= 1) && (x >= 1 && x <= 5) && (m_ColorMatrix[x + 2, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor)))
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves4++;
                            }
                        }
                        else if (
                                // 4 row
                                ((y <= 6) && (x >= 2 && x <= 6) && (m_ColorMatrix[x - 2, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) ||
                                  (y <= 6) && (x >= 1 && x <= 5) && (m_ColorMatrix[x + 2, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor)))
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves4++;
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];
                        if (
                            // x - x x
                            ((x <= 4) && (currentColor == m_ColorMatrix[x + 2, y]) &&
                            (currentColor == m_ColorMatrix[x + 3, y])) ||
                            ((x <= 6) && (y >= 1) && (y <= 6) && (m_ColorMatrix[x + 1, y - 1] == currentColor) &&
                            (m_ColorMatrix[x + 1, y + 1] == currentColor)) ||
                            ((y <= 5) && (x <= 6) && (m_ColorMatrix[x + 1, y + 1] == currentColor) &&
                            (m_ColorMatrix[x + 1, y + 2] == currentColor)) ||
                            ((y >= 2) && (x <= 6) && (m_ColorMatrix[x + 1, y - 1] == currentColor) &&
                            (m_ColorMatrix[x + 1, y - 2] == currentColor)))
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves3++;
                            }
                        }
                        else if (
                                 ((x >= 3) && (currentColor == m_ColorMatrix[x - 2, y]) &&
                                 (currentColor == m_ColorMatrix[x - 3, y])) ||

                                 ((x >= 1) && (y >= 1) && (m_ColorMatrix[x - 1, y - 1] == currentColor) &&
                                 (y <= 6) && (m_ColorMatrix[x - 1, y + 1] == currentColor)) ||

                                 ((x >= 1) && (y <= 5) && (m_ColorMatrix[x - 1, y + 1] == currentColor) &&
                                 (m_ColorMatrix[x - 1, y + 2] == currentColor)) ||

                                 ((x >= 1) && (y >= 2) && (m_ColorMatrix[x - 1, y - 1] == currentColor) &&
                                  (m_ColorMatrix[x - 1, y - 2] == currentColor)))
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves3++;
                            }
                        }
                        else if (
                                 ((y >= 3) && (currentColor == m_ColorMatrix[x, y - 2]) &&
                                 (currentColor == m_ColorMatrix[x, y - 3])) ||

                                 ((y >= 1) && (((x >= 1) && (m_ColorMatrix[x - 1, y - 1] == currentColor) &&
                                  (x <= 6) && (m_ColorMatrix[x + 1, y - 1] == currentColor)) ||

                                 ((y >= 1) && (x >= 2) && (m_ColorMatrix[x - 1, y - 1] == currentColor) &&
                                  (m_ColorMatrix[x - 2, y - 1] == currentColor)) ||

                                 ((y >= 1) && (x <= 5) && (m_ColorMatrix[x + 1, y - 1] == currentColor) &&
                                  (m_ColorMatrix[x + 2, y - 1] == currentColor)))))
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves3++;
                            }
                        }
                        else if (
                                 // 3'fers
                                 (((y <= 6) && (x <= 5) && (currentColor == m_ColorMatrix[x + 1, y + 1] &&
                                 (currentColor == m_ColorMatrix[x + 2, y + 1]))) ||

                                 ((x >= 2) && (y <= 6) && (currentColor == m_ColorMatrix[x - 1, y + 1] &&
                                  (currentColor == m_ColorMatrix[x - 2, y + 1]))) ||

                                 ((y <= 4) && (m_ColorMatrix[x, y + 2] == currentColor) &&
                                   (m_ColorMatrix[x, y + 3] == currentColor)) ||

                                 ((x >= 1) && (x <= 6) && (y <= 6) && (currentColor == m_ColorMatrix[x + 1, y + 1]) &&
                                   (currentColor == m_ColorMatrix[x - 1, y + 1])))
                                   )
                        {
                            if (currentTicks - lastTouched[x, y] > (touchDelay * 10000))
                            {
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves3++;
                            }
                        }
                    }
                }
            }
        }
        public void GetColourGrid()
        {
            m_ColourGrid = new Bitmap(m_Window.Width,
                                      m_Window.Height,
                                      PixelFormat.Format32bppArgb);

            using (Graphics gfxColourgrid = Graphics.FromImage(m_ColourGrid))
            {
                m_lockBejeweledImage.LockBits();

                for (int x = 0; x < m_lockBejeweledImage.Width; x += 40)
                {
                    for (int y = 0; y < m_lockBejeweledImage.Height; y += 40)
                    {
                        var test = m_lockBejeweledImage.GetSubset(x + 20, y + 10, 20, 1);
                        int Rs = (int)test.Where((n, index) => index % 4 == 2).Average(num => (int)num);
                        int Gs = (int)test.Where((n, index) => index % 4 == 1).Average(num => (int)num);
                        int Bs = (int)test.Where((n, index) => index % 4 == 0).Average(num => (int)num);

                        #region oldstuff

                        #endregion

                        // high red
                        if (Rs >= 180)
                        {
                            // high green
                            if (Gs > 180)
                            {
                                if (Bs > 200)
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.White;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.White), new Rectangle(x, y, 40, 40));
                                }
                                else
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Yellow;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(x, y, 40, 40));
                                }
                            }
                            else if (Gs < 100)
                            // low green
                            {
                                // high blue
                                if (Bs > 170)
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Purple;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Purple), new Rectangle(x, y, 40, 40));
                                }
                                else // low blue
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Red;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Red), new Rectangle(x, y, 40, 40));
                                }
                            }
                            else
                            {
                                m_ColorMatrix[x / 40, y / 40] = Color.Orange;
                                gfxColourgrid.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(x, y, 40, 40));
                            }
                        }
                        // low red
                        else
                        {
                            // high green
                            if (Gs > 170)
                            {
                                // high blue
                                if (Bs > 200)
                                {
                                    //throw new Exception("No color");
                                }
                                // low blue
                                else
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Green;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Green), new Rectangle(x, y, 40, 40));
                                }
                            }
                            else
                            // low green
                            {
                                // high blue
                                if (Bs > 200)
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Blue;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(x, y, 40, 40));
                                }
                                else if (Bs == 176)
                                { // x2 white
                                    m_ColorMatrix[x / 40, y / 40] = Color.White;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.White), new Rectangle(x, y, 40, 40));
                                }
                                else // low blue
                                {
                                    m_ColorMatrix[x / 40, y / 40] = Color.Black;
                                    gfxColourgrid.FillRectangle(new SolidBrush(Color.Black), new Rectangle(x, y, 40, 40));
                                }
                            }
                        }
                        //m_ColorMatrix[x / 40, y / 40] = Color.FromArgb(R, G, B);
                        //gfxColourgrid.FillRectangle(new SolidBrush(Color.FromArgb(R,G,B)), new Rectangle(x, y, 40, 40));
                        //}
                    }
                }
                m_lockBejeweledImage.UnlockBits();
            }
        }

        private void MoveGem(int x, int y, ValidGemMoves Direction)
        {
            switch (Direction)
            {

                case ValidGemMoves.Down:
                    {
                        // Move gem down
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        Thread.Sleep(5);
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) + 60);
                        WinAPI.DoMouseClick();
                        break;
                    }
                case ValidGemMoves.Left:
                    {
                        // Move gem left
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        Thread.Sleep(5);
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) - 20, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        break;
                    }
                case ValidGemMoves.Right:
                    {
                        // Move Gem Right
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        Thread.Sleep(5);
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 60, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        break;
                    }
                case ValidGemMoves.Up:
                    {
                        // Move gem up
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) + 20);
                        WinAPI.DoMouseClick();
                        Thread.Sleep(5);
                        WinAPI.SetCursorPos(m_Window.Left + (x * 40) + 20, m_Window.Top + (y * 40) - 20);
                        WinAPI.DoMouseClick();
                        break;
                    }
            }
            lastTouched[x, y] = DateTime.Now.Ticks;
        }

        public bool Calibrate()
        {
            Point Location = new Point();
            m_lockScreenShot.LockBits();
            for (int x = 0; x < m_lockScreenShot.Width; x++)
            {
                for (int y = 0; y < m_lockScreenShot.Height; y++)
                {
                    if (Location == Point.Empty)
                    {
                        if (m_lockScreenShot.GetPixel(x, y) == Color.FromArgb(255, 70, 33, 10))
                        {
                            Location = new Point(x + 1, y);
                            break;
                        }
                    }
                }
                if (Location != Point.Empty)
                    break;
            }
            m_lockScreenShot.UnlockBits();
            if (Location != Point.Empty)
            {
                m_Window = new Rectangle(Location, m_Window.Size);
                m_BejeweledImage = m_ScreenShot.Clone(m_Window, PixelFormat.Format32bppArgb);
                m_lockBejeweledImage = new LockBitmap(m_BejeweledImage);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class WindowNotFoundException : Exception { }
}
