using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LibMessage;
using System.Data.SqlClient;

namespace client
{
    public partial class LoginScreen : Form
    {

        // NOTE: remember to close any connected clients before stopping the debugger!

        SqlConnection Connection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");
        bool exception_occured;
        SqlCommand Command;
        public LoginScreen()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        public string userName { get; set; }     // the username the user has entered
        public string ipAddress { get; set; }      // the ip address the user has entered
        public bool ConnectionCancelled { get; set; }   // determines if the user has cancelled the connection dialog
        public int port { get; set; }    // the port the user has entered
        public Color textcolor { get; set; }    // the text color the user has chosen (default is Black)
        ColorDialog ChooseColor = new ColorDialog();
        public int UserID { get; set; }     // the user's ID which is listed at the database, will only function once the user is connected and is not displayed on the UI
        private void button1_Click(object sender, EventArgs e)
        {
            ConnectionCancelled = false;
            exception_occured = false;
            Command = new SqlCommand("select ID,Name,UserName,IsConnected from ChatTable", Connection);
            Connection.Open();

            SqlDataReader DataReader = Command.ExecuteReader();     // read the data from the database to check if the entered username exists

            while (DataReader.Read())
            {
                if (UserNameBox.Text == (string)DataReader["UserName"] && NameBox.Text == (string)DataReader["Name"])   // if the entered name and username exist in the database
                {
                    bool IsOnline = DataReader.GetBoolean(DataReader.GetOrdinal("IsConnected"));   // read the "IsConnected" field as boolean to check if the user is offline
                    if (IsOnline == true)   // if the user is online the reader stops and the conne ction attempt will be terminated
                    {
                        MessageBox.Show("This user is already connected");
                        Connection.Close();
                        break;
                    }
                    //**************** checking the name the user entered:
                    UserID = DataReader.GetInt32(DataReader.GetOrdinal("ID"));
                    try
                    {
                        userName = UserNameBox.Text;
                        if (UserNameBox.Text == string.Empty)
                        {
                            MessageBox.Show("Enter a name first");
                            exception_occured = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid name entered");
                        exception_occured = true;
                    }

                    //*************** checking the IP address:
                    try
                    {
                        ipAddress = IPBox.Text;
                        if (IPBox.Text == string.Empty && exception_occured == false)
                        {
                            MessageBox.Show("Enter IP");
                            exception_occured = true;
                        }
                    }
                    catch (Exception)
                    {
                        if (exception_occured == false)
                        {
                            MessageBox.Show("Error: Invalid IP entered");
                            exception_occured = true;
                        }
                    }

                    //**************** checking the port:
                    try
                    {
                        port = int.Parse(PortBox.Text);
                        if (PortBox.Text == string.Empty && exception_occured == false)
                        {
                            MessageBox.Show("Enter Port");
                            exception_occured = true;
                        }
                    }
                    catch (Exception)
                    {
                        if (exception_occured == false)
                        {
                            MessageBox.Show("Error: Invalid Port entered");
                            exception_occured = true;
                        }
                    }

                    //**************** if name, IP adress and port are all valid:
                    if (exception_occured == false)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConnectionCancelled = true;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ChooseColor.ShowDialog() == DialogResult.OK)
            {
                textcolor = ChooseColor.Color;
            }
        }
    }
}
