﻿/*
 * Zachary T. Vig
 * Jamie Gleason
 * Keegon Cabinaw
 * GROUP PHIV
 * CIT195 Group Project
 * Code-stop date: 04/22/2015
 * 
 * Program .cs file
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thief_Escape
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
