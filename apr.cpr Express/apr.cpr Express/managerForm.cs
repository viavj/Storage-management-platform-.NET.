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
using System.Windows.Forms.DataVisualization.Charting;

namespace apr.cpr_Express
{
    public partial class managerForm : Form
    {
        #region dragMove
        //  Make the window dragable
        public const int WM_NCLBUTTONDOWN = 0xA1; public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);[DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void managerForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

        #region sqlTools
        string connection = "Data Source=DESKTOP-6P1648G;Initial Catalog=apr.cprExpress;User ID=coder;Password=sqlCoder";
        #endregion

        PopUpMessage popUp = new PopUpMessage();
        Thread thread;

        public managerForm()
        {
            InitializeComponent();
        }

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

        private void btnOffice_MouseHover(object sender, EventArgs e)
        {
            btnOffice.BackgroundImage = Properties.Resources.officeBlur;
        }

        private void btnOffice_MouseLeave(object sender, EventArgs e)
        {
            btnOffice.BackgroundImage = Properties.Resources.office;
        }

        private void btnAddUser_MouseHover(object sender, EventArgs e)
        {
            btnAddUser.BackgroundImage = Properties.Resources.adduserBlur;
        }

        private void btnAddUser_MouseLeave(object sender, EventArgs e)
        {
            btnAddUser.BackgroundImage = Properties.Resources.adduser;
        }

        private void btnLogout_MouseHover(object sender, EventArgs e)
        {
            btnLogout.BackgroundImage = Properties.Resources.logoutBlur;
        }

        private void btnLogout_MouseLeave(object sender, EventArgs e)
        {
            btnLogout.BackgroundImage = Properties.Resources.logout;
        }
        private void btnMessanger_MouseHover(object sender, EventArgs e)
        {
            btnMessanger.BackgroundImage = Properties.Resources.messangerBlur;
        }

        private void btnMessanger_MouseLeave(object sender, EventArgs e)
        {
            btnMessanger.BackgroundImage = Properties.Resources.messanger;
        }

        #endregion

        #region fill the main window
        private void managerForm_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {           //  Load data...
                this.ReportTableAdapter.Fill(this.apr_cpr___ExpressDataSet.Report);
                this.storageTableAdapter.Fill(this.apr_cpr___ExpressDataSet.Storage);
                this.upComingTableAdapter.Fill(this.apr_cpr___ExpressDataSet.UpComing);
                this.reportViewerProfit.RefreshReport();
                lblUsername.Text = loginForm.userName;// Full name of a user
            }
            catch { }
            Cursor = Cursors.Default;
        }
        #endregion

