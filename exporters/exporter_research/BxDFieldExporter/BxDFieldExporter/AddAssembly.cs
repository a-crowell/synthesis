﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BxDFieldExporter;
using Inventor;

namespace InvAddIn
{
    public partial class AddAssembly : Form
    {
        //Used to access StandardAddInServer's exposed API
        private Inventor.Application mApplication;
        private AutomationInterface mAddInInterface;

        public AddAssembly()
        {
            this.Location = new System.Drawing.Point(450, 350);
            InitializeComponent();
            this.TopMost = true;

            //Used to access StandardAddInServer's exposed API
            try
            {
                mApplication = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }

            catch
            {
                Type inventorAppType = System.Type.GetTypeFromProgID("Inventor.Application");
                mApplication = System.Activator.CreateInstance(inventorAppType) as Inventor.Application;
            }
            mApplication.Visible = true;


            //Iterates through Inventor Add-Ins collection  
            foreach (ApplicationAddIn oAddIn in mApplication.ApplicationAddIns)
            {
                //Looks for StandardAddInServer's Class ID;
                if (oAddIn.ClassIdString == "{E50BE244-9F7B-4B94-8F87-8224FABA8CA1}")
                {
                    //Calls Automation property    
                    mAddInInterface = (AutomationInterface)oAddIn.Automation;
                }

            }
        }

        private void OKButton_OnClick(object sender, EventArgs e)
        {
            mAddInInterface.setRunOnce(false);
            this.Close();
        }

        private void CancelButton_onClick(object sender, EventArgs e)
        {
            mAddInInterface.setCancel(true);
            mAddInInterface.setRunOnce(false);
            this.Close();
        }

        private void AddAssembly_Load(object sender, EventArgs e)
        {

        }

        private void CancelButton_onClick(object sender, FormClosedEventArgs e)
        {
            mAddInInterface.setCancel(true);
            mAddInInterface.setRunOnce(false);
        }
    }
}