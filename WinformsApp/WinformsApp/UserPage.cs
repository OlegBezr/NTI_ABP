using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace WinformsApp
{
    public partial class UserPage : Form
    {
        SqlConnection connection;
        string connectionString;
        int id = 1;
        string name = "";
        int roleId = 1;
        string role = "";

        public UserPage()
        {
            InitializeComponent();
        }

        public UserPage(int id)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["WinformsApp.Properties.Settings.CompanyInfoConnectionString"].ConnectionString;

            this.id = id;
            Get_User_Data(id);
            Display_Greetings();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Get_User_Data(int id)
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com;
                SqlDataReader reader;
                using (com = new SqlCommand())
                {
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT Username, RoleId FROM Users Where Id={0}", id);
                    using (reader = com.ExecuteReader())
                        if (reader.Read())
                        {
                            name = reader["Username"].ToString();
                            roleId = Convert.ToInt32(reader["RoleId"]);
                        }
                }

                using (com = new SqlCommand())
                {
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT name FROM Role Where Id={0}", roleId);
                    using (reader = com.ExecuteReader())
                        if (reader.Read())
                        {
                            role = reader["name"].ToString();
                        }
                }
            }
        }

        private void Display_Greetings()
        {
            label3.Text = String.Format("Добро пожаловать, {0}!", name);
            label1.Text = String.Format("Ваша роль: {0}. Здесь вы найдёте всю важную информацию", role);
        }
    }
}
