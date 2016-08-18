using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace apr.cpr_Express
{
    public partial class registrForm : Form
    {
        #region sqlTools
        SqlConnection connect = new SqlConnection("Data Source=DESKTOP-6P1648G;Initial Catalog=apr.cprExpress;User ID=coder;Password=sqlCoder");
        SqlCommand cmd;
        string manager = "m";
        string employee = "e";
        #endregion

        PopUpMessage popUp = new PopUpMessage();
        public registrForm()
        {
            InitializeComponent();
        }

        #region button design
        private void btnSignUp_MouseHover(object sender, EventArgs e)
        {
            btnSignUp.BackgroundImage = Properties.Resources.loginBlur;
        }

        private void btnSignUp_MouseLeave(object sender, EventArgs e)
        {
            btnSignUp.BackgroundImage = Properties.Resources.login;
        }

        private void btnCancelSignUp_MouseHover(object sender, EventArgs e)
        {
            btnCancelSignUp.BackgroundImage = Properties.Resources.closeBlur;
        }

        private void btnCancelSignUp_MouseLeave(object sender, EventArgs e)
        {
            btnCancelSignUp.BackgroundImage = Properties.Resources.close;
        }

        #endregion

        #region registration
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSignUpLogin.Text))
                {
                    popUp.text("Please check your Name","");
                    txtSignUpLogin.Focus();
                    popUp.BringToFront();
                    return;
                }
                if (string.IsNullOrEmpty(txtNickName.Text))
                {
                    popUp.text("Please check your username", "");
                    txtNickName.Focus();
                    popUp.BringToFront();
                    return;
                }
                if (string.IsNullOrEmpty(txtSignUpPass.Text))
                {
                    popUp.text("Please check your password","");
                    txtSignUpPass.Focus();
                    popUp.BringToFront();
                    return;
                }

                // Check if the username is not taken (username, not the full name)
                using (ExpressEntities ent = new ExpressEntities())
                {
                  
                    var nickName = from nick in ent.Users where nick.nickName == txtNickName.Text select nick;
                    // if not null - means an item was selected, which means username is taken.
                    if (nickName.SingleOrDefault()!=null) 
                    {
                        popUp.text("please, choose another username.", "");
                        txtNickName.Focus();
                        popUp.BringToFront();
                        return;
                    }
                }
                // After check, we can open our connection and start the registration
                connect.Open();
                if (chkBoxSuperviser.Checked) // if it's true, the new user is going to be one of administrators.
                    {
                            // so we add him as "manager"
                        cmd = new SqlCommand("insert into Users(Username,Password,Status,nickName)values('" + txtSignUpLogin.Text + "','" + txtSignUpPass.Text + "','" + manager + "','" + txtNickName.Text + "')", connect);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                                // and here as "employee"
                        cmd = new SqlCommand("insert into Users(Username,Password,Status,nickName)values('" + txtSignUpLogin.Text + "','" + txtSignUpPass.Text + "','" + employee + "','" + txtNickName.Text + "')", connect);
                        cmd.ExecuteNonQuery();
                    }
                connect.Close();
                popUp.text("You've registrated correctly","");
                this.Close();
            }
            catch(Exception msj)
            {
                popUp.text("oups, try again...", "");
                MessageBox.Show(msj.ToString());
            }
        }
        #endregion

        #region events
        /// <summary>
        /// Some events to work with keyboard, no mouse using.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelSignUp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSignUpLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                txtSignUpPass.Focus();
            }
        }

        private void txtSignUpPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                txtNickName.Focus();
            }
        }
        #endregion
    }
}
