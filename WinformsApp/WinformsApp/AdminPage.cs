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
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void roleBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.roleBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.companyInfoDataSet);

        }

        private void AdminPage_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "companyInfoDataSet.Role". При необходимости она может быть перемещена или удалена.
            this.roleTableAdapter.Fill(this.companyInfoDataSet.Role);

        }
    }
}
