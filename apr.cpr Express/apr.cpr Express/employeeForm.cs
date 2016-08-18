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
using System.Runtime.InteropServices;
using System.Data.SqlClient;


namespace apr.cpr_Express
{
    public partial class employeeForm : Form
    {
        #region dragMove
        //  Make the window dragable
        public const int WM_NCLBUTTONDOWN = 0xA1; public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);[DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void employeeForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

        #region sqlTools
        string connection = "Data Source=DESKTOP-6P1648G;Initial Catalog=apr.cprExpress;User ID=coder;Password=sqlCoder";
        #endregion

        PopUpMessage popUp = new PopUpMessage(); 
        Thread thread;
        public employeeForm()
        {
            InitializeComponent();
        }
        #region fill the main window
        private void employeeForm_Load(object sender, EventArgs e)
        {
            try
            {           //  Load data...
                Cursor = Cursors.WaitCursor;
                this.UpComingTableAdapter.Fill(this.apr_cpr___ExpressDataSet.UpComing);
                this.storageTableAdapter.Fill(this.apr_cpr___ExpressDataSet.Storage);
                this.reportViewerUpcoming.RefreshReport();
                lblUsername.Text = loginForm.userName; // Full name of a user
                Cursor = Cursors.Default;
            }
            catch
            {
                popUp.text("oups, try again...", "");
            }
        }
        #endregion

        #region button design
        private void btnStorage_MouseHover(object sender, EventArgs e)
        {
            btnStorage.BackgroundImage = Properties.Resources.storageBlur;
        }

        private void btnStorage_MouseLeave(object sender, EventArgs e)
        {
            btnStorage.BackgroundImage = Properties.Resources.storage;
        }

        private void btnUpComing_MouseHover(object sender, EventArgs e)
        {
            btnUpComing.BackgroundImage = Properties.Resources.upcomingBlur;
        }

        private void btnUpComing_MouseLeave(object sender, EventArgs e)
        {
            btnUpComing.BackgroundImage = Properties.Resources.upcoming;
        }

        private void btnLogout_MouseHover(object sender, EventArgs e)
        {
            btnLogout.BackgroundImage = Properties.Resources.logoutBlur;
        }

        private void btnLogout_MouseLeave(object sender, EventArgs e)
        {
            btnLogout.BackgroundImage = Properties.Resources.logout;
        }
        private void btnMesssanger_MouseHover(object sender, EventArgs e)
        {
            btnMesssanger.BackgroundImage = Properties.Resources.messangerBlur;
        }

        private void btnMesssanger_MouseLeave(object sender, EventArgs e)
        {
            btnMesssanger.BackgroundImage = Properties.Resources.messanger;
        }
        #endregion

        #region form button actions
        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        // Logout
        private void btnLogout_Click(object sender, EventArgs e)
        {
     // To run some APP (tht is closed, not just hidden) - you can use Threading first, and then close the main window.
            try
            {
                thread = new Thread(runLoginForm);
                thread.Start();
                this.Close();
            }
            catch { }
        }
        private void runLoginForm()
        {
            Application.Run(new loginForm());
        }

        // Open a messanger
        private void btnMesssanger_Click(object sender, EventArgs e)
        {
            thread = new Thread(runMessanger);
            thread.Start();
        }
        void runMessanger()
        {
            Application.Run(new messanger());
        }
        #endregion

        #region pages
        /// <summary>
        /// 
        /// Hole this part could be in some switch or if-else. But it takes pretty much as many lines of code as this way.
        /// So we  leave it like this, just to keep it as simple as posible.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        // Storage
        private void btnStorage_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                panelStorage.Visible = true;
                panelUpComing.Visible = false;
                lblStorage.Visible = true;
                lblUpComing.Visible = false;
                Cursor = Cursors.Default;
            } catch { }
        }

        // UpComing
        private void btnUpComing_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                panelUpComing.Visible = true;
                panelStorage.Visible = false;
                lblUpComing.Visible = true;
                lblStorage.Visible = false;
                Cursor = Cursors.Default;
            }catch { }
        }
        #endregion

        #region binding navigator elements
        /// <summary>
        /// 
        /// Some tools (buttons) to work with data "Save, Delete, Add ...".  
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void storageBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();                                       
            this.storageBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
        }
        private void storageNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.storageBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
        }
        #endregion


        #region save data

        /// <summary>
        /// 
        /// To manage the system memory, it's better work with keyword "using", instead of declaring all this objects.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        // save new data into Storage table
        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                using (SqlConnection cn = new SqlConnection(connection))
                {
                    using (SqlDataAdapter adapterStorage = new SqlDataAdapter("select * from Storage", connection))
                    {
                        using (DataTable dtStorage = new DataTable("Storage"))
                        {
                            using (SqlCommand cmd = new SqlCommand("insert into Storage(Product,Quantity,Via,Customer,Price,Date,Country,Address)values('" + txtProduct.Text + "','" + txtQuantity.Text + "','" + txtVia.Text + "','" + txtCustomer.Text + "','" + txtPrice.Text + "','" + dateTimePicker.Text + "','" + txtCountry.Text + "','" + txtAddress.Text + "')", cn))
                            {
                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();
                                clearTextBoxes();
                                txtProduct.Focus();
                                popUp.text("Saved","");
                                storageBindingSource.EndEdit();
                                storageDataGridView.EndEdit();
                                storageDataGridView.Update();
                                storageDataGridView.Refresh();
                                dtStorage.Clear();
                                adapterStorage.Fill(dtStorage);

                                // To re-fill the adapter, we don't use --DataSource-- method.
                                this.storageTableAdapter.Fill(this.apr_cpr___ExpressDataSet.Storage);
                                // We use the same source we used at the begining to run the app (without re-bind the source).
                                // Becouse some elements migh stop work. In this case Binding Navidators.
                            }
                        }
                    }
                }
            }
            catch
            {
                popUp.text("oups, try again...","");
            }
            Cursor = Cursors.Default;
        }
        #endregion

        #region cleanUp textBoxes
        void clearTextBoxes()
        {
            txtAddress.Clear();
            txtCustomer.Clear();
            txtCountry.Clear();
            txtPrice.Clear();
            txtProduct.Clear();
            txtQuantity.Clear();
            txtVia.Clear();
            dateTimePicker.Value = DateTime.Now;
        }
        #endregion

        #region search in datagrid
        // search in Storage table
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                switch (cboBoxSearch.Text)
                {

                    // And again - don not use  common "DataSource, RowFilter" methods. Read the line 239.

                    case ("Product"):
                        storageBindingSource.Filter = string.Format("Product like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Quantity"):
                        storageBindingSource.Filter = string.Format("Quantity like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Via"):
                        storageBindingSource.Filter = string.Format("Via like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Customer"):
                        storageBindingSource.Filter = string.Format("Customer like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Price"):
                        storageBindingSource.Filter = string.Format("Price like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Date"):
                        storageBindingSource.Filter = string.Format("Date like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Country"):
                        storageBindingSource.Filter = string.Format("Country like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Address"):
                        storageBindingSource.Filter = string.Format("Address like '%{0}%'", txtSearch.Text);
                        break;
                }
            }
            catch
            {
                popUp.text("oups, try again...","");
            }
            txtSearch.Clear();
            Cursor = Cursors.Default;
        }
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                btnSearch.PerformClick();
                txtSearch.Clear();
            }
        }
        #endregion    
    }
}