        #region form buttonActions
        private void btnMinimise_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Add a new user to database
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                registrForm rf = new registrForm();
                rf.Show();
                rf.BringToFront();
            }
            catch { }
        }

        // Logout
        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
     // To run some APP (tht is closed, not just hidden) - you can use Threading first, and then close the main window.
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
        private void btnMessanger_Click(object sender, EventArgs e)
        {
            thread = new Thread(runMessanger);
            thread.Start();
        }
        void runMessanger()
        {
            Application.Run(new messanger());
        }
        #endregion

        #region Report View Charts
        void report()
        {
            // This is something you better put in try-catch Always, becouse you are working with database.
            // But in this case -  below (where it's actualy called) it IS in try-catch. 
            using (ExpressEntities ent = new ExpressEntities())
            {
                chartProfit.DataSource = ent.Reports.ToList();
                chartProfit.Series["Profit"].XValueMember = "Year";
                chartProfit.Series["Profit"].XValueType = ChartValueType.UInt32;
                chartProfit.Series["Profit"].YValueMembers = "Profit";
                chartProfit.Series["Profit"].YValueType = ChartValueType.Double;

                chartIncome.DataSource = ent.Reports.ToList();
                chartIncome.Series["Income"].XValueMember = "Year";
                chartIncome.Series["Income"].XValueType = ChartValueType.UInt32;
                chartIncome.Series["Income"].YValueMembers = "Income";
                chartIncome.Series["Income"].YValueType = ChartValueType.Double;
            }
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
                panelOffice.Visible = false;
                panelUpComing.Visible = false;
                lblStorage.Visible = true;
                lblUpComing.Visible = false;
                lblOffice.Visible = false;
                Cursor = Cursors.Default;
            }
            catch
            {
                popUp.text("oups, reload please.","");
            }
        }
        // UpComing
        private void btnUpComing_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                panelUpComing.Visible = true;
                panelStorage.Visible = false;
                panelOffice.Visible = false;
                lblUpComing.Visible = true;
                lblStorage.Visible = false;
                lblOffice.Visible = false;
                Cursor = Cursors.Default;
            }
            catch
            {
                popUp.text("oups, reload please.","");
            }
        }
        // Office
        private void btnOffice_Click(object sender, EventArgs e)
        {
            // Here we updating our charts, it may take a while. So we are changing a cursor. 
            Cursor = Cursors.WaitCursor; 
            try
            {
                Cursor = Cursors.WaitCursor;
                panelOffice.Visible = true;
                panelStorage.Visible = false;
                panelUpComing.Visible = false;
                lblOffice.Visible = true;
                lblStorage.Visible = false;
                lblUpComing.Visible = false;
                Cursor = Cursors.Default;
                report();
            }
            catch
            {
                popUp.text("oups,  please reload.","");
            }
            Cursor = Cursors.Default;
        }
        #endregion

        #region bindingNavigator elements

        /// <summary>
        /// 
        /// Some tools (buttons) to work with data "Save, Delete, Add ...".  
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void storageBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.storageBindingSource.EndEdit();
                this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
            }
            catch { }
        }
        private void storageBindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.storageBindingSource.EndEdit();
                this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
            }
            catch { }
        }
        private void upComingBindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.upComingBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
        }
        private void upComingBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.upComingBindingSource.EndEdit();
                this.tableAdapterManager.UpdateAll(this.apr_cpr___ExpressDataSet);
            }
            catch { }
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
        
            // save new report
        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                using (SqlConnection cn = new SqlConnection(connection))
                {
                        using (SqlCommand cmd = new SqlCommand("insert into Report(Year,Income,Highest,Fixed_Outcome,Profit)values('" + txtYear.Text + "','" + txtIncome.Text + "','" + txtHighest.Text + "','" + txtOutcome.Text + "','" + txtProfit.Text + "')", cn))
                        {
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                            popUp.text("saved","");
                            clearTextBoxes3();
                            this.ReportTableAdapter.Fill(this.apr_cpr___ExpressDataSet.Report);
                            reportViewerProfit.RefreshReport();
                            report();
                        }
                }
            }
            catch
            {
                popUp.text("oups, try again...","");
            }
            Cursor = Cursors.Default;
        }

        // save new data into Storage table
        private void btnSaveS_Click(object sender, EventArgs e)
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
                            using (SqlCommand cmd = new SqlCommand("insert into Storage(Product,Quantity,Via,Customer,Price,Date,Country,Address)values('" + txtProductS.Text + "','" + txtQuantityS.Text + "','" + txtViaS.Text + "','" + txtCustomerS.Text + "','" + txtPriceS.Text + "','" + dateTimePickerS.Text + "','" + txtCountryS.Text + "','" + txtAddressS.Text + "')", cn))
                            {
                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();
                                clearTextBoxes();
                                txtProductS.Focus();
                                popUp.text("saved","");
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

        // save new data into UpComing table
        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                using (SqlConnection cn = new SqlConnection(connection))
                {
                    using (SqlDataAdapter adapterUpComing = new SqlDataAdapter("select * from UpComing", connection))
                    {
                        using (DataTable dtUpComing = new DataTable("UpComing"))
                        {
                            using (SqlCommand cmd = new SqlCommand("insert into UpComing(Product,Quantity,Via,Country,Customer,Price,Date,Address)values('" + txtProduct.Text + "','" + txtQuantity.Text + "','" + txtVia.Text + "','" + txtCountry.Text + "','" + txtCustomer.Text + "','" + txtPrice.Text + "','" + dateTimePicker.Text + "','" + txtAddress.Text + "')", cn))
                            {
                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();
                                clearTextBoxes2();
                                txtProduct.Focus();
                                popUp.text("saved","");
                                upComingBindingSource.EndEdit();
                                upComingDataGridView.EndEdit();
                                upComingDataGridView.Update();
                                upComingDataGridView.Refresh();
                                dtUpComing.Clear();
                                adapterUpComing.Fill(dtUpComing);
                                this.upComingTableAdapter.Fill(this.apr_cpr___ExpressDataSet.UpComing);

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
        void clearTextBoxes2()
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
        void clearTextBoxes3()
        {
            txtYear.Clear();
            txtIncome.Clear();
            txtOutcome.Clear();
            txtHighest.Clear();
            txtProfit.Clear();
        }
        void clearTextBoxes()
        {
            txtAddressS.Clear();
            txtCustomerS.Clear();
            txtCountryS.Clear();
            txtPriceS.Clear();
            txtProductS.Clear();
            txtQuantityS.Clear();
            txtViaS.Clear();
            dateTimePickerS.Value = DateTime.Now;
        }
        #endregion

        #region search in datagrid
        // search in Storage table
        private void btnSearchS_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {

                // And again - don not use  common "DataSource, RowFilter" methods. Read the line 382.
                switch (cboBoxSearchS.Text)
                {
                    case ("Product"):
                        storageBindingSource.Filter = string.Format("Product like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Quantity"):
                        storageBindingSource.Filter = string.Format("Quantity like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Via"):
                        storageBindingSource.Filter = string.Format("Via like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Customer"):
                        storageBindingSource.Filter = string.Format("Customer like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Price"):
                        storageBindingSource.Filter = string.Format("Price like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Date"):
                        storageBindingSource.Filter = string.Format("Date like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Country"):
                        storageBindingSource.Filter = string.Format("Country like '%{0}%'", txtSearchS.Text);
                        break;
                    case ("Address"):
                        storageBindingSource.Filter = string.Format("Address like '%{0}%'", txtSearchS.Text);
                        break;
                }
            }
            catch
            {
                popUp.text("oups, try again...","");
            }
            Cursor = Cursors.Default;
            txtSearchS.Clear();
        }
        private void txtSearchS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearchS.PerformClick();
            }
        }

        // search in UpComing table
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                switch (cboBoxSearch.Text)
                {
                    case ("Product"):
                        upComingBindingSource.Filter = string.Format("Product like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Quantity"):
                        upComingBindingSource.Filter = string.Format("Quantity like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Via"):
                        upComingBindingSource.Filter = string.Format("Via like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Customer"):
                        upComingBindingSource.Filter = string.Format("Customer like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Price"):
                        upComingBindingSource.Filter = string.Format("Price like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Date"):
                        upComingBindingSource.Filter = string.Format("Date like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Country"):
                        upComingBindingSource.Filter = string.Format("Country like '%{0}%'", txtSearch.Text);
                        break;
                    case ("Address"):
                        upComingBindingSource.Filter = string.Format("Address like '%{0}%'", txtSearch.Text);
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
            if (e.KeyChar == (char)13)
            {
                btnSearch.PerformClick();
                txtSearch.Clear();
            }
        }
        #endregion
    }
}
