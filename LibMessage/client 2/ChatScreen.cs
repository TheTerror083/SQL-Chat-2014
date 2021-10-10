using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibMessage;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace client
{
    public partial class ChatScreen : Form
    {
        SqlConnection Connection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");
        SqlDataAdapter Adapter = new SqlDataAdapter();
        SqlCommandBuilder CommandBuilder;
        BinaryFormatter Formatter = new BinaryFormatter();
        ChatMessage MyProfile = new ChatMessage();
        TcpClient client = new TcpClient();
        NetworkStream MyNetStream;
        Thread RecieveMessages;
        bool FirstConnection = true;
        bool ConnectionFinished = false;
        DataTable MyDataTable = new DataTable();
        public ChatScreen()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            btnSend.Enabled = false;
            btnDisconnect.Enabled = false;
            richTextBox1.ReadOnly = true;
            UploadImg_Btn.Enabled = false;
            BtnClearSel.Enabled = false;
            this.AcceptButton = btnSend;
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (MsgTextBox.Text != string.Empty)      // if the messgae textbox has a text written in it
            {
                if (ClientListBox.SelectedItem != null) // if the user has selected to send a private message to another user
                {
                    richTextBox1.SelectionColor = MyProfile.color;        // sets the font's color
                    MyProfile.Msg = "[To: " + ClientListBox.SelectedItem.ToString() + "] " + MsgTextBox.Text;   // sends the user a private message
                }
                else
                {
                    richTextBox1.SelectionColor = MyProfile.color;
                    MyProfile.Msg = MsgTextBox.Text;       // sends a publice message
                }
                Formatter.Serialize(MyNetStream, MyProfile);
                MsgTextBox.Clear();
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)  // NOTE: if the user disconnects without closing the client, it will still be marked as Connected in the database to avoid other users connecting to the same account
        {
            MyProfile.Msg = MyProfile.UserName + " has Disconnected \n";   // contacts the server that this user has disconnected but didnt close the client
            Formatter.Serialize(MyNetStream, MyProfile);
            ClientListBox.Items.Clear();    // clears the online clients list
            RecieveMessages.Suspend();
            // ***************************** disable all buttons and enable the Connect button
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
            btnConnect.Enabled = true;
            UploadImg_Btn.Enabled = false;
            BtnClearSel.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            LoginScreen LoginWindow = new LoginScreen();
            if (FirstConnection == true)       // if it is the frist time this client connects to the server
            {
            connecting:
                LoginWindow.ShowDialog();            // display the login window
                if (LoginWindow.ConnectionCancelled == false)   // if the user did not close the login window
                {
                    MyProfile.UserName = LoginWindow.userName;           //set the client's username
                    MyProfile.color = LoginWindow.textcolor;            //set the client's text color
                    MyProfile.UserID = LoginWindow.UserID;
                    try
                    {
                        client = new TcpClient();
                        client.Connect(LoginWindow.ipAddress, LoginWindow.port);  // recieves the prot and ip address entered by the user at the login window
                        MyNetStream = client.GetStream();
                        this.Text = MyProfile.UserName;      // sets the name of this form as the user's username
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An Error has occured");           // if the client fails to connect it displays an error message
                        goto connecting;                                  // return to the login window. all filled textboxes remain but their content is not saved on the server
                    }

                    MyProfile.Msg = MyProfile.UserName + " has Connected" + "\n";           // contacts the server that this client has connected
                    Formatter.Serialize(MyNetStream, MyProfile);
                    RecieveMessages = new Thread(GetMessage);                         // starts the GetMessage loop with a Thread, this loop reads data recieved from the server
                    RecieveMessages.Start();
                    ConnectionFinished = true;
                }
            }

            if (FirstConnection == false)                  // if the client has already connected before
            {
                MyProfile.Msg = MyProfile.UserName + " has Reconnected \n";     // contacts the server that this client has reconnected        
                Formatter.Serialize(MyNetStream, MyProfile);
                RecieveMessages.Join(200);                 // the GetMessage loop will wait until the server finishes reconnecting this client 
                RecieveMessages.Resume();                    // reactivate the GetMessage loop
            }
            if (ConnectionFinished == true)   // when the client has finished connecting, all buttons are enabled and the Connect button is disabled
            {
                Adapter = new SqlDataAdapter("SELECT * FROM ChatTable WHERE ID=" + MyProfile.UserID + " ", Connection);
                Adapter.Fill(MyDataTable);
                MyDataTable.Rows[0].BeginEdit();
                MyDataTable.Rows[0]["IsConnected"] = 1;   // writes that this user is connected in the database 
                MyDataTable.Rows[0]["LastConnectionDate"] = DateTime.Now; // sets the user's last connection date to the current date
                MyDataTable.Rows[0].EndEdit();
                CommandBuilder = new SqlCommandBuilder(Adapter);
                Adapter.Update(MyDataTable);
                //*************************************** enable all buttons and disable the register and connect buttons
                UploadImg_Btn.Enabled = true;
                FirstConnection = false;
                btnSend.Enabled = true;
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
                BtnClearSel.Enabled = true;
                RegisterBtn.Enabled = false;
            }

        }

        void GetMessage() // reads the messages recieved from the server
        {
            try
            {
                while (true)
                {
                    ChatMessage RecievedData = (ChatMessage)Formatter.Deserialize(MyNetStream);                        // reads data recieved from the server
                    richTextBox1.Invoke(new Action(() =>
                    {
                        richTextBox1.SelectionColor = RecievedData.color;
                        richTextBox1.SelectedText += RecievedData.UserName + ": " + RecievedData.Msg + "\n";        // writes the sender's name along with his message
                        if (RecievedData.Msg == RecievedData.UserName + " has uploaded an Image \n")         // if another client has uploaded an Image
                        {
                            pictureBox1.Image = new Bitmap(RecievedData.UploadedImage);          // the image will be displayed at the picturebox
                            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;           // resize the image to match the picturebox's size
                        }
                        if (RecievedData.Msg == RecievedData.UserName + " has Disconnected \n" || RecievedData.Msg == RecievedData.UserName + " has Quit \n"               // if a user has disconnected, closed the client
                            || RecievedData.Msg == RecievedData.UserName + " has Connected" + "\n" || RecievedData.Msg == RecievedData.UserName + " has Reconnected \n")  // connected or reconnected
                        {
                            ClientListBox.Items.Clear();    // this client's online client list will be cleared and refilled with the currently online clients
                            for (int i = 0; i < RecievedData.ClientList.Count; i++)  // this loop fills the ClienListBox with the currently online clients. it occurs every time a message is received
                            {
                                if (RecievedData.ClientList[i] != MyProfile.UserName) // prevents this client's from appearing on list 
                                {
                                    ClientListBox.Items.Add(RecievedData.ClientList[i]); //adds the online clients' name to the list so this client can send them private messages
                                }
                            }
                        }
                    }));
                }
            }
            catch (System.Runtime.Serialization.SerializationException)   // a serilization exception may occur if the server has been shut down while the client is active
            {
                MessageBox.Show("Connection lost.. (server is offline?) Exitting chat...");
                Application.Exit();
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e) // if this client has been closed
        {
            //try
            //{
                if (client.Client.Connected)
                {
                    Adapter = new SqlDataAdapter("SELECT * FROM ChatTable WHERE ID=" + MyProfile.UserID + " ", Connection);
                    Adapter.Fill(MyDataTable);
                    MyDataTable.Rows[0].BeginEdit();
                    MyDataTable.Rows[0]["IsConnected"] = 0;    // mark the client as NOT Connected in the database
                    MyDataTable.Rows[0].EndEdit();
                    CommandBuilder = new SqlCommandBuilder(Adapter);
                    Adapter.Update(MyDataTable);
                    //**********************************
                    MyProfile.Msg = MyProfile.UserName + " has Quit \n";   // contacts the server that this client has been closed
                    Formatter.Serialize(MyNetStream, MyProfile);
                    RecieveMessages.Abort();            // aborts the GetMessage loop
                    MyNetStream.Close();
                    client.Close();
                }
            //}
            //catch (Exception)
            //{
            //}
            Application.Exit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ChooseImage.Filter = "JPEG|*.jpg";                              // the image selection window only allows to upload JPEG type images
            if (ChooseImage.ShowDialog() != DialogResult.Cancel)          // if the user didnt cancel the image selection window
            {
                Image ChosenFile = new Bitmap(ChooseImage.FileName);
                MyProfile.UploadedImage = ChosenFile;
                MyProfile.Msg = MyProfile.UserName + " has uploaded an Image \n";           // conatcts the server that this client has uploaded an image
                Formatter.Serialize(MyNetStream, MyProfile);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            ClientListBox.ClearSelected();                        // clears this client's clientlist selection, allows to send public messages instead of private ones
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            MsgTextBox.Focus();       // if the user clicks on the richtextbox, the caret will be moved to the Message textbox. this is done to avoid writing disorders
        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            RegisterScreen registration = new RegisterScreen();
            registration.ShowDialog();
        }
    }
}
