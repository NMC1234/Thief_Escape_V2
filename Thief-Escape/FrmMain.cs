/*
 * Zachary T. Vig
 * Jamie Gleason
 * Keegon Cabinaw
 * GROUP PHIV
 * CIT195 Group Project
 * Code-stop date: 04/22/2015
 * 
 * frmMain main .cs file
 */
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

namespace Thief_Escape
{
    public partial class FrmMain : Form
    {

        public FrmMain()
        {
            InitializeComponent();
        }

        string PlayerName = "User";

        #region [ Button Click Events ]

        //New Game Button
        private void btnNew_Click(object sender, EventArgs e)
        {
            //Create the GetName form, and launch it
            
                FrmGetName frm = new FrmGetName();
                frm.Show();

            //Hide this form
            this.Hide();
        }

        //  Load Game Button
        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            //to-do
            var LoadGame = new frmLoad();
            if (LoadGame.ShowDialog() == DialogResult.OK) 
            {
                    Form LoadSavedGame = new FrmGame(LoadGame.UserName);
                    LoadSavedGame.Show();
                    this.Hide();
            }

        }
     

        //  Exit Game Button
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        //  When this form is closed ( not hidden ) ensure that everything is closed.
        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
                Application.Exit();
        }

        //  When the program is loaded make sure the DefaultMaps exist.
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //  Ensure the DefaultMaps directory exists, if not inform the user and close the program
            if (!Directory.Exists("DefaultMaps"))
            {
                MessageBox.Show(@"Unable to find the DefaultMaps directory. This folder should be in the same folder as Theif-Escape.exe. Without this folder the game cannot load.", "Critical Failure");
                this.Close();
            }
        }
    }
}
