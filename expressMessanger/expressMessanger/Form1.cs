using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.IO;

namespace expressMessanger
{
    /// <summary>
    /// 
    /// Well, the server does not have any controls. It just works, doing his job..))
    ///
    /// </summary>
    public partial class Form1 : Form
    {
        #region entry point
        server serv = new server();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                serv.startListening();
                lblStatus.Text = "started";
            }
            catch { MessageBox.Show("Please run the server again"); this.Close(); }

        }
        #endregion
    }



    /// <summary>
    /// 
    /// Container class. Working with existing clients.
    /// </summary>
    class server
    {
        #region tools
        public static Hashtable hashTUsers = new Hashtable(100);// Users container. 
        // we gonna use this dictionary, to be able to remove users from 'hashtUsers'
        public static Hashtable hashTConnections = new Hashtable(100);
        TcpClient tcpClient;
        Thread thrListener;
        TcpListener tcpListener;
        static StreamWriter sTW;  
        static TcpClient[] clients; 
        #endregion

        #region add user
        public static void addUser(TcpClient user, string name)
        {
            hashTUsers.Add(name, user);// the 'key' is a string(name)
            hashTConnections.Add(user, name);// the 'key' is an object(TcpClient)
        }
        #endregion

        #region remove user
        public static void removeUser(TcpClient user)
        {
            if (hashTConnections[user] != null)// one more check, just in case
            {
                sendMessage(hashTConnections[user].ToString(), " - is offline.");
                // Here is the trick...
                // The hashTConnections[user] is the key that returns the value of type string - 'name'
                // We need that string to remove the current user from hashTUsers container
                hashTUsers.Remove(hashTConnections[user]); 
                hashTConnections.Remove(user); 
            }
        }
        #endregion

        #region send message
        public static void sendMessage(string from, string message)
        {
            clients = new TcpClient[hashTUsers.Count]; // 'Total' of connected users.
            hashTUsers.Values.CopyTo(clients, 0);// copy just the objects 'TcpClient', without names
            for (int i = 0; i < clients.Length; i++)
            {
                try
                {
                    // the message IS filtered, but again .. just in case.
                    // if the massege is  null, or contains nothing but white spaces
                    if (message.Trim() == "" || clients[i] == null) 
                    {
                        continue;
                    }
                    sTW = new StreamWriter(clients[i].GetStream());
                    sTW.WriteLine(from + ":   " + message); 
                    sTW.Flush();
                }
                catch
                {
                    removeUser(clients[i]); // any problems with the client - the client get removed.
                }
            }
        }
        #endregion

        #region start
        public void startListening()
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            tcpListener.Start();  
            thrListener = new Thread(listen);
            thrListener.Start();
        }

        // looking for client
        void listen()
        {
            while (true)
            {
                tcpClient = tcpListener.AcceptTcpClient();  // got one
                filtr c = new filtr(tcpClient);// now we have to filter him

            }
        }
        #endregion

    }



    /// <summary>
    /// 
    /// Here we filtering all tasks to work with and process the client
    /// </summary>
    class filtr
    {
        #region tools
        TcpClient tcpClient;
        Thread thrSend;
        StreamReader sTR;
        StreamWriter sTW;
        string currentUser;
        string respomse;
        #endregion

        #region accept client
        public filtr(TcpClient client)
        {
            tcpClient = client;
            thrSend = new Thread(acceptClient);
            thrSend.Start();
        }
        void acceptClient()
        {
            sTR = new StreamReader(tcpClient.GetStream());
            sTW = new StreamWriter(tcpClient.GetStream());
            currentUser = sTR.ReadLine();
            if (currentUser == "info")  // in this case, just send the conected clients lit.
            {
                foreach (string conectedUser in server.hashTUsers.Keys)
                {
                    sTW.WriteLine(conectedUser);
                    sTW.Flush();
                }
                return;// we do not include the 'info' user to the list.
            }

            server.addUser(tcpClient, currentUser); // if username is not ' info'  - start processing
            server.sendMessage(currentUser, " - is online.");

            try
            {
                while ((respomse = sTR.ReadLine()) != "")// validate the message
                {
                    if (respomse == null)// null it's not 'empty', it means the user is gone. There is no response.
                    {
                        server.removeUser(tcpClient);// remove
                        return;// HERE IF YOU DO NOT EXIT FROM THE LOOP, IT'LL GET HIS REVANGE ON CPU )))))
                    }
                    server.sendMessage(currentUser, respomse);//  if the message is valid
                }
            }
            catch
            {
                server.removeUser(tcpClient);// here,  we don't handle the errors - we remove them.)
            }
        }
        #endregion
    }
}

