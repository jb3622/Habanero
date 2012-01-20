using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Globalization;
using System.Threading;
//using Disney.DA.IP400;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASUI
{
    public partial class frmEASSubmitters : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private iDB2Connection DB2connection;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private DataTable _dtData;
        private Form _mdiparent;
        private DataTable _dtGridDataTable;

        DataTable _dtApplication;
        DataTable _dtModule;
        DataTable _dtFunction;
        DataTable _dtApprovalStatus;
        DataTable _dtDetails;
        DataView _dvDetails;

        Dictionary<string, int> _dicModule = new Dictionary<string, int>();
        Dictionary<string, int> _dicFunction = new Dictionary<string, int>();
        Dictionary<string, int> _dicApprovalStatus = new Dictionary<string, int>();

        private Boolean bDataGridSetup;

        private string _applicationfilter = "ApplicationId = ";

        public frmEASSubmitters(ASNA.VisualRPG.Runtime.Database dbparamref,
                                iDB2Connection DB2connection,
                                Disney.Menu.Users username,
                                Disney.Menu.Environments paramenv,
                                Form Form1)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            this.DB2connection = DB2connection;
            _username = username;
            _paramenv = paramenv;
            _mdiparent = Form1;

            SetupDataTable();
        }

        private void SetupDataTable()
        {
            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, DB2connection, _username);

            _dtApplication = new DataTable();
            _dtModule = new DataTable();
            _dtFunction = new DataTable();
            _dtApprovalStatus = new DataTable();

            _dtApplication = dsspare.GetApplicationTbl();

            cbxApplication.DisplayMember = "Application";
            cbxApplication.ValueMember = "ApplicationId";
            cbxApplication.DataSource = _dtApplication;

            _dtModule = dsspare.GetModuleTbl();
            for (int i = 0; i < _dtModule.Rows.Count; i++)
            {
                try
                {
                    _dicModule.Add(Convert.ToString(_dtModule.Rows[i]["ModuleIdName"]), Convert.ToInt32(_dtModule.Rows[i]["ModuleId"]));
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("An element with Key = \"{1}\" already exists");
                }
            }

            foreach (KeyValuePair<string, int> kvp in _dicModule)
            {
                chklbModule.Items.Add(kvp.Key);
            }

            _dtFunction = dsspare.GetFunctionTbl();

            for (int i = 0; i < _dtFunction.Rows.Count; i++)
            {
                try
                {
                    _dicFunction.Add(Convert.ToString(_dtFunction.Rows[i]["FunctionCodeName"]), Convert.ToInt32(_dtFunction.Rows[i]["FunctionId"]));
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("An element with Key = \"{1}\" already exists");
                }
            }

            foreach (KeyValuePair<string, int> kvp in _dicFunction)
            {
                chklbFunction.Items.Add(kvp.Key);
            }

            _dicApprovalStatus.Add("Outstanding", 1);
            _dicApprovalStatus.Add("Approved", 2);
            _dicApprovalStatus.Add("Rejected", 3);

            foreach (KeyValuePair<string, int> kvp in _dicApprovalStatus)
            {
                chklbApprovalStatus.Items.Add(kvp.Key);
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Int64 requestid = 1;

            frmEASRequest frmeasrequest = new frmEASRequest(_dbparamref, DB2connection, _username, _paramenv, _mdiparent, requestid);
            frmeasrequest.ShowDialog();
            frmeasrequest = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string spiceponumber = "A5196";
            short spicepoversion = 1;

            frmPoModificationSummary frmpomodificationsummary = new frmPoModificationSummary(_dbparamref, _username, _paramenv, _mdiparent, spiceponumber, spicepoversion);

            frmpomodificationsummary.ShowDialog();

            frmpomodificationsummary = null;

        }


        private void chklbModule_Click(object sender, EventArgs e)
        {
        }

        private void chklbFunction_Click(object sender, EventArgs e)
        {
        }

        private void chklbApprovalStatus_Click(object sender, EventArgs e)
        {
        }

        private void btnApplyFilters_Click(object sender, EventArgs e)
        {
            int ito;
            int ifrom;
            Boolean bdisplayall;
            int applicationid;

            applicationid = Convert.ToInt32(cbxApplication.SelectedValue);

            if (Int32.TryParse(txtbxFrom.Text, out ifrom) == false)
            {
                ifrom = 0;
            }

            if (Int32.TryParse(txtbxTo.Text, out ito) == false)
            {
                ito = Int32.MaxValue;
            }

            if (cbxDisplayAll.CheckState == CheckState.Checked)
            {
                bdisplayall = true;
            }
            else
            {
                bdisplayall = false;
            }

            Cursor.Current = Cursors.WaitCursor;

            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, DB2connection, _username);

            try
            {
                _dtDetails = dsspare.SearchForSubmittersData(applicationid, chklbModule, chklbFunction, chklbApprovalStatus, ifrom, ito, txtbxDetails.Text.ToUpper(), bdisplayall);
            }
            catch (DataException dEx)
            {
                MessageBox.Show(dEx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (bDataGridSetup == false && _dtDetails != null)
            {
                SetupDataGrid();

                bDataGridSetup = true;
            }
            else
            {
                DataView _dvDetails = new DataView(_dtDetails);
                _dvDetails.Sort = "CreateDateTime, RequestId";

                dgvDetails.DataSource = _dvDetails;
            }

            Cursor.Current = Cursors.Default;
        }

        private void SetupDataGrid()
        {
            DataView _dvDetails = new DataView(_dtDetails);
            _dvDetails.Sort = "CreateDateTime, RequestId";

            dgvDetails.DataSource = _dvDetails;

            dgvDetails.Columns["RequestId"].HeaderText = "Request ID";
            dgvDetails.Columns["ApplicationName"].HeaderText = "Application";
            dgvDetails.Columns["ModuleName"].HeaderText = "Module";
            dgvDetails.Columns["FunctionName"].HeaderText = "Function";
            dgvDetails.Columns["Details"].HeaderText = "Details";
            dgvDetails.Columns["Submitted"].HeaderText = "Submitted User";
            dgvDetails.Columns["Status"].HeaderText = "Status";
            dgvDetails.Columns["CreateDateTime"].HeaderText = "Creation Date";

            dgvDetails.Columns["RequestId"].Width = 75;
            dgvDetails.Columns["ApplicationName"].Width = 75;
            dgvDetails.Columns["ModuleName"].Width = 85;
            dgvDetails.Columns["FunctionName"].Width = 75;
            dgvDetails.Columns["Details"].Width = 180;
            dgvDetails.Columns["Submitted"].Width = 85;
            dgvDetails.Columns["Status"].Width = 85;
            dgvDetails.Columns["CreateDateTime"].Width = 90;

            dgvDetails.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvDetails.Columns["FunctionId"].Visible = false;
            dgvDetails.Columns["ActiveFlag"].Visible = false;
        }

        private void dgvDetails_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Int64 requestid;

            string details;
            string function;
            int functionid;
            char[] delimit = new char[] { ' ' };
                      
            
            details = Convert.ToString(dgvDetails["Details", e.RowIndex].Value);
            function = Convert.ToString(dgvDetails["FunctionName", e.RowIndex].Value);
            functionid = Convert.ToInt32(dgvDetails["FunctionID", e.RowIndex].Value);
            requestid = Convert.ToInt64(dgvDetails["RequestId", e.RowIndex].Value);

            Cursor.Current = Cursors.WaitCursor;

            frmEASRequest frmeasrequest = new frmEASRequest(_dbparamref, DB2connection, _username, _paramenv, _mdiparent, requestid);

            frmeasrequest.ShowDialog();
            frmeasrequest = null;
            
        }

        private void btnResetFilters_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chklbModule.Items.Count; i++)
            {
                chklbModule.SetItemChecked(i, false);
            }

            for (int i = 0; i < chklbFunction.Items.Count; i++)
            {
                chklbFunction.SetItemChecked(i, false);
            }

            for (int i = 0; i < chklbApprovalStatus.Items.Count; i++)
            {
                chklbApprovalStatus.SetItemChecked(i, false);
            }

            if (_dtDetails != null)
            {
                _dtDetails.Clear();
            }

            txtbxDetails.Text = "";
            txtbxFrom.Text = "";
            txtbxTo.Text = "";

            cbxDisplayAll.Checked = true;
        }

        private void cbxApplication_SelectedIndexChanged(object sender, EventArgs e)
        {
            int applicationid = Convert.ToInt32(cbxApplication.SelectedValue);
            _applicationfilter = "ApplicationId = " + applicationid.ToString();
        }

        private void FilterFunctionCheckList(int[] moduleid)
        {
            string dvfilter;

            if (moduleid.Length > 0)
            {
                dvfilter = _applicationfilter + " and ("; ;

                for (int i = 0; i < moduleid.Length; i++)
                {
                    if (i == 0)
                    {
                        dvfilter = dvfilter + " ModuleId = " + moduleid[i].ToString() + " ";
                    }

                    if (i > 0)
                    {
                        dvfilter = dvfilter + " or ModuleId = " + moduleid[i].ToString();
                    }
                }
                dvfilter = dvfilter + ")";
            }
            else
            {
                dvfilter = _applicationfilter;
            }

            DataView dvFunction = new DataView(_dtFunction);

            dvFunction.RowFilter = dvfilter;

            chklbFunction.Items.Clear();

            for (int i = 0; i < dvFunction.Count; i++)
            {
                try
                {
                    chklbFunction.Items.Add(dvFunction[i]["FunctionCodeName"]);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("An element with Key = \"{1}\" already exists");
                }
            }
        }

        private int[] GetModuleId(string[] moduleidname)
        {
            int[] moduleid = new int[moduleidname.Length];
            int counter = 0;

            for (int j = 0; j < moduleidname.Length; j++)
            {
                for (int i = 0; i < _dtModule.Rows.Count; i++)
                {
                    if (Convert.ToString(_dtModule.Rows[i]["ModuleIDName"]) == moduleidname[j])
                    {
                        moduleid[counter] = Convert.ToInt32(_dtModule.Rows[i]["ModuleId"]);
                        counter++;
                    }
                }
            }

            return moduleid;
        }

        private void chklbModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            int[] moduleid;
            int selectedindex;

            selectedindex = chklbModule.SelectedIndex;

            string[] destination = new string[chklbModule.CheckedItems.Count];
            chklbModule.CheckedItems.CopyTo(destination, 0);
            string rooms = string.Join(" ", destination);

            moduleid = GetModuleId(destination);

            FilterFunctionCheckList(moduleid);
        }

        private void dgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == -1)
            {
                return;
            }

            if (dgvDetails.Columns[e.ColumnIndex].Name == "CreateDateTime")
            {
                if (e.Value != null)
                {
                    DateTime creationdate;
                    creationdate = DateTime.Parse(e.Value.ToString());
                    e.Value = creationdate.ToString("dd - MMM - yyyy").ToUpper();
                    e.FormattingApplied = true;

                }
            }
        }

        private string GetInquiryStatus(int requestid)
        {
            string statusdetails = String.Empty;

            Disney.DA.EAS.DSSPAAR dsspaar = new Disney.DA.EAS.DSSPAAR(_dbparamref, _username);

            try
            {
                statusdetails = dsspaar.GetStatusDetails(requestid);
            }
            catch (DataException dEx)
            {
                MessageBox.Show(dEx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return statusdetails;
        }
    }
}