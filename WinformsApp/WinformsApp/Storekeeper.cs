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

        public Storekeeper()
        {
            InitializeComponent();
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
            WorkBook workbook = WorkBook.Load(FilePath);
            WorkSheet sheet = workbook.WorkSheets.First();

            for (int i = 0; i < 5; i++)
            {
                if (sheet[columns[i] + "1"].StringValue != columnNames[i])
                {
                    textBox3.Text = "Неверный формат файла!";
                    return;
                }
            }

            //run throw rows
            for (int i = 1; i < 1000000; i++)
            {
                if (sheet['A' + i.ToString()].StringValue == "")
                {
                    break;
                }

                //run throw columns
                for (int j = 0; j < 5; j++)
                {
                    rowData[j] = sheet[columns[j] + i.ToString()].StringValue;
                }

                textBox3.Text = rowData[3];
            }
        }
    }
}
