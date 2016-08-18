using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace apr.cpr_Express
{
    public partial class popUp : Form
    {
        Thread thread;
        public popUp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            thread = new Thread(runLoginForm);
            thread.Start();
        }

        private void runLoginForm()
        {
            Application.Run(new loginForm());
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.okBlur;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.ok;
        }
    }
}
