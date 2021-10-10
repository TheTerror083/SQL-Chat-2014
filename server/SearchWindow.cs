using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class SearchWindow : Form
    {
        SqlConnection Connection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");
        SqlDataAdapter SearchAdapter = new SqlDataAdapter();
        SqlCommandBuilder DeleteUserCommand;
        SqlCommand Command;
        DataTable MessageTable,UserTable;
        public SearchWindow()
        {
            InitializeComponent();
            UserGridView.ReadOnly = true;
            MessageGridView.ReadOnly = true;
        }

        private void SearchMessage_Click(object sender, EventArgs e)
        {
            if (SearchDateBox.Text!=string.Empty)          // if the user has entered a data to search for in the "search by date" box
            {
                try
                {
                    DateTime maxday = DateTime.Parse(SearchDateBox.Text).AddDays(1);   // add one day to the date the user has entered, this is done to mark an end to the range of hours which the searcher should look for
                    var EndDate = maxday.Date.ToString("yyyy-MM-dd HH:mm:ss");     // this is the format of the date that should be searched, any other format will result in an Error
                    SearchAdapter = new SqlDataAdapter("select * from MessageTable where SentDate between '"
                    + SearchDateBox.Text + "' and '" + EndDate.ToString() + "'", Connection); // search messages sent between the entered date and the day after
                    MessageTable = new DataTable();    // clear the message search datatable from the previous results  
                    SearchAdapter.Fill(MessageTable); // fill the message search datatable with the found results
                    MessageGridView.DataSource = MessageTable;  // display the results on the message search's datagridview
                    SearchDateBox.Clear();      // clear the "search by date" textbox 
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: invalid date entered (enter the date as Year-Month-Day)");
                    SearchDateBox.Clear();
                }
            }
            if (SearchWordBox.Text != string.Empty)     // if the user has entered a data to search for in the "search by word" box
            {
                SearchAdapter = new SqlDataAdapter("select * from MessageTable where Message like '%" + SearchWordBox.Text + "%'", Connection);  // search for messages similar to the entered words from the textbox
                MessageTable = new DataTable();    // clear the message search datatable from the previous results    
                SearchAdapter.Fill(MessageTable);     // fill the message search datatable with the found results
                MessageGridView.DataSource = MessageTable;   // display the results on the message search's datagridview
                SearchWordBox.Clear();       // clear the "search by word" textbox 
            }
        }

        private void SearchName_Click(object sender, EventArgs e)
        {
            if (SearchIDBox.Text != string.Empty)
            {
                int ID_input = int.Parse(SearchIDBox.Text);    // read the the input from the "search ID box" as int
                SearchAdapter = new SqlDataAdapter("select * from ChatTable where ID = " + ID_input, Connection);  // search for the user which has the entered ID
                UserTable = new DataTable();        // clear the user search datatable from the previous results  
                SearchAdapter.Fill(UserTable);       // fill the user search datatable with the found results
                UserGridView.DataSource = UserTable;   // display the results on the user search's datagridview
                SearchIDBox.Clear();                  // clear the "search by ID" textbox 
            }
            if (SearchUserNameBox.Text != string.Empty)
            {
                SearchAdapter = new SqlDataAdapter("select * from ChatTable where UserName = '" + SearchUserNameBox.Text + "'", Connection); // search for te user with the same username the user has entered
                UserTable = new DataTable();       // clear the user search datatable from the previous results 
                SearchAdapter.Fill(UserTable);    // fill the user search datatable with the found results
                UserGridView.DataSource = UserTable;    // display the results on the user search's datagridview
                SearchUserNameBox.Clear();       // clear the "search by ID" textbox 
            }
        }

        private void DeleteUserBtn_Click(object sender, EventArgs e)
        {
            DeleteUserCommand = new SqlCommandBuilder(SearchAdapter); 
            Command = new SqlCommand("select ID,Name,UserName,IsConnected from ChatTable", Connection);
            Connection.Open();

            SqlDataReader reader = Command.ExecuteReader();     // start reading data from the database

            while (reader.Read())
            {
                try
                {
                    if ((int)reader["ID"] == int.Parse(SearchIDBox.Text))       // if the entered ID matches the recently recieved ID
                    {
                        bool IsOnline = reader.GetBoolean(reader.GetOrdinal("IsConnected"));    // read the "IsConnected" field from the database as a boolean
                        if (IsOnline == false)           // if the "IsConnected" field is marked as false
                        {
                            UserTable.Rows[0].Delete();   // the user will be removed
                            reader.Close();             // stop reading from the database
                            SearchAdapter.Update(UserTable);  // update the databse and remove the selected user from the database
                            break;
                        }
                        else     // if the "IsConnected" field is marked as true
                        {
                            MessageBox.Show("A User cannot be removed while he's Online");   // the user won't be removed as online useres cannot be deleted unless they disconnect
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            Connection.Close();
        }
    }
}
