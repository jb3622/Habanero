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
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASUI
{
    public partial class frmEASMain : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private IBM.Data.DB2.iSeries.iDB2Connection DB2connection;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private Form _mdiparent;

        DataTable _dtApplication;
        DataTable _dtModule;
        DataTable _dtFunction;
        DataTable _dtApprovalStatus;
        DataTable _dtDetails;
        
        Dictionary<string, int> _dicModule = new Dictionary<string, int>();
        Dictionary<string, int> _dicFunction = new Dictionary<string, int>();
        Dictionary<string, int> _dicApprovalStatus = new Dictionary<string, int>();

        private Boolean bDataGridSetup;

        private string _applicationfilter = "ApplicationId = ";
                
        public frmEASMain(ASNA.VisualRPG.Runtime.Database dbparamref,
                          IBM.Data.DB2.iSeries.iDB2Connection DB2connect,
                          Disney.Menu.Users user,
                          Disney.Menu.Environments paramenv,
                          Form Form1)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            this.DB2connection = DB2connect;
            _username = user;
            _paramenv = paramenv;
            _mdiparent = Form1;
                        
            SetupDataTable();
        }

        private void SetupDataTable()
        {
            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, DB2connection, _username);

            _dtApplication      = new DataTable();
            _dtModule           = new DataTable();
            _dtFunction         = new DataTable();
            _dtApprovalStatus   = new DataTable();

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
                chklbModule.Items.Add (kvp.Key);
            }

            _dtFunction = dsspare.GetFunctionTbl();

            DataView dvFunctionTable = new DataView(_dtFunction);
            dvFunctionTable.RowFilter = _applicationfilter;

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

        private void btnApplyFilters_Click(object sender, EventArgs e)
        {
            int ito;
            int ifrom;
            Boolean bdisplayall;
            int applicationid;

            Cursor.Current = Cursors.WaitCursor;

            applicationid = Convert.ToInt32 (cbxApplication.SelectedValue);

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


            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, DB2connection, _username);
            try
            {
                _dtDetails = dsspare.SearchForApprovalRequests(applicationid, chklbModule, chklbFunction, chklbApprovalStatus, ifrom, ito, txtbxDetails.Text.ToUpper (), bdisplayall);
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
                _dvDetails.Sort = "CreateDateTime, RequestID";
 
                dgvDetails.DataSource = _dvDetails;
            }

            Cursor.Current = Cursors.Default;
        }

        private void SetupDataGrid()
        {
            DataView _dvDetails = new DataView(_dtDetails);
            _dvDetails.Sort = "CreateDateTime, RequestID";

            dgvDetails.DataSource = _dvDetails;

            dgvDetails.Columns["RequestId"].      HeaderText = "Request ID";
            dgvDetails.Columns["ApplicationName"].HeaderText = "Application";
            dgvDetails.Columns["ModuleName"].     HeaderText = "Module";
            dgvDetails.Columns["FunctionName"].   HeaderText = "Function";
            dgvDetails.Columns["Details"].        HeaderText = "Details";
            dgvDetails.Columns["Submitted"].      HeaderText = "Submitted User";
            dgvDetails.Columns["Status"].         HeaderText = "Status";
            dgvDetails.Columns["CreateDateTime"]. HeaderText = "Creation Date";
            dgvDetails.Columns["ActiveFlag"].     HeaderText = "Active";

            dgvDetails.Columns["RequestId"].      Width = 50;
            dgvDetails.Columns["ApplicationName"].Width = 60;
            dgvDetails.Columns["ModuleName"].     Width = 75;
            dgvDetails.Columns["FunctionName"].   Width = 75;
            dgvDetails.Columns["Details"].        Width = 170;
            dgvDetails.Columns["Submitted"].      Width = 80;
            dgvDetails.Columns["Status"].         Width = 80;
            dgvDetails.Columns["CreateDateTime"]. Width = 95;
            dgvDetails.Columns["ActiveFlag"].     Width = 50;

            dgvDetails.Columns["Details"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvDetails.Columns["FunctionId"].Visible = false;
        }

        private void dgvDetails_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Cursor.Current = Cursors.WaitCursor;

            Int64  requestid  = Convert.ToInt64(dgvDetails["RequestId", e.RowIndex].Value);
            String status     = GetInquiryStatus(requestid);
            String activeflag = Convert.ToString(dgvDetails["ActiveFlag",  e.RowIndex].Value);

            if (!status.ToUpper().Equals("OUTSTANDING") || !activeflag.ToUpper().Equals("ACTIVE"))
            {
                DisplayApproved(requestid);
            }
            else
            {
                Int32 functionid = Convert.ToInt32(dgvDetails["FunctionID", e.RowIndex].Value);
                String details   = Convert.ToString(dgvDetails["Details", e.RowIndex].Value);
                
                switch (functionid)
                {
                    case 10003:
                    case 10004:
                    case 10005:
                    case 10013:
                        poEASrequest(requestid,functionid,details);
                        break;

                    case 10008:
                        itemEASrequest(requestid);
                        break;
                }
            }
        }

        private void poEASrequest(Int64 requestid, Int32 functionid, String details)
        {
            string spiceponumber = String.Empty;
            string[] items;
            char[] delimit = new char[] { ' ' };

            {
                items = details.Split(delimit);
                foreach (string substr in items)
                {
                    Int32 pomark = substr.IndexOf("PO=");
                    if (pomark != -1)
                    {
                        spiceponumber = substr.Substring(3, substr.Length - 3);
                        break;
                    }
                }

                if (String.IsNullOrEmpty(spiceponumber))
                {
                    MessageBox.Show("PO number not found in Details column", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                POapprove approvePO = new POapprove(_dbparamref, DB2connection, _username, _paramenv, spiceponumber, functionid, requestid);
                approvePO.ShowDialog();
                approvePO = null;
            }
            catch(Exception ex)
            {
                if (ex.Message != "Not authorised to PO")
                {
                    throw ex;
                }                
                return;
            }

        }

        private void itemEASrequest(Int64 requestid)
        {
            Cursor.Current = Cursors.WaitCursor;

            EASApproveReject frmapprovereject = new EASApproveReject(_dbparamref, DB2connection, _username, _paramenv, requestid);
            frmapprovereject.ShowDialog();
            frmapprovereject = null;
        }

        private void DisplayApproved(Int64 requestid)
        {
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

            txtbxDetails.Text   = "";
            txtbxFrom.Text      = "";
            txtbxTo.Text = "";

            cbxDisplayAll.Checked = false;
        }

        private void FilterFunctionCheckList(int[] moduleid)
        {
            string dvfilter;

            if (moduleid.Length > 0)
            {
                dvfilter = _applicationfilter + " and (";;

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

        private int[] GetModuleId(string[] moduleidname)
        {
            int[] moduleid = new int[moduleidname.Length];
            int counter = 0;

            for (int j = 0; j < moduleidname.Length; j++)
            {
                for (int i = 0; i < _dtModule.Rows.Count; i++)
                {
                    if (Convert.ToString(_dtModule.Rows[i]["ModuleIDName"]) == moduleidname[j] )
                    {
                        moduleid[counter] = Convert.ToInt32(_dtModule.Rows[i]["ModuleId"]);
                        counter++;
                    }
                }
            }

            return moduleid;
        }

        private void cbxApplication_SelectedIndexChanged(object sender, EventArgs e)
        {
            int applicationid = Convert.ToInt32(cbxApplication.SelectedValue);
            _applicationfilter = "ApplicationId = " + applicationid.ToString();
        }

        private void dgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == -1)
            {
                return;
            }

            if (dgvDetails.Columns[e.ColumnIndex].Name == "CreateDateTime")
            {
                if (e.Value != null )
                {
                    DateTime creationdate;
                    creationdate = DateTime.Parse (e.Value.ToString ());
                    e.Value = creationdate.ToString ("dd - MMM - yyyy").ToUpper ();
                    e.FormattingApplied = true;
                }
            }
        }
                
        private string GetInquiryStatus(Int64 requestid)
        {
            string statusdetails = String.Empty;

            Disney.DA.EAS.DSSPAAR dsspaar = new Disney.DA.EAS.DSSPAAR(_dbparamref, _username);

            try
            {
                statusdetails = dsspaar.GetRequestApproverStatus(requestid);
            }
            catch (DataException dEx)
            {
                MessageBox.Show(dEx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return statusdetails;
        }
    }
}