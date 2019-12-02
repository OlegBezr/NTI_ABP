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
        int mistakes = 0;
        int minutesBan = 3;

        public LoginPage()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["WinformsApp.Properties.Settings.NTI_ABPConnectionString"].ConnectionString;
        }

        private void LoginPage_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int timeBan = CheckTimeBlock();
            if (timeBan > 0)
            {
                NotificationLabel.Text = String.Format("Login is banned. Wait for {0} minutes", timeBan);
            }
            else
            {
                CheckUser(textBox1.Text, textBox2.Text);
            }
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

        private int CheckTimeBlock()
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com;
                SqlDataReader reader;
                using (com = new SqlCommand())
                {
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT * FROM Ban WHERE Id = 1");
                    using (reader = com.ExecuteReader())
                        if (reader.Read())
                        {
                            if (reader["StartTime"] is DBNull)
                            {
                                return 0;
                            }

                            int timeBan = (int)(DateTime.Now.TimeOfDay - Convert.ToDateTime(reader["StartTime"]).TimeOfDay).TotalMinutes;
                            if (timeBan < minutesBan)
                            {
                                return (int)(minutesBan - timeBan);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                }
            }
            MessageBox.Show("4", "Error");
            return 0;
        }

        private bool CheckUser(string username, string password)
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
                    CreateLoginLog(id);
                    this.Hide();
                    UserPage userPage = new UserPage(id);
                    userPage.ShowDialog();
                    this.Close();
                    return true;
                }
                else
                {
                    mistakes++;
                    NotificationLabel.Text = String.Format("Wrong username or password.\nNumber of tries: {0}", mistakes);

                    if (mistakes >= 3)
                    {
                        using (com = new SqlCommand())
                        {
                            com.Connection = connection;
                            com.CommandText = String.Format("UPDATE Ban SET StartTime = '{0}', Name = 'Smb' WHERE Id = 1", DateTime.Now);
                            com.ExecuteNonQuery();
                        }
                        NotificationLabel.Text = String.Format("Login is banned. Wait for {0} minutes", minutesBan);
                        mistakes = 0;
                    }

                    return false;
                }
            }

            return false;
        }

        private void CreateLoginLog(int id)
        {
            using (connection = new SqlConnection(connectionString))
            {
                SqlCommand com;
                try
                {
                    using (com = new SqlCommand())
                    {
                        com.Connection = connection;
                        com.CommandType = CommandType.Text;
                        com.CommandText = "INSERT INTO Logs (ComputerName, LoginTime, UserId) VALUES (@ComputerName, @LoginTime, @UserId)";
                        com.Parameters.AddWithValue(@"ComputerName", SystemInformation.ComputerName);
                        com.Parameters.AddWithValue(@"LoginTime", DateTime.Now);
                        com.Parameters.AddWithValue(@"UserId", id);
                        connection.Open();
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
