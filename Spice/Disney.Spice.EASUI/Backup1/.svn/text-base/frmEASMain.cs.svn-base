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
using Disney.DA.IP400;
using Disney.Spice.POBO;

namespace Disney.Spice.EASUI
{
    public partial class frmEASMain : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
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
        
        PurchaseOrder _porder;

        // Default constructor
        public frmEASMain()
        {
            InitializeComponent();
        }

        public frmEASMain(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;
            _mdiparent = Form1;

            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);

            SetupDataTable();
        }

        private void SetupDataTable()
        {
            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, _username);

            _dtApplication      = new DataTable();
            _dtModule           = new DataTable();
            _dtFunction         = new DataTable();
            _dtApprovalStatus   = new DataTable();

            _dtApplication = dsspare.GetApplicationTbl();

            cbxApplication.DisplayMember = "Application";
            cbxApplication.ValueMember = "ApplicationId";

            cbxApplication.DataSource = _dtApplication;

            // At this stage the combobox for Application will have its 
            // very first item selected. We now have to filter the module checklist box and
            // function checklist box appropriately.
            
            // Comment the capture of application id below, as this is done in 
            // SelectionChanged event of the Applicatini checkbox.
            //applicationid = Convert.ToInt32(cbxApplication.SelectedValue);
            //_applicationfilter = _applicationfilter + applicationid.ToString ();

            // Module Checklistbox
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

            // Function CheckListBox
            _dtFunction = dsspare.GetFunctionTbl();

            // Apply the application level filter to the 
            // function table
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

            // Approval Status :
            // Hardcode for the time being. Will have to be read from a XML file.
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

            // Application Id
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

            // hourglass cursor
            Cursor.Current = Cursors.WaitCursor;

            Disney.DA.EAS.DSSPARE dsspare = new Disney.DA.EAS.DSSPARE(_dbparamref, _username);

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
                dgvDetails.DataSource = _dtDetails;
            }

            // Default cursor
            Cursor.Current = Cursors.Default;
        }

        private void SetupDataGrid ()
        {
            dgvDetails.DataSource = _dtDetails;

            // Header Text
            dgvDetails.Columns["RequestId"].      HeaderText = "Request ID";
            dgvDetails.Columns["ApplicationName"].HeaderText = "Application";
            dgvDetails.Columns["ModuleName"].     HeaderText = "Module";
            dgvDetails.Columns["FunctionName"].   HeaderText = "Function";
            dgvDetails.Columns["Details"].        HeaderText = "Details";
            dgvDetails.Columns["Submitted"].      HeaderText = "Submitted User";
            dgvDetails.Columns["Status"].         HeaderText = "Status";
            dgvDetails.Columns["CreateDateTime"]. HeaderText = "Creation Date";
            dgvDetails.Columns["ActiveFlag"].     HeaderText = "Active";

            // Width
            dgvDetails.Columns["RequestId"].      Width = 50;
            dgvDetails.Columns["ApplicationName"].Width = 60;
            dgvDetails.Columns["ModuleName"].     Width = 75;
            dgvDetails.Columns["FunctionName"].   Width = 75;
            dgvDetails.Columns["Details"].        Width = 170;
            dgvDetails.Columns["Submitted"].      Width = 80;
            dgvDetails.Columns["Status"].         Width = 80;
            dgvDetails.Columns["CreateDateTime"]. Width = 95;
            dgvDetails.Columns["ActiveFlag"].     Width = 50;

            // AutoFill
            dgvDetails.Columns["Details"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Visible
            dgvDetails.Columns["FunctionId"].Visible = false;
        }

        private void dgvDetails_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string spiceponumber = String.Empty;
            //short spicepoversion;
            Int64 requestid;

            // Item Stuff
            short classcode  =0;
            int   vendorcode =0;
            short stylecode  =0;
            short colourcode =0;
            short itemsize   =0;

            // Extract the PO number and version number from the "Details" column
            string      details;
            string      function;
            string      status;
            int         functionid;
            string[]    items;
            char[]      delimit = new char[] { ' ' };
            string      activeflag;

            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Cursor.Current = Cursors.WaitCursor;

            details     = Convert.ToString(dgvDetails["Details",     e.RowIndex].Value);
            function    = Convert.ToString(dgvDetails["FunctionName",e.RowIndex].Value);
            functionid  = Convert.ToInt32 (dgvDetails["FunctionID",  e.RowIndex].Value);
            requestid   = Convert.ToInt64 (dgvDetails["RequestId",   e.RowIndex].Value);
            status      = Convert.ToString(dgvDetails["Status",      e.RowIndex].Value);
            activeflag  = Convert.ToString(dgvDetails["ActiveFlag",  e.RowIndex].Value);

            int pomark  = details.IndexOf("PO=");
            items       = details.Split(delimit);

            foreach (string substr in items)
            {
                pomark = substr.IndexOf("PO=");
                if (pomark != -1)
                {
                    spiceponumber = substr.Substring(3, substr.Length - 3);
                    break;
                }
            }

            // If Po Number cannot be extracted from the "Details" column
            // then dont launch the form.
            if (String.IsNullOrEmpty(spiceponumber))
            {
                MessageBox.Show("PO number not found in Details colum", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            status = GetInquiryStatus(Convert.ToInt32 (requestid));
            if ((functionid == 10003 || functionid == 10004 || functionid == 10005 || functionid == 10013) && (status.ToUpper() == "OUTSTANDING" && activeflag.ToUpper() == "ACTIVE") )
            {
                // hourglass cursor
                Cursor.Current = Cursors.WaitCursor;

                POapprove frmpomodification = new POapprove(_dbparamref, _username, _paramenv, spiceponumber, functionid, requestid);

                frmpomodification.ShowDialog();
                frmpomodification = null;
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;

                frmEASRequest frmeasrequest = new frmEASRequest(_dbparamref, _username, _paramenv, _mdiparent, requestid);
                frmeasrequest.ShowDialog();
                frmeasrequest = null;
            }

            // Item EAS stuff
            if (functionid == 10008)
            {
                Cursor.Current = Cursors.WaitCursor;

                EASApproveReject frmapprovereject = new EASApproveReject(_dbparamref, _username, _paramenv, classcode, vendorcode, stylecode, colourcode, itemsize, requestid);
                //frmApproveRejectItemSizeChangeCaseDetails frmapprovereject = new frmApproveRejectItemSizeChangeCaseDetails(_dbparamref, _username, _paramenv, classcode, vendorcode, stylecode, colourcode, itemsize);
                frmapprovereject.ShowDialog();
                frmapprovereject = null;
            }
        }

        private void btnResetFilters_Click(object sender, EventArgs e)
        {
            // Reset the selections
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

            // Clear the contents of the grid
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
                    //_dicFunction.Add(Convert.ToString(_dtFunction.Rows[i]["FunctionCodeName"]), Convert.ToInt32(_dtFunction.Rows[i]["FunctionId"]));
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
            //string moduleidname;

            selectedindex = chklbModule.SelectedIndex;

            string[] destination = new string[chklbModule.CheckedItems.Count];
            chklbModule.CheckedItems.CopyTo(destination, 0);
            string rooms = string.Join(" ", destination);
            //_study.CurrentStudy.Rooms = rooms; //this is where I set the object that is saved to the DB later.

            //String.Join(";", chklbModule.CheckedItems.Cast<String>().ToArray<String>());

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
            // Apply the filters again to the check list box
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

        private string GetPoStatus(string spiceponumber, out short changesequence)
        {
            string status;
            status = _porder.GetPOStatus(spiceponumber, out changesequence);

            return status;
        }

        private string GetInquiryStatus(int requestid)
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