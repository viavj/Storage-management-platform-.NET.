using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace apr.cpr_Express
{
    public partial class messageRoom : Form
    {
        #region tools
        PopUpMessage popUp = new PopUpMessage();
        TcpClient tcpServer; // Now the server is our client
        Thread thrMessaging;
        StreamReader sTR;
        StreamWriter sTW;
        const string ipAddress = "127.0.0.1";
        const int port = 8888;
        string username = loginForm.nickName; // The nickname, from the very first window.
        delegate void callBack(string delMessage); 
        string notifyMessage;
        #endregion

        #region dragMove()
        public const int WM_NCLBUTTONDOWN = 0xA1; public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);[DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void messageRoom_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

        #region button design
        private void btnSend_MouseHover(object sender, EventArgs e)
        {
            btnSend.BackgroundImage = Properties.Resources.sendBlur;
        }

        private void btnSend_MouseLeave(object sender, EventArgs e)
        {
            btnSend.BackgroundImage = Properties.Resources.send;
        }
        #endregion

        public messageRoom()
        {
            InitializeComponent();
        }

        #region start
        private void messageRoom_Load(object sender, EventArgs e)
        {
            try
            {
                connect();
                lblNickName.Text = username; 
                txtMessage.Focus();
            }
            catch
            {
                popUp.text("oups, try again...", "");
            }
            // Window start position
            Rectangle wArea = Screen.GetWorkingArea(this);
            this.Location = new Point(wArea.Right - Size.Width, wArea.Bottom - Size.Height);
        }
        #endregion

        #region handshake with the server
        void connect()
        {
            tcpServer = new TcpClient();
            tcpServer.Connect(ipAddress, port);
            sTW = new StreamWriter(tcpServer.GetStream());
            sTW.WriteLine(username); //
            sTW.Flush();
            thrMessaging = new Thread(recieveMessage); // response
            thrMessaging.Start();
        }

        void recieveMessage()
        {
            try
            {
                sTR = new StreamReader(tcpServer.GetStream());
                while (true)
                {
                    Invoke(new callBack(updateChat), new object[] { sTR.ReadLine() }); // invoke the message to the textbox
                }
            }
            catch { } // do not show nothing, It's looped in thread, the program will try again until success

        }

        void updateChat(string msj)  // updating the chat room, using 'callback' delegate.
        {
            notifyMessage = msj;
            txtChat.AppendText(msj + "\r\n");
        }
        #endregion

        #region send message
        private void btnSend_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (txtMessage.Lines.Length > 0) // if someone is typing
                {
                    sTW.WriteLine(txtMessage.Text);
                    sTW.Flush();
                    txtMessage.Lines = null; // 
                    txtMessage.Focus();
                }
            }
            catch
            {
                popUp.text("oups, try again...", "");
            }
            Cursor = Cursors.Default;
        }
        #endregion

        // cleaning up
        private void btnClose_Click(object sender, EventArgs e)
        {
            thrMessaging.Abort();
            sTR.Close();
            sTW.Close();
            tcpServer.Close();
            this.Close();
        }
         

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtChat_TextChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.ShowBalloonTip(1000, "", notifyMessage, ToolTipIcon.None);
            }
            txtMessage.Focus();
        }
    }
}
