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

namespace client
{
    public partial class RegisterScreen : Form
    {
        SqlConnection Connection = new SqlConnection("Data Source=HAHAMOV-PC\\SQLEXPRESS;Initial Catalog=ChatTestDB;Integrated Security=True");
        SqlCommand Command;
        SqlTransaction AddUserTransaction;
        bool Registration_Successful;
        public RegisterScreen()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            Registration_Successful = true;    // this boolean will be set as true and will only change if an exception occurs during registraion
            string Name = RGSNameBox.Text;
            string UserName = RGSUserNameBox.Text;
            Command = new SqlCommand("insert into ChatTable (Name,UserName,LastConnectionDate,IsConnected) values('"   // add the entered values from the textboxes to the database
                // the written values are: the entered name (not to be confused with username!), the entered username,
                //the date of the last time this user has connected (upon registration this column is filled with the current date even if the user did not connect yet), "IsConnected" field is marked as false
                + Name + "','" + UserName + "',GETDATE(),0)", Connection);
            Connection.Open();
            AddUserTransaction = Connection.BeginTransaction();
            Command.Transaction = AddUserTransaction;
            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("The UserName you entered already exists, enter another name"); // if the entered username already exists the registration attempt will fail
                Registration_Successful = false;
            }
            AddUserTransaction.Commit(); // add the new user to the database with a transaction
            Connection.Close();
            if (Registration_Successful == true)    // if all entered values are valid and the registration is complete
            {
                MessageBox.Show("Registration Succussful");
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
