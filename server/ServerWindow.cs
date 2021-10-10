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
using System.IO;
using System.Threading;
using System.Data.SqlClient;
namespace server
{
    public partial class ServerWindow : Form
    {
        SqlConnection Connection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");       // this is the main connection used to write and read data from the database
        SqlConnection DataUpdateConnection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");  // this connection is used to update the datagridview seperately from the main connection
        BinaryFormatter formatter = new BinaryFormatter();
        DataTable data = new DataTable();                   // this is the datatable which contains all the data from the database
        SqlDataAdapter Adapter = new SqlDataAdapter();
        TcpListener listener;
        TcpClient client;
        NetworkStream Stream;
        Thread MessageReader, CheckClients, ClientAccepted,UpdateDataGrid;
        bool FirstClientConnected = false;
        List<TcpClient> connectedClients = new List<TcpClient>();                 // this list contains all clients' connections
        List<NetworkStream> ClientNetStreams = new List<NetworkStream>();        //this is a list that contains all client's Networkstreams
        List<string> ClientNames = new List<string>();                          // this is a list of all client's names
        List<string> OnlineClientNames = new List<string>();                   // this is a list of the currently online clients
        List<int> UserIDList = new List<int>();
        int connected_count;
        SqlCommand com;
        SearchWindow search;
        public ServerWindow()
        { 
            InitializeComponent();
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4000);            //starts the server on the specified port and ip
            listener.Start();
            CheckClients = new Thread(AcceptClients);
            CheckClients.Start();
            MessageReader = new Thread(ReadMessages);
            History_RichTextBox.ReadOnly = true;
            this.ClientsListView.View = View.Details;
            connected_count = connectedClients.Count;
            Adapter = new SqlDataAdapter("SELECT * FROM ChatTable", Connection);
            Adapter.Fill(data);
            DataGridView.DataSource = data;
            DataGridView.ReadOnly = true;
            UpdateDataGrid = new Thread(UpdateGrid);
            UpdateDataGrid.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)    // if the server has been closed
        {
            try
            {
                client.Close();
                listener.Stop();
                MessageReader.Abort();
                CheckClients.Abort();
                UpdateDataGrid.Abort();
                Application.Exit();
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }
        void ReadMessages()                     // this fuction reads message from all clients
        {
            ChatMessage RecievedMessage;
            while (FirstClientConnected==true)
            {
                try
                {
            start_reading:
                    MessageReader.Join(100);     // the message reader will wait to avid memory overuse
                    for (int i = 0; i < ClientNetStreams.Count; i++)            // this loop reads messages from each client's Networkstream
                    {
                        if (ClientNetStreams[i].DataAvailable)   // if a client's has sent data on his Networkstream (sent a message, uploaded a picture)
                        {
                            if (connectedClients.Count != connected_count) //if a user has connected or closed the client
                            {
                                MessageReader.Join(100);       // the MessageReader will wait until another thread finishes to deserialize the client's Networkstream
                                connected_count = connectedClients.Count;    // checks if the number of connected clients has changed since the last message
                                i = 0;
                                goto start_reading;    //the MessageReader loop will restart
                            }
                            RecievedMessage = (ChatMessage)formatter.Deserialize(ClientNetStreams[i]);   // reads the data from the client's Networkstream
                            if (RecievedMessage.Msg == RecievedMessage.UserName + " has Reconnected \n") // if a user has connected again
                            {
                                OnlineClientNames.Insert(i, RecievedMessage.UserName);            // his username will be added to the list of online clients
                                RecievedMessage.ClientList.Clear();                       //clears the sender's ClientList
                                for (int n = 0; n < OnlineClientNames.Count; n++)     // each client's "online clients" list will be updated
                                {
                                    RecievedMessage.ClientList.Add(OnlineClientNames[n]);
                                }
                                ClientsListView.Invoke(new Action(() =>
                                {
                                    History_RichTextBox.SelectedText += DateTime.Now + " " + RecievedMessage.UserName + " Reconnected" + "\n";
                                    ClientsListView.Items[i].Remove();
                                    ListViewItem items = new ListViewItem(new[] { RecievedMessage.UserName, "Online", DateTime.Now.ToString() });
                                    this.ClientsListView.Items.Insert(i, items);
                                }));
                            }
                            if (RecievedMessage.Msg == RecievedMessage.UserName + " has Disconnected \n")
                            {
                                OnlineClientNames.Remove(RecievedMessage.UserName);        // the client's name will be removed from the server's online clients list
                                RecievedMessage.ClientList.Clear();
                                for (int n = 0; n < OnlineClientNames.Count; n++)   // each client's "online clients" list will be updated
                                {
                                    if (OnlineClientNames[n] != RecievedMessage.UserName)
                                    {
                                        RecievedMessage.ClientList.Add(OnlineClientNames[n]);
                                    }
                                }
                                ClientsListView.Invoke(new Action(() =>
                                {
                                    History_RichTextBox.SelectedText += DateTime.Now + " " + RecievedMessage.UserName + " Disconnected" + "\n";
                                    ClientsListView.Items[i].Remove();
                                    ListViewItem items = new ListViewItem(new[] { RecievedMessage.UserName, "Offline", DateTime.Now.ToString() });
                                    this.ClientsListView.Items.Insert(i, items);                  // the client's name will be marked as "offline" in the listview
                                }));
                            }
                            if (RecievedMessage.Msg == RecievedMessage.UserName + " has Quit \n")               // if the client has completely left the chat
                            {
                                UserIDList.RemoveAt(i);
                                OnlineClientNames.Remove(RecievedMessage.UserName);
                                RecievedMessage.ClientList.Remove(RecievedMessage.UserName);                   // his name will be removed from the ClientList
                                RecievedMessage.ClientList.Clear();
                                for (int n = 0; n < OnlineClientNames.Count; n++)    // each client's "online clients" list will be updated
                                {
                                    RecievedMessage.ClientList.Add(OnlineClientNames[n]);
                                }
                                ClientsListView.Invoke(new Action(() =>
                                {
                                    connectedClients.RemoveAt(i);               // his client's connection will be removed from the list
                                    History_RichTextBox.SelectedText += DateTime.Now + " " + RecievedMessage.UserName + " has Quit" + "\n";
                                    try         // if the name doesn't exist in the listview, then the server will skip the next line
                                    {
                                        this.ClientsListView.Items.RemoveAt(i);          // his name will be removed from the server's listview
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    ClientNetStreams.RemoveAt(i);              // his networkstream will be removed from the list
                                }));
                            }
                            if (ClientNetStreams.Count > 0)
                            {
                                if (RecievedMessage.Msg.Contains("[To: ")) // checks if the recieved message is a private message
                                {
                                    for (int c = 0; c < OnlineClientNames.Count; c++)    // starts to look for the client meant to recieve this private message
                                    {
                                        if (RecievedMessage.Msg.Contains("[To: " + OnlineClientNames[c] + "] ")) // if this client was meant to recieve this private message
                                        {
                                            formatter.Serialize(ClientNetStreams[i], RecievedMessage);             // sends the message to the sender only
                                            formatter.Serialize(ClientNetStreams[c], RecievedMessage);             // sends the message only to the specified client
                                            com = new SqlCommand("insert into MessageTable (Message,UserID,SentDate,RecipientID) values('"   // writes the recieved message to the MessageTable in the database 
                                            + RecievedMessage.Msg + "','" + RecievedMessage.UserID + "',GETDATE()," + UserIDList[c] + ")", Connection);
              //the written data in the database includes: the message, the sender's ID, the date the message was sent, and the recipient's ID if it is a private message (public messages' RecipientID is NULL)
                                            Connection.Open();
                                            com.ExecuteNonQuery();
                                            Connection.Close();
                                        }
                                    }
                                }
                                else             // if the message is a public\not a private message
                                {
                                    for (int j = 0; j < ClientNetStreams.Count; j++)           // this loop sends the message to all clients
                                    {
                                        formatter.Serialize(ClientNetStreams[j], RecievedMessage);
                                    }
                                    com = new SqlCommand("insert into MessageTable (Message,UserID,SentDate) values('"  // writes the recieved message to the MessageTable in the database  
                                    + RecievedMessage.Msg + "','" + RecievedMessage.UserID + "',GETDATE())", Connection); 
                                    //the written data in the database includes: the message, the sender's ID, the date the message was sent
                                    Connection.Open();
                                    com.ExecuteNonQuery();
                                    Connection.Close();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        void AcceptClients()           // this thread accepts clients' connection requests
        {
            while (true)
            {
                try
                {
                    client = listener.AcceptTcpClient();
                    Stream = client.GetStream();        // recieves the client's Networkstream 
                            ClientAccepted = new Thread(WriteClientConnected);
                            ClientAccepted.Start();
                }
                catch (Exception)
                {
                }
            }
        }
        void UpdateGrid()          // This thread updates the server's DataGridView repeatedly
        {
            while (true)
            {
                DataGridView.Invoke(new Action(() =>
                 {
                 Adapter = new SqlDataAdapter("SELECT * FROM ChatTable", DataUpdateConnection);    // read all the data from the database that contains the registered users' data
                 data = new DataTable();                // place the recieved data on a virtual datatable and remove the previously recieved data
                 try
                 {
                     Adapter.Fill(data);             // fill the datatable with data from the database
                 }
                 catch (Exception)
                 {
                 }
                 DataGridView.DataSource = data;      // display the recieved data on the datagridview
                }));
                UpdateDataGrid.Join(500);      // the thread will wait to reduce memory usage
            }

        }
        void WriteClientConnected()
        {
            try
            {
                ChatMessage RecievedClient = (ChatMessage)formatter.Deserialize(Stream);      // reads the connected client's data
                    UserIDList.Add(RecievedClient.UserID);
                    ClientNames.Add(RecievedClient.UserName);                 // adds the client's username to the ClientNames list, this is done to ensure that 2 clients won't use the same username
                    OnlineClientNames.Add(RecievedClient.UserName);
                    connectedClients.Add(client);                  // adds the client's connection to the connectedClients list
                    ClientNetStreams.Add(client.GetStream());         // adds the client's networkstream to the ClientNetStreams list
                    History_RichTextBox.Invoke(new Action(() =>
                    {
                            History_RichTextBox.SelectedText += DateTime.Now + " " + RecievedClient.UserName + " Connected" + "\n";      // writes in the server's history that the client has connected
                            for (int i = 0; i < connectedClients.Count; i++)
                            {
                                NetworkStream net = connectedClients[i].GetStream();
                                RecievedClient.ClientList.Clear();
                                for (int j = 0; j < OnlineClientNames.Count; j++)               // adds the client's name to each online client's ClientList so they can send him private messages
                                {
                                    RecievedClient.ClientList.Add(OnlineClientNames[j]);
                                }
                                formatter.Serialize(net, RecievedClient);
                            }
                            ListViewItem items = new ListViewItem(new[] { RecievedClient.UserName, "Online", DateTime.Now.ToString() }); //adds the client's name to the listview
                            this.ClientsListView.Items.Add(items);
                    }));
                    if (FirstClientConnected == false)          // if no other clients connected before this client
                    {
                        MessageReader.Start();                   // the messagereader thread starts, it reads the data and messages sent by all clients
                        FirstClientConnected = true;
                    }
                    ClientAccepted.Abort();                     // after this client has connected to the server, his connection thread will abort since the connection is complete
            }
            catch (Exception)
            {
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            search = new SearchWindow();        // open the search window which alows searching data from the database and deleting users
            search.ShowDialog();                // display the search window
        }
    }
}
