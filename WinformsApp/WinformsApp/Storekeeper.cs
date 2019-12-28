using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using IronXL;
using System.Threading;

namespace WinformsApp
{
    public partial class Storekeeper : Form
    {
        private string[] columnNames = new string[5]{ "NAME", "TYPE", "WEIGHT", "ARTICLE", "YEAROFISSUE" };
        private char[] columns = new char[5] { 'A', 'B', 'C', 'D', 'E' };
        private string[] rowData = new string[5] { "", "", "", "", "" };

        SqlConnection connection;
        string connectionString;

        WorkBook mistakes = null;

        public Storekeeper()
        {
            InitializeComponent();

            IronXL.License.LicenseKey = "IRONSTUDIO-225346179-443843-3F2132B-D81C16BC0-96ADD8-UEx9354304405A9F09-2037182459";

            connectionString = ConfigurationManager.ConnectionStrings["WinformsApp.Properties.Settings.NTI_ABPConnectionString"].ConnectionString;
        }

        OpenFileDialog dialog = new OpenFileDialog();
        private void button1_Click(object sender, EventArgs e)
        {
            dialog.Filter = "XLSX, XLS, CSV|*.xlsx;*.xls;*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
                textBox2.Text = dialog.SafeFileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //call function with the path to the file
            readXLS(textBox1.Text);
        }

        public void readXLS(string FilePath)
        {
            try
            {
                WorkBook workbook = WorkBook.Load(FilePath);
                WorkSheet sheet = workbook.WorkSheets.First();

                for (int i = 0; i < 5; i++)
                {
                    if (sheet[columns[i] + "1"].StringValue != columnNames[i])
                    {
                        label1.Text = "Неверный формат файла!";
                        return;
                    }
                }

                //prepare excel for bad data
                WorkBook mistakesWorkbook = WorkBook.Create(ExcelFileFormat.XLS);
                mistakesWorkbook.Metadata.Author = "IronXL";
                WorkSheet xlsSheet = mistakesWorkbook.CreateWorkSheet("Sheet1");
                for (int j = 0; j < 5; j++)
                {
                    xlsSheet[columns[j] + "1"].Value = columnNames[j];
                }
                int rowCounter = 2;

                //run throw rows
                for (int i = 2; i < 1000000; i++)
                {
                    if (sheet['A' + i.ToString()].StringValue == "")
                    {
                        break;
                    }

                    bool checkFormat = true;
                    //run throw columns
                    string name = sheet[columns[0] + i.ToString()].StringValue;
                    string type = sheet[columns[1] + i.ToString()].StringValue;
                    string weight = sheet[columns[2] + i.ToString()].StringValue;
                    string article = sheet[columns[3] + i.ToString()].StringValue;
                    string yearOfIssue = sheet[columns[4] + i.ToString()].StringValue;

                    //check data format
                    if (name.Length > 100)
                        checkFormat = false;
                    try {
                        Convert.ToDouble(weight);
                    }
                    catch {
                        checkFormat = false;
                    }
                    if (article.Length > 16)
                        checkFormat = false;
                    for (int j = 0; j < article.Length; j++) {
                        if (article[j] == ' ')
                        {
                            checkFormat = false;
                            break;
                        }
                    }
                    try {
                        Convert.ToDateTime(yearOfIssue);
                    }
                    catch {
                        checkFormat = false;
                    }

                    if (!checkFormat)
                    {
                        xlsSheet["A" + rowCounter.ToString()].Value = name;
                        xlsSheet["B" + rowCounter.ToString()].Value = type;
                        xlsSheet["C" + rowCounter.ToString()].Value = weight;
                        xlsSheet["D" + rowCounter.ToString()].Value = article;
                        xlsSheet["E" + rowCounter.ToString()].Value = yearOfIssue;

                        rowCounter++;
                    }
                    else
                    {
                        AddInfoToComponentsTable(name, type, Convert.ToDouble(weight), article, Convert.ToDateTime(yearOfIssue));
                    }
                }

                label1.Text = "Кол-во строк в плохом формате: " + (rowCounter - 2).ToString();
                mistakes = mistakesWorkbook;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                label1.Text = "Вероятно, выбранный вами файл используется другой программой!";
            }
        }

        private void AddInfoToComponentsTable(string name, string type, double weight, string article, DateTime yearOfIssue)
        {
            using (connection = new SqlConnection(connectionString))
            {
                SqlCommand com;
                SqlDataReader reader;

                connection.Open();

                //Get rid of useless copies
                using (com = new SqlCommand())
                {
                    com.Connection = connection;
                    com.CommandText = String.Format("SELECT Article FROM Components");

                    using (reader = com.ExecuteReader())
                        while (reader.Read())
                        {
                            if (article.Equals(reader["Article"].ToString()))
                            {
                                connection.Close();
                                return;
                            }
                        }
                }
                connection.Close();

                try
                {
                    using (com = new SqlCommand())
                    {
                        com.Connection = connection;
                        com.CommandType = CommandType.Text;
                        com.CommandText = "INSERT INTO Components (Name, Type, Weight, Article, YearOfIssue) VALUES (@Name, @Type, @Weight, @Article, @YearOfIssue)";
                        com.Parameters.AddWithValue(@"Name", name);
                        com.Parameters.AddWithValue(@"Type", type);
                        com.Parameters.AddWithValue(@"Weight", weight);
                        com.Parameters.AddWithValue(@"Article", article);
                        com.Parameters.AddWithValue(@"YearOfIssue", yearOfIssue);
                        connection.Open();
                        com.ExecuteNonQuery();
                        connection.Close();
                    }

                    this.componentsTableAdapter.Fill(this.nTI_ABPDataSet.Components);
                    this.tableAdapterManager.UpdateAll(this.nTI_ABPDataSet);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void componentsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.componentsBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.nTI_ABPDataSet);
        }

        private void Storekeeper_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "nTI_ABPDataSet.Components". При необходимости она может быть перемещена или удалена.
            this.componentsTableAdapter.Fill(this.nTI_ABPDataSet.Components);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (mistakes != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "XLS|*.xls|XLSX|*.xlsx|CSV|*.csv";
                saveFileDialog1.FileName = "IncorrectData";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mistakes.SaveAs(saveFileDialog1.FileName);
                }
            }
            else
            {
                label1.Text = "Чтобы сохранить файл с ошибками, сначала загрузите файл с данными!";
            }
        }
    }
}
