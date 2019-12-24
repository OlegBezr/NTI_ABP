using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsApp
{
    public partial class Storekeeper : Form
    {
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
    }
}
