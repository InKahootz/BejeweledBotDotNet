using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DotNetBejewelledBot
{
    internal class BejeweledWindowManager
    {
        private const int touchDelay = 1200 * 10000;  // Change first number, 10,000 is ticks per ms.
        private Rectangle m_Window;
        private Bitmap m_ScreenShot;
        private LockBitmap m_lockScreenShot;
        private Bitmap m_BejeweledImage;
        private LockBitmap m_lockBejeweledImage;
        private Bitmap m_ColourGrid;
        private Color[,] m_ColorMatrix;
        private long[,] lastTouched = new long[8, 8];

        private List<GemMove> GemMovesToDo = new List<GemMove>();

        private long moves1 = 0;
        private long moves2 = 0;
        private long moves3 = 0;
        private long moves4 = 0;
        private long moves5 = 0;

        private enum ValidGemMoves
        {
            Up,
            Down,
            Left,
            Right
        }

        private enum MoveTypes
        {
            T,
            L,
            Five,
            Four,
            Three
        }

        public bool BotRunning { get; set; }

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
        }

        public void GetScreenshot()
        {
            try
            {
                m_ScreenShot = new Bitmap(2560, 1440, PixelFormat.Format32bppArgb);

                using (Graphics gfxScreenshot = Graphics.FromImage(m_ScreenShot))
                {
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                 Screen.PrimaryScreen.Bounds.Y,
                                                 0,
                                                 0,
                                                 Screen.PrimaryScreen.Bounds.Size,
                                                 CopyPixelOperation.SourceCopy);
                }
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
                        if (currentColor == Color.Black) continue;
                        // T
                        if (
                            (x >= 3 && y >= 1 && y <= 6) &&
                            (m_ColorMatrix[x - 3, y] == currentColor && m_ColorMatrix[x - 2, y] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor)
                            )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 3, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 3, y] = Color.Black;
                                lastTouched[x - 3, y] = currentTicks;
                                m_ColorMatrix[x - 2, y] = Color.Black;
                                lastTouched[x - 2, y] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                m_ColorMatrix[x - 1, y] = Color.Black;
                                lastTouched[x - 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x - 1, ToY = y, Priority = 5});
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves5++;
                            }
                        }
                        else if (
                            (x <= 4 && y >= 1 && y <= 6) &&
                            (m_ColorMatrix[x + 3, y] == currentColor && m_ColorMatrix[x + 2, y] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor)
                            )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 3, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x + 3, y] = Color.Black;
                                lastTouched[x + 3, y] = currentTicks;
                                m_ColorMatrix[x + 2, y] = Color.Black;
                                lastTouched[x + 2, y] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                m_ColorMatrix[x + 1, y] = Color.Black;
                                lastTouched[x + 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x + 1, ToY = y, Priority = 5});
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves5++;
                            }
                        }
                        else if (
                            (x >= 1 && x <= 6 && y >= 3) &&
                            (m_ColorMatrix[x, y - 3] == currentColor && m_ColorMatrix[x, y - 2] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor)
                            )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x, y - 3] > touchDelay) &&
                                (currentTicks - lastTouched[x, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay))
                            {
                                m_ColorMatrix[x, y - 3] = Color.Black;
                                lastTouched[x, y - 3] = currentTicks;
                                m_ColorMatrix[x, y - 2] = Color.Black;
                                lastTouched[x, y - 2] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                m_ColorMatrix[x, y - 1] = Color.Black;
                                lastTouched[x, y - 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y - 1, Priority = 5});
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves5++;
                            }
                        }
                        else if (
                            (x >= 1 && x <= 6 && y <= 4) &&
                            (m_ColorMatrix[x, y + 3] == currentColor && m_ColorMatrix[x, y + 2] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor)
                            )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x, y + 3] > touchDelay) &&
                                (currentTicks - lastTouched[x, y + 2] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x, y + 3] = Color.Black;
                                lastTouched[x, y + 3] = currentTicks;
                                m_ColorMatrix[x, y + 2] = Color.Black;
                                lastTouched[x, y + 2] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                m_ColorMatrix[x, y + 1] = Color.Black;
                                lastTouched[x, y + 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 5});
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves5++;
                            }
                        }
                    }
                }
            }

            #region L Moves

            //for (int x = 0; x < 8; x++)
            //{
            //    for (int y = 0; y < 8; y++)
            //    {
            //        if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
            //        {
            //            Color currentColor = m_ColorMatrix[x, y];
            //            if (currentColor == Color.Black) continue;

            //            // ┘ ┐
            //            if (
            //                (((x >= 3) && (y >= 2)) && (m_ColorMatrix[x - 3, y] == currentColor && m_ColorMatrix[x - 2, y] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor && m_ColorMatrix[x - 1, y - 2] == currentColor))
            //                ||
            //                (((x >= 3) && (y <= 5)) && (m_ColorMatrix[x - 3, y] == currentColor && m_ColorMatrix[x - 2, y] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor && m_ColorMatrix[x - 1, y + 2] == currentColor))
            //                )
            //            {
            //                if (currentTicks - lastTouched[x, y] > touchDelay) &&)
            //                {
            //                    MoveGem(x, y, ValidGemMoves.Left);
            //                    moves6++;
            //                }
            //            }
            //            // ┌ └
            //            else if (
            //                (((x <= 4) && (y <= 5)) && ((m_ColorMatrix[x + 3, y] == currentColor && m_ColorMatrix[x + 2, y] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor && m_ColorMatrix[x + 1, y + 2] == currentColor)))
            //                ||
            //                (((x <= 4) && (y >= 2)) && ((m_ColorMatrix[x + 3, y] == currentColor && m_ColorMatrix[x + 2, y] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x + 1, y - 2] == currentColor)))
            //                )
            //            {
            //                    if (currentTicks - lastTouched[x, y] > touchDelay) &&)
            //                    {
            //                        MoveGem(x, y, ValidGemMoves.Right);
            //                        moves6++;
            //                    }
            //            }
            //            // └ ┘
            //            else if (
            //                (((x <= 5) && (y >= 3)) && ((m_ColorMatrix[x, y - 3] == currentColor && m_ColorMatrix[x, y - 2] == currentColor && m_ColorMatrix[x + 1, y - 1] == currentColor && m_ColorMatrix[x + 2, y - 1] == currentColor)))
            //                ||
            //                (((x >= 2) && (y >= 3)) && ((m_ColorMatrix[x, y - 3] == currentColor && m_ColorMatrix[x, y - 2] == currentColor && m_ColorMatrix[x - 1, y - 1] == currentColor && m_ColorMatrix[x - 2, y - 1] == currentColor)))
            //                )
            //            {
            //                        if (currentTicks - lastTouched[x, y] > touchDelay) &&)
            //                        {
            //                            MoveGem(x, y, ValidGemMoves.Up);
            //                            moves6++;
            //                        }
            //            }
            //            // ┌ ┐
            //            else if (
            //                (((x <= 5) && (y <= 4)) && ((m_ColorMatrix[x, y + 3] == currentColor && m_ColorMatrix[x, y + 2] == currentColor && m_ColorMatrix[x + 1, y + 1] == currentColor && m_ColorMatrix[x + 2, y + 1] == currentColor)))
            //                ||
            //                (((x >= 2) && (y <= 4)) && ((m_ColorMatrix[x, y + 3] == currentColor && m_ColorMatrix[x, y + 2] == currentColor && m_ColorMatrix[x - 1, y + 1] == currentColor && m_ColorMatrix[x - 2, y + 1] == currentColor)))
            //                )
            //            {
            //                            if (currentTicks - lastTouched[x, y] > touchDelay) &&)
            //                            {
            //                                MoveGem(x, y, ValidGemMoves.Down);
            //                                moves6++;
            //                            }
            //            }
            //        }
            //    }
            //}

            #endregion L Moves

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];
                        if (currentColor == Color.Black) continue;
                        if (
                                // 5 row
                                ((y >= 1) && (x >= 2 && x <= 5) && (m_ColorMatrix[x - 2, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x + 2, y - 1] == currentColor))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y - 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 2, y - 1] = Color.Black;
                                lastTouched[x - 2, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 2, y - 1] = Color.Black;
                                lastTouched[x + 2, y - 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                m_ColorMatrix[x, y - 1] = Color.Black;
                                lastTouched[x, y - 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y-1, Priority =  4});
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves4++;
                            }
                        }
                        else if (
                                // 5 row
                                ((y <= 6) && (x >= 2 && x <= 5) && (m_ColorMatrix[x - 2, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x + 2, y + 1] == currentColor))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 2, y + 1] = Color.Black;
                                lastTouched[x - 2, y + 1] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 2, y + 1] = Color.Black;
                                lastTouched[x + 2, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                m_ColorMatrix[x, y + 1] = Color.Black;
                                lastTouched[x, y + 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority =  4});
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves4++;
                            }
                        }
                        else if (
                                // 5 column
                                ((x >= 1) && (y >= 2 && y <= 5) && (m_ColorMatrix[x - 1, y + 2] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 2] == currentColor))
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 2] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 2] > touchDelay))
                            {
                                m_ColorMatrix[x - 1, y + 2] = Color.Black;
                                lastTouched[x - 1, y + 2] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 2] = Color.Black;
                                lastTouched[x - 1, y - 2] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                m_ColorMatrix[x - 1, y] = Color.Black;
                                lastTouched[x - 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x - 1, ToY = y, Priority =  4});
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves4++;
                            }
                        }
                        else if (
                                // 5 column
                                ((x <= 6) && (y >= 2 && y <= 5) && (m_ColorMatrix[x + 1, y - 2] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 2] == currentColor))
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 2] > touchDelay))
                            {
                                m_ColorMatrix[x + 1, y - 2] = Color.Black;
                                lastTouched[x + 1, y - 2] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 2] = Color.Black;
                                lastTouched[x + 1, y + 2] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                m_ColorMatrix[x + 1, y] = Color.Black;
                                lastTouched[x + 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x + 1, ToY = y, Priority =  4});
                                MoveGem(x, y, ValidGemMoves.Right);
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
                        if (currentColor == Color.Black) continue;

                        #region 4 Row

                        if (
                                // 4 row
                                (y >= 1) && (x >= 2 && x <= 6) && (m_ColorMatrix[x - 2, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor)
                            )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 2, y - 1] = Color.Black;
                                lastTouched[x - 2, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                m_ColorMatrix[x, y - 1] = Color.Black;
                                lastTouched[x, y - 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y - 1, Priority = 2});
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves2++;
                            }
                        }
                        else if (
                                // 4 row
                                (y >= 1) && (x >= 1 && x <= 5) && (m_ColorMatrix[x + 2, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay))
                            {
                                m_ColorMatrix[x + 2, y - 1] = Color.Black;
                                lastTouched[x + 2, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                m_ColorMatrix[x, y - 1] = Color.Black;
                                lastTouched[x, y - 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y - 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves2++;
                            }
                        }
                        else if (
                                  // 4 row

                                  (y <= 6) && (x >= 1 && x <= 5) && (m_ColorMatrix[x + 2, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor))
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x + 2, y + 1] = Color.Black;
                                lastTouched[x + 2, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                m_ColorMatrix[x, y + 1] = Color.Black;
                                lastTouched[x, y + 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2});
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves2++;
                            }
                        }
                        else if (
                                // 4 row
                                (y <= 6) && (x >= 2 && x <= 6) && (m_ColorMatrix[x - 2, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 2, y + 1] = Color.Black;
                                lastTouched[x - 2, y + 1] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                m_ColorMatrix[x, y + 1] = Color.Black;
                                lastTouched[x, y + 1] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves2++;
                            }
                        }

                        #endregion 4 Row

                        #region 4 Column

                        else if (
                                // 4 column
                                (x <= 6) && (y >= 2 && y <= 6) && (m_ColorMatrix[x + 1, y - 2] == currentColor) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x + 1, y - 2] = Color.Black;
                                lastTouched[x + 1, y - 2] = currentTicks;
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                m_ColorMatrix[x + 1, y] = Color.Black;
                                lastTouched[x + 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x + 1, ToY = y, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves2++;
                            }
                        }
                        else if (
                                // 4 column
                                (x <= 6) && (y >= 1 && y <= 5) && (m_ColorMatrix[x + 1, y - 1] == currentColor) && (m_ColorMatrix[x + 1, y + 1] == currentColor) && (m_ColorMatrix[x + 1, y + 2] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 2] > touchDelay))
                            {
                                m_ColorMatrix[x + 1, y - 1] = Color.Black;
                                lastTouched[x + 1, y - 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 1] = Color.Black;
                                lastTouched[x + 1, y + 1] = currentTicks;
                                m_ColorMatrix[x + 1, y + 2] = Color.Black;
                                lastTouched[x + 1, y + 2] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                m_ColorMatrix[x + 1, y] = Color.Black;
                                lastTouched[x + 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x + 1, ToY = y, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves2++;
                            }
                        }
                        else if (
                                // 4 column
                                (x >= 1) && (y >= 2 && y <= 6) && (m_ColorMatrix[x - 1, y - 2] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 1, y - 2] = Color.Black;
                                lastTouched[x - 1, y - 2] = currentTicks;

                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;

                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                m_ColorMatrix[x - 1, y] = Color.Black;
                                lastTouched[x - 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x - 1, ToY = y, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves2++;
                            }
                        }
                        else if (
                                // 4 column
                                (x >= 1) && (y >= 1 && y <= 5) && (m_ColorMatrix[x - 1, y + 2] == currentColor) && (m_ColorMatrix[x - 1, y + 1] == currentColor) && (m_ColorMatrix[x - 1, y - 1] == currentColor)
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 2] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay))
                            {
                                m_ColorMatrix[x - 1, y + 2] = Color.Black;
                                lastTouched[x - 1, y + 2] = currentTicks;
                                m_ColorMatrix[x - 1, y + 1] = Color.Black;
                                lastTouched[x - 1, y + 1] = currentTicks;
                                m_ColorMatrix[x - 1, y - 1] = Color.Black;
                                lastTouched[x - 1, y - 1] = currentTicks;
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                m_ColorMatrix[x - 1, y] = Color.Black;
                                lastTouched[x - 1, y] = currentTicks;
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x - 1, ToY = y, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves2++;
                            }
                        }

                        #endregion 4 Column
                    }
                }
            }

            for (int x = 0; x <= 7; x++)
            {
                for (int y = 0; y <= 7; y++)
                {
                    if (BejeweledColor.Collection.Contains(m_ColorMatrix[x, y]))
                    {
                        Color currentColor = m_ColorMatrix[x, y];
                        if (currentColor == Color.Black) continue;

                        #region 3 Row

                        if (
                            ((x <= 4) && (currentColor == m_ColorMatrix[x + 2, y]) && (currentColor == m_ColorMatrix[x + 3, y]))
                            )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 3, y] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 3, 0);
                                TouchColor(currentTicks, x, y, 2, 0);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                TouchColor(currentTicks, x, y, 1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves1++;
                            }
                        }
                        else if (
                            ((x >= 3) && (currentColor == m_ColorMatrix[x - 2, y]) && (currentColor == m_ColorMatrix[x - 3, y]))
                            )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 3, y] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -3, 0);
                                TouchColor(currentTicks, x, y, -2, 0);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                TouchColor(currentTicks, x, y, -1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves1++;
                            }
                        }
                        else if (
                                ((x <= 5 && y <= 6) && (currentColor == m_ColorMatrix[x + 1, y + 1]) && (currentColor == m_ColorMatrix[x + 2, y + 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y + 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 1, 1);
                                TouchColor(currentTicks, x, y, 2, 1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                TouchColor(currentTicks, x, y, 0, 1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 1 && x <= 6 && y <= 6) && (currentColor == m_ColorMatrix[x - 1, y + 1]) && (currentColor == m_ColorMatrix[x + 1, y + 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -1, 1);
                                TouchColor(currentTicks, x, y, 1, 1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                TouchColor(currentTicks, x, y, 0, 1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 2 && y <= 6) && (currentColor == m_ColorMatrix[x - 2, y + 1]) && (currentColor == m_ColorMatrix[x - 1, y + 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 2, 0);
                                TouchColor(currentTicks, x, y, 3, 0);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                TouchColor(currentTicks, x, y, 0, 1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves1++;
                            }
                        }
                        else if (
                                ((x <= 5 && y >= 1) && (currentColor == m_ColorMatrix[x + 1, y - 1]) && (currentColor == m_ColorMatrix[x + 2, y - 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 2, y - 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 1, -1);
                                TouchColor(currentTicks, x, y, 2, -1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                TouchColor(currentTicks, x, y, 0, -1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 1 && x <= 6 && y >= 1) && (currentColor == m_ColorMatrix[x - 1, y - 1]) && (currentColor == m_ColorMatrix[x + 1, y - 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -1, -1);
                                TouchColor(currentTicks, x, y, 1, -1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                TouchColor(currentTicks, x, y, 0, -1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 2 && y >= 1) && (currentColor == m_ColorMatrix[x - 2, y - 1]) && (currentColor == m_ColorMatrix[x - 1, y - 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 2, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -2, -1);
                                TouchColor(currentTicks, x, y, -1, -1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                TouchColor(currentTicks, x, y, 0, -1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves1++;
                            }
                        }
                        #endregion 3 Row

                        #region 3 Column
                        if (
                            ((y <= 4) && (currentColor == m_ColorMatrix[x, y + 2]) && (currentColor == m_ColorMatrix[x, y + 3]))
                            )
                        {
                            if ((currentTicks - lastTouched[x, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x, y + 2] > touchDelay) &&
                                (currentTicks - lastTouched[x, y + 3] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 0, 2);
                                TouchColor(currentTicks, x, y, 0, 3);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y + 1];
                                TouchColor(currentTicks, x, y, 0, 1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Down);
                                moves1++;
                            }
                        }
                        else if (
                            ((y >= 3) && (currentColor == m_ColorMatrix[x, y - 2]) && (currentColor == m_ColorMatrix[x, y - 3]))
                            )
                        {
                            if ((currentTicks - lastTouched[x, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x, y - 3] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 0, -2);
                                TouchColor(currentTicks, x, y, 0, -3);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x, y - 1];
                                TouchColor(currentTicks, x, y, 0, -1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Up);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 1 && y <= 5) && (currentColor == m_ColorMatrix[x - 1, y + 1]) && (currentColor == m_ColorMatrix[x - 1, y + 2]))
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 2] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -1, 1);
                                TouchColor(currentTicks, x, y, -1, 2);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                TouchColor(currentTicks, x, y, -1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 1 && y >= 1 && y <= 6) && (currentColor == m_ColorMatrix[x - 1, y - 1]) && (currentColor == m_ColorMatrix[x - 1, y + 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y + 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -1, -1);
                                TouchColor(currentTicks, x, y, -1, +1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                TouchColor(currentTicks, x, y, -1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves1++;
                            }
                        }
                        else if (
                                ((x >= 1 && y >= 2) && (currentColor == m_ColorMatrix[x - 1, y - 2]) && (currentColor == m_ColorMatrix[x - 1, y - 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x - 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x - 1, y - 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, -1, -2);
                                TouchColor(currentTicks, x, y, -1, -1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x - 1, y];
                                TouchColor(currentTicks, x, y, 0, 1);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Left);
                                moves1++;
                            }
                        }
                        else if (
                                ((x <= 6 && y <= 5) && (currentColor == m_ColorMatrix[x + 1, y + 1]) && (currentColor == m_ColorMatrix[x + 1, y + 2]))
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 2] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 1, 1);
                                TouchColor(currentTicks, x, y, 1, 2);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                TouchColor(currentTicks, x, y, 1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves1++;
                            }
                        }
                        else if (
                                ((x <= 6 && y >= 1 && y <= 6) && (currentColor == m_ColorMatrix[x + 1, y - 1]) && (currentColor == m_ColorMatrix[x + 1, y + 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y + 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 1, -1);
                                TouchColor(currentTicks, x, y, 1, 1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                TouchColor(currentTicks, x, y, 1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves1++;
                            }
                        }
                        else if (
                                ((x <= 6 && y >= 2) && (currentColor == m_ColorMatrix[x + 1, y - 2]) && (currentColor == m_ColorMatrix[x + 1, y - 1]))
                                )
                        {
                            if ((currentTicks - lastTouched[x + 1, y] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 2] > touchDelay) &&
                                (currentTicks - lastTouched[x + 1, y - 1] > touchDelay))
                            {
                                TouchColor(currentTicks, x, y, 1, -2);
                                TouchColor(currentTicks, x, y, 1, -1);
                                m_ColorMatrix[x, y] = m_ColorMatrix[x + 1, y];
                                TouchColor(currentTicks, x, y, 1, 0);
                                //GemMovesToDo.Add(new GemMove() { FromX = x, FromY = y, ToX = x, ToY = y + 1, Priority = 2 });
                                MoveGem(x, y, ValidGemMoves.Right);
                                moves1++;
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        public void TouchColor(long currentTicks, int x, int y, int offsetX, int offsetY)
        {
            m_ColorMatrix[x + offsetX, y + offsetY] = Color.Black;
            lastTouched[x + offsetX, y + offsetY] = currentTicks;
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
                        var test = m_lockBejeweledImage.GetSubset(x, y, 40, 40);
                        // [BGRA]
                        int Rs = (int)test.Where((n, index) => index % 4 == 2).Where((n, index) => clippingMask[index % 40, index / 40] == 1).Average(num => (int)num);
                        int Gs = (int)test.Where((n, index) => index % 4 == 1).Where((n, index) => clippingMask[index % 40, index / 40] == 1).Average(num => (int)num);
                        int Bs = (int)test.Where((n, index) => index % 4 == 0).Where((n, index) => clippingMask[index % 40, index / 40] == 1).Average(num => (int)num);

                        Color nearest = GetNearest(BejeweledColor.Collection, Color.FromArgb(Rs, Gs, Bs));

                        if (nearest == BejeweledColor.YellowCoin)
                        {
                            nearest = BejeweledColor.Yellow;
                        }

                        m_ColorMatrix[x / 40, y / 40] = nearest;
                        gfxColourgrid.FillRectangle(new SolidBrush(nearest), new Rectangle(x, y, 40, 40));
                    }
                }
                m_lockBejeweledImage.UnlockBits();
            }
        }

        private void MoveGem(int x, int y, ValidGemMoves Direction)
        {
            //if (BotRunning)
            //{
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
            //}
            //else
            //{
            using (Graphics gfxColourgrid = Graphics.FromImage(m_ColourGrid))
            {
                Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 5);
                pen.StartCap = LineCap.RoundAnchor;
                pen.EndCap = LineCap.ArrowAnchor;
                int ToX = 0;
                int ToY = 0;
                if (Direction == ValidGemMoves.Left || Direction == ValidGemMoves.Right)
                {
                    ToX = (Convert.ToInt32(Direction) % 2) == 1 ? x * 40 + 60 : x * 40 - 20;
                }
                else
                {
                    ToX = x * 40 + 20;
                }
                if (Direction == ValidGemMoves.Up || Direction == ValidGemMoves.Down)
                {
                    ToY = (Convert.ToInt32(Direction) % 2) == 1 ? y * 40 + 60 : y * 40 - 20;
                }
                else
                {
                    ToY = y * 40 + 20;
                }
                gfxColourgrid.DrawLine(pen, x * 40 + 20, y * 40 + 20, ToX, ToY);
            }
            //}
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

        private static Color GetNearest(List<Color> bejeweledColors, Color gemColor)
        {
            var colors = bejeweledColors.Select(x => new { Value = x, Diff = GetDiff(x, gemColor) }).ToList();
            var min = colors.Min(x => x.Diff);
            return colors.Find(x => x.Diff == min).Value;
        }

        private static int GetDiff(Color color, Color gemColor)
        {
            int a = color.A - gemColor.A,
                r = color.R - gemColor.R,
                g = color.G - gemColor.G,
                b = color.B - gemColor.B;
            return a * a + r * r + g * g + b * b;
        }

        /// <summary>
        /// 40x40 clipping mask for getting colors. Should be good enough for each 40x40 cell.
        /// </summary>
        private byte[,] clippingMask = {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, }
        };
    }

    internal class WindowNotFoundException : Exception
    { }
}