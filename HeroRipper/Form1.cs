using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace HeroRipper
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void DOTA2_Click(object sender, EventArgs e)
        {
            DOTA2 dota = new DOTA2();
            dota.Rip(this);
        }

        public void SetOutput(string text)
        {
            richTextBox1.AppendText(text);
            richTextBox1.ScrollToCaret();
        }
    }
}
