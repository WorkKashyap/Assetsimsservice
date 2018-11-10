using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIMSServices
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            timer1 = new Timer();

            timer1.Interval = 6000;
            timer1.Start();
            timer1.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            SIMSServiceProject.SIMSSettings mf = new SIMSServiceProject.SIMSSettings();
            mf.Show();

            //hide this form

            this.Hide();
        }
    }
}
