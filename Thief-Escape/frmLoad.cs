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
            UserName = userName;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lstLoadGame.SelectedIndex == 0)
            {
                UserName = "Jamie";
                this.DialogResult = DialogResult.OK;
            }
            if (lstLoadGame.SelectedIndex == 1)
            {
                UserName = "Zachary";
                this.DialogResult = DialogResult.OK;
            }
            if (lstLoadGame.SelectedIndex == 2)
            {
                UserName = "Keegon";
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if (lstLoadGame.SelectedIndex < 0 || lstLoadGame.SelectedIndex > 2)
                    MessageBox.Show("Please select a game to load.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        private void loadSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstLoadGame.SelectedIndex == 0)
            {
                UserName = "Jamie";
                this.DialogResult = DialogResult.OK;
            }
            if (lstLoadGame.SelectedIndex == 1)
            {
                UserName = "Zachary";
                this.DialogResult = DialogResult.OK;
            }
            if (lstLoadGame.SelectedIndex == 2)
            {
                UserName = "Keegon";
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if (lstLoadGame.SelectedIndex < 0 || lstLoadGame.SelectedIndex > 2)
                    MessageBox.Show("Please select a game to load.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }        
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create the GetName form, and launch it
            FrmGetName frm = new FrmGetName();
            frm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

