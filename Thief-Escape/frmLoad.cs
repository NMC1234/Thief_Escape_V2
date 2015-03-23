using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thief_Escape
{
    public partial class frmLoad : Form
    {
        public string UserName;
        public frmLoad()
        {
            InitializeComponent();
        }
        public frmLoad(string userName)
        {
            InitializeComponent();
            userName = UserName;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lstLoadGame.SelectedIndex == 0)
            {
                UserName = "Jamie";
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            if (lstLoadGame.SelectedIndex == 1)
            {
                UserName = "Zachary";
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            if (lstLoadGame.SelectedIndex == 2)
            {
                UserName = "Keegon";
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            else
                MessageBox.Show("Please select a game to load.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

