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
                MessageBox.Show(username + ' ' + password, "EnteredInfo");

                connection.Open();
                SqlCommand com;
                int count;
                using (com = new SqlCommand())
                { 
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT count(*) FROM Users WHERE Username='{0}' and Password='{1}'", username, password);
                    count = Convert.ToInt32(com.ExecuteScalar());
                }

                if (count > 0)
                { 
                    MessageBox.Show("Login Successful!", "Congrates");
                    Close();
                    Thread th = new Thread(Change_Form);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
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

        private void Change_Form(object obj)
        {
            Application.Run(new UserPage());
        }
    }
}
