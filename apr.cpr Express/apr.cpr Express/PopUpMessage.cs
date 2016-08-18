using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apr.cpr_Express
{
    public partial class PopUpMessage : Form
    {
        public PopUpMessage()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnOk_MouseHover(object sender, EventArgs e)
        {
            btnOk.BackgroundImage = Properties.Resources.okBlur;
        }

        // Some flexible method to popup messages. It takes 2 parameters, the second is optionally.
        public  void text(string s, string s2)
        {
            try
            {
                lblMessage.Text = s;
                lblMessage2.Text = s2;
                this.Show();
            }
            catch { }
        }
       
        private void btnOk_MouseLeave(object sender, EventArgs e)
        {
            btnOk.BackgroundImage = Properties.Resources.ok;
        }
    }
}
