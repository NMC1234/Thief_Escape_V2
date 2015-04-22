/*
 * Zachary T. Vig
 * Jamie Gleason
 * Keegon Cabinaw
 * GROUP PHIV
 * CIT195 Group Project
 * Code-stop date: 04/22/2015
 * 
 * frmLoad main .cs file
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
    public partial class frmLoad : Form
    {
        public string UserName;

        //  Default Constructor
        public frmLoad()
        {
            InitializeComponent();
        }

        //  Load Event
        private void frmLoad_Load(object sender, EventArgs e)
        {
            //  See if the SaveGames Directory Exists
            if (Directory.Exists("SaveGames"))
            {
                //  Find any Save folders in the SaveGames directory
                string[] files = Directory.GetDirectories("SaveGames");

                //  Strip the directory from the strings
                string[] names = new string[files.Count()];

                for (int i = 0; i < files.Count(); i++)
                {
                    names[i] = files[i].Replace("SaveGames\\", "");
                }

                //  Populate listbox with save names
                foreach(string s in names)
                {
                    lstLoadGame.Items.Add(s);
                }
            }
            else   //   Inform the user via message box, return to main menu
            {
                MessageBox.Show("Unable to find SaveGames folder, returning to Main Menu", "Error");
                this.Close();
            }
        }

        //  Select Button
        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lstLoadGame.SelectedIndex == -1)    //  Nothing is selected
            {
                MessageBox.Show("Please select a game", "No Selection");
                return;
            }

            //  Update username and set dialogresult to OK
            UserName = lstLoadGame.Items[lstLoadGame.SelectedIndex].ToString();
            this.DialogResult = DialogResult.OK;
        }

        //  ToolStrip Selected
        private void loadSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstLoadGame.SelectedIndex == -1)    //  Nothing is selected
            {
                MessageBox.Show("Please select a game", "No Selection");
                return;
            }

            //  Update username and set dialogresult to OK
            UserName = lstLoadGame.Items[lstLoadGame.SelectedIndex].ToString();
            this.DialogResult = DialogResult.OK;
        }

        //  ToolStrip NewGame
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create the GetName form, and launch it
            FrmGetName frm = new FrmGetName();
            frm.Show();
            
            //  Hide this form
            this.Hide();
        }

        //  ToolStrip Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}

