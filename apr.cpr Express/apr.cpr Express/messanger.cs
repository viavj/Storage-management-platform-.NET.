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
using System.IO;
using System.Net.Sockets;

namespace apr.cpr_Express
{
    public partial class messanger : Form
    {
        #region tools
        TcpClient tcpServer; // Now the server is our client
        StreamReader sTR;
        StreamWriter sTW;
        Thread thrMessanging;
        Thread thread;
        messageRoom msgRoom = new messageRoom();
        PopUpMessage popUp = new PopUpMessage();
        string username = "info";    // Used just once at the begining, to request the  conected users's list
        const string ipAddress = "127.0.0.1";
        const int port = 8888;
        delegate void callBack(string delMessage);
        #endregion

        public messanger()
        {
            InitializeComponent();
        }

        #region button design
        private void btnEnter_MouseHover(object sender, EventArgs e)
        {
            btnEnter.BackgroundImage = Properties.Resources.enterBlur;
        }

        private void btnEnter_MouseLeave(object sender, EventArgs e)
        {
            btnEnter.BackgroundImage = Properties.Resources.enter;
        }
        #endregion

        #region start
        private void messanger_Load(object sender, EventArgs e)
        {
            try
            {
                connect();
            }
            catch
            {
                popUp.text("oups, try again...", "");
            }
            // window position
            Rectangle wArea = Screen.GetWorkingArea(this);
            this.Location = new Point(wArea.Right - Size.Width - 50, wArea.Bottom - Size.Height - 150);
        }
        #endregion

        #region handshake with the server
        void connect()
        {
            tcpServer = new TcpClient();
            tcpServer.Connect(ipAddress, port);
            sTW = new StreamWriter(tcpServer.GetStream());
            sTW.WriteLine(username); 
            sTW.Flush();
            thrMessanging = new Thread(receiveMessage);
            thrMessanging.Start();
        }

        void receiveMessage()
        {
            try
            {
                sTR = new StreamReader(tcpServer.GetStream());
                while (true)
                {
                    Invoke(new callBack(showUsers), new object[] { sTR.ReadLine() }); // show the message
                }
            }
            catch { } // do not show nothing, It's looped in thread, the program will try again until success
        }
        void showUsers(string msj)
        {
            txtUsers.AppendText(msj + "\r\n");
        }
        #endregion

        #region open messagingRoom
        private void btnEnter_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // Cleaning up.
                thrMessanging.Abort();
                sTW.Close();
                sTR.Close();
                tcpServer.Close();
                this.Close();
                // ----------

                thread = new Thread(runMessageRoom);
                thread.Start();
            }
            catch
            {
                popUp.text("oups, try again...", "");
            }
            Cursor = Cursors.Default;
        }
        void runMessageRoom()
        {
            Application.Run(new messageRoom());
        }
        #endregion
    }
}
