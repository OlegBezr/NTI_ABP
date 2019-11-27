using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinformsApp
{
    public partial class LoginPage : Form
    {
        SqlConnection connection;
        string connectionString;

        public LoginPage()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["WinformsApp.Properties.Settings.CompanyInfoConnectionString"].ConnectionString;
        }

        private void LoginPage_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Check_User(textBox1.Text, textBox2.Text);
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "USER")
            {
                textBox1.Text = "";
            }
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox2.Text == "###")
            {
                textBox2.Text = "";
            }
        }

        private bool Check_User(string username, string password)
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand();
                com.Connection = connection;
                com.CommandText = "SELECT * FROM Users";
                SqlDataReader sqlDataReader = com.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    MessageBox.Show("Your data: " + username + password, "Data");
                    MessageBox.Show(sqlDataReader["Username"].ToString(), "Info1");
                    MessageBox.Show(sqlDataReader["Password"].ToString(), "Info2");
                    MessageBox.Show((username == sqlDataReader["Username"].ToString()).ToString());
                    MessageBox.Show((username == sqlDataReader["Password"].ToString()).ToString());

                    if (username.Equals(sqlDataReader["Username"].ToString()) && password.Equals(sqlDataReader["Password"].ToString()))
                    { 
                        MessageBox.Show("Login Successful!", "Congrates");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Login failed!", "Error");
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
