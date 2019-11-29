using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Threading;

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
                SqlCommand com;
                SqlDataReader reader;
                bool checkUser = false;
                int id = 1;
                using (com = new SqlCommand())
                { 
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT * FROM Users");
                    using (reader = com.ExecuteReader())
                        while (reader.Read())
                        {
                            if (username.Equals(reader["Username"].ToString()) && password.Equals(reader["Password"].ToString()))
                            {
                                checkUser = true;
                                id = Convert.ToInt32(reader["Id"]);
                                break;
                            }
                        }
                }

                if (checkUser)
                { 
                    MessageBox.Show("Login Successful!", "Congrates");
                    Create_Login_Log(id);
                    this.Hide();
                    UserPage userPage = new UserPage(id);
                    userPage.ShowDialog();
                    this.Close();
                    return true;
                }
                else
                {
                    MessageBox.Show("Login failed!", "Error");
                    return false;
                }
            }

            return false;
        }

        private void Create_Login_Log(int id)
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com;
                try
                {
                    using (com = new SqlCommand())
                    {
                        com.Connection = connection;
                        com.CommandText = String.Format("insert into Logs(ComputerName, LoginTime, UserId) values(@ComputerName, @LoginTime, @UserId)");
                        com.Parameters.AddWithValue(@"ComputerName", SystemInformation.ComputerName);
                        com.Parameters.AddWithValue(@"LoginTime", DateTime.Now);
                        com.Parameters.AddWithValue(@"UserId", id);
                        com.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
