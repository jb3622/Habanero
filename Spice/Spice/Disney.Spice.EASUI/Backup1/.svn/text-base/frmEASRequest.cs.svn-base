using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace Disney.Spice.EASUI
{
    public partial class frmEASRequest : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users               _username;
        private Disney.Menu.Environments        _paramenv;
        private Form                            _mdiparent;
        private Int64                           _requestid;
        private DataTable                       dtApprovers;
        private Disney.DA.EAS.DSSPARE           _dsspare;
        
        public frmEASRequest()
        {
            InitializeComponent();
        }

        public frmEASRequest(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1, Int64 requestid)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;
            _mdiparent = Form1;

            _requestid = requestid;

            this.Text = this.Text + " " + _requestid.ToString();

            SetupDatTable();

            SetupDataGrid();

            // hourglass cursor
            Cursor.Current = Cursors.Default;
        }

        private void SetupDatTable()
        {
            DataTable dtData = new DataTable ();

            Disney.DA.EAS.DSSPAAR dsspaar = new Disney.DA.EAS.DSSPAAR(_dbparamref, _username);
            dtData = dsspaar.GetApprovers(_requestid);

            _dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, _username);
            _dsspare.GetApprovalDetails(Convert.ToInt32(_requestid));

            dtApprovers = dtData.Copy();
        }


        private void SetupDataGrid()
        {
            List<String> columnsvisible = new List<string>();
            columnsvisible.Add("Approver");
            columnsvisible.Add("ApprovalLevel");
            columnsvisible.Add("ApprovalSubLevel");
            columnsvisible.Add("Status");
            columnsvisible.Add("RejectionReason");
            columnsvisible.Add("ActiveDescription");

            // HK : 14-01-2010 : Add the Active Column
            string expression;
            expression = "iif (Active = 'Y', 'Active', 'In Active')";
            dtApprovers.Columns.Add("ActiveDescription", typeof(string));
            dtApprovers.Columns["ActiveDescription"].Expression = expression;

            // Set the datasource
            dgvApprovers.DataSource = dtApprovers;

            // Header Text
            dgvApprovers.Columns["Approver"].HeaderText = "Approver";
            dgvApprovers.Columns["ApprovalLevel"].HeaderText = "Level";
            dgvApprovers.Columns["ApprovalSubLevel"].HeaderText = "Sub-Level";
            dgvApprovers.Columns["Status"].HeaderText = "Status";
            dgvApprovers.Columns["ActiveDescription"].HeaderText = "Active";
            dgvApprovers.Columns["RejectionReason"].HeaderText = "Rejection Reason";

            // Width
            dgvApprovers.Columns["Approver"].Width = 80;
            dgvApprovers.Columns["ApprovalLevel"].Width = 50;
            dgvApprovers.Columns["ApprovalSubLevel"].Width = 70;
            dgvApprovers.Columns["Status"].Width = 70;
            dgvApprovers.Columns["ActiveDescription"].Width = 50;
            dgvApprovers.Columns["RejectionReason"].Width = 150;

            // Visible
            foreach (DataColumn dc in dtApprovers.Columns)
            {
                if (!columnsvisible.Contains(dc.ColumnName))
                {
                    dgvApprovers.Columns[dc.ColumnName].Visible = false;
                }
            }

            // Ordinal
            dgvApprovers.Columns["Approver"].DisplayIndex = 1;
            dgvApprovers.Columns["ApprovalLevel"].DisplayIndex = 2;
            dgvApprovers.Columns["ApprovalSubLevel"].DisplayIndex = 3;
            dgvApprovers.Columns["Status"].DisplayIndex = 4;
            dgvApprovers.Columns["ActiveDescription"].DisplayIndex = 5;
            dgvApprovers.Columns["RejectionReason"].DisplayIndex = 6;

            // AutoFill
            dgvApprovers.Columns["RejectionReason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // //////////////////////////////////////////////////////////////
            // Populate the other fields
            // //////////////////////////////////////////////////////////////

            // HK : CJones : 21-12-2009 : Class written by JU are now obsolete.
            // Use new classes by CJones
            // Application
            //Disney.DA.EAS.DSSPAPP dsspapp = new Disney.DA.EAS.DSSPAPP(_dbparamref, _username);
            //dsspapp.GetApplicationDetails(_dsspare.ApplicationID);

            // Module
            //Disney.DA.EAS.DSSPMOD dsspmod = new Disney.DA.EAS.DSSPMOD(_dbparamref, _username);
            //dsspmod.GetModuleDetails(_dsspare.ModuleID);

            // Function
            //Disney.DA.EAS.DSSPFCT dssfct = new Disney.DA.EAS.DSSPFCT(_dbparamref, _username);
            //dssfct.GetFunctionDetails(_dsspare.FunctionID);

            //txtbApplication.Text    = dsspapp.ApplicationDesc;
            //txtbModule.Text         = dsspmod.ModuleName;
            //txtbFunction.Text       = dssfct.FunctionName;


            txtbApplication.Text    = _dsspare.ApplicationDesc;
            txtbModule.Text         = _dsspare.ModuleName;
            txtbFunction.Text       = _dsspare.FunctionName;

            txtbDetails.Text        = _dsspare.RequestDetails;
            txtbUser.Text           = _dsspare.CreateUser;

            txtCrtDate.Text = _dsspare.CreateDateTime.ToLongDateString();

            // HK : 20-01-2010 : Fix Bug 284
            if (_dsspare.ChangedDate.Day == 01 && _dsspare.ChangedDate.Month == 01 &&
                            _dsspare.ChangedDate.Year == 01)
            {
                txtChgDate.Text = "";
            }
            else
            {
                txtChgDate.Text = _dsspare.ChangedDate.ToLongDateString();
            }

        }
    

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }
    }
}