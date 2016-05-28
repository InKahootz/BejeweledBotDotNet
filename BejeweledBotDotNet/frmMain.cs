using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetBejewelledBot
{
    public partial class frmMain : Form
    {
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int START_HOTKEY_ID = 1;
        private BejeweledWindowManager m_BWM;
        private int tick = 0;
        private bool botRunning = false;

        public frmMain()
        {
            InitializeComponent();
            m_BWM = new BejeweledWindowManager();
            BejeweledColor.Collection = new List<Color>();
            BejeweledColor.Collection.Add(BejeweledColor.Blue);
            BejeweledColor.Collection.Add(BejeweledColor.Green);
            BejeweledColor.Collection.Add(BejeweledColor.Orange);
            BejeweledColor.Collection.Add(BejeweledColor.Purple);
            BejeweledColor.Collection.Add(BejeweledColor.Red);
            BejeweledColor.Collection.Add(BejeweledColor.White);
            BejeweledColor.Collection.Add(BejeweledColor.Yellow);
            BejeweledColor.Collection.Add(BejeweledColor.Black);
            RegisterHotKey(this.Handle, START_HOTKEY_ID, 0, (int)Keys.F2);
        }

        private void screenGrabTimer_Tick(object sender, EventArgs e)
        {
            tick++;
            m_BWM.GetScreenshot();
            m_BWM.GetColourGrid();
            m_BWM.CalculateMoves();
            pictureBox1.Image = m_BWM.ColourGrid;
        }

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            m_BWM.GetScreenshot();
            if (m_BWM.Calibrate())
            {
                pictureBox1.Image = m_BWM.ScreenShot;
                //MessageBox.Show("Calibrated");
            }
            else
            {
                MessageBox.Show("Couldn't calibrate");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            screenGrabTimer.Start();
            btnStart.Enabled = false;
            botRunning = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == START_HOTKEY_ID)
            {
                if (botRunning)
                {
                    botRunning = false;
                    screenGrabTimer.Stop();
                    tick = 0;
                    btnStart.Enabled = true;
                }
                else
                {
                    botRunning = true;
                    screenGrabTimer.Start();
                    btnStart.Enabled = false;
                }
            }
            base.WndProc(ref m);
        }

        private void ColorGridize_Click(object sender, EventArgs e)
        {
            m_BWM.GetColourGrid();
            pictureBox1.Image = m_BWM.ColourGrid;
        }

        private void ScreenShot_Click(object sender, EventArgs e)
        {
            m_BWM.GetScreenshot();
            pictureBox1.Image = m_BWM.ScreenShot;
        }
    }
}
