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
    public partial class loginForm : Form
    {
        Thread thread;
        PopUpMessage popUp = new PopUpMessage();
        public static string userName; // Full name showed on platform.
        public static string nickName; // Nick name showed in massenger.
        public loginForm()
        {
            InitializeComponent();
        }

        #region button design
        private void btnLogin_MouseHover(object sender, EventArgs e)
        {
            btnLogin.BackgroundImage = Properties.Resources.loginBlur;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackgroundImage = Properties.Resources.login;
        }

        private void btnClose_MouseHover(object sender, EventArgs e)
        {
            btnClose.BackgroundImage = Properties.Resources.closeBlur;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackgroundImage = Properties.Resources.close;
        }

        private void loginForm_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region Log in
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLogin.Text))
                {
                    this.Close();
                    thread = new Thread(runPopUp);
                    thread.Start();
                    txtLogin.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    thread = new Thread(runPopUpPassword);
                    thread.Start();
                    this.Close();
                    txtPassword.Focus();
                    return;
                }

                using (ExpressEntities ent = new ExpressEntities())
                {   
                    // Esxtracting data from database.
                    var manager = from man in ent.Users where man.nickName == txtLogin.Text && man.Password == txtPassword.Text && man.Status == "m" select man;
                    if(manager.SingleOrDefault()!=null)
                    {
                        // Extracting the nick name from "manager" Enumerable.
                        var nameM = (from ma in manager where ma.Username != ""&&ma.nickName!="" select ma).FirstOrDefault();

                        userName = nameM.Username;
                        nickName = nameM.nickName;
                        this.Close();
                        thread = new Thread(runManagerForm);  // In case the user is one fo supervisers.
                        thread.Start();
                        return;
                    }
                    // the same, but now checking for a user with "employee" status
                    var employee = from emp in ent.Users where emp.nickName == txtLogin.Text && emp.Password == txtPassword.Text && emp.Status == "e" select emp;
                    if(employee.SingleOrDefault()!=null)
                    {
                        var nameE = (from em in employee where em.Username != "" select em).FirstOrDefault();
                        
                        userName = nameE.Username;
                        nickName = nameE.nickName;
                        this.Close();
                        thread = new Thread(runEmployeeForm);
                        thread.Start();
                        return;
                    }

                    // iF no user found...
                    if(manager.SingleOrDefault()==null&&employee.SingleOrDefault()==null)
                    {
                        popUp.text("The username or password","are incorrect");
                    }
                }
            }
            catch
            {
                popUp.text("oups, try again...","");
            }
        }
        #endregion

        #region events
        private void runManagerForm()
        {
            Application.Run(new managerForm());
        }

        private void runPopUpPassword()
        {
            Application.Run(new popUpPassword());
        }

        private void runPopUp()
        {
            Application.Run(new popUp());
        }

        private void runEmployeeForm()
        {
            Application.Run(new employeeForm());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnLogin.PerformClick();
            }
        }
        #endregion
    }
}
