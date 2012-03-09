using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Disney.DA.IP400;
using Disney.Spice.POBO;

namespace Disney.Spice.POUI
{
    public partial class frmSubmitToEAS : Form
    {

        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users               _username;
        private Disney.Menu.Environments        _paramenv;
        private DataTable                       _dtData;
        private Form                            _mdiparent;
        private DataTable                       _dtGridDataTable;
        private DataTable                       _ststusdescription;
        PurchaseOrder                           _porder;


        public frmSubmitToEAS()
        {
            InitializeComponent();
        }

        public frmSubmitToEAS(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1, DataTable dtData)
        {
            InitializeComponent();

            _dbparamref     = dbparamref;
            _username       = username;
            _paramenv       = paramenv;
            _mdiparent      = Form1;
            _dtData         = dtData;

            // HK : 20-01-2010 : Get the Status of the POs
            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);

            // ///////////////////////////////////////////////////////
            // HK : 07-01-2010 : Get description for status from the DA
            // Fix Bug : 207
            // ///////////////////////////////////////////////////////
            DSSPPOScls dsspposcls = new DSSPPOScls(_dbparamref);

            DataTable ststusdescription = new DataTable();
            _ststusdescription = dsspposcls.GetStatusDescriptionsDataTable();

            SetupDataTable();

            SetupGrid();

        }

        private void SetupDataTable ()
        {
            string  spiceponumber;
            string  status;
            short   changesequence;

            DataTable dtTemp;
            dtTemp = _dtData.Copy();
            //_dtData.Clear();
            _dtGridDataTable = dtTemp.Clone();

            DataRow[] rows = dtTemp.Select(" clmSelect = true");

            foreach (DataRow dr in rows)
            {
                // HK : 20-01-2010 : Get the current status of the PO from the dataqueue
                spiceponumber = Convert.ToString(dr["clmSpicePO"]);
                status = GetPoStatus(spiceponumber, out changesequence);

                // Assign the most up to date status from the database
                dr["clmStatus"] = status;
                // add values into the datatabled
                _dtGridDataTable.Rows.Add(dr.ItemArray);
            }

        }

        private void SetupGrid()
        {
            List<String> columnsvisible = new List<string>();
            string sexpression = "clmVendor + ' - ' + clmVendorName";
            DataGridViewImageColumn dgvicpoupdate = new DataGridViewImageColumn();

            // Add compute columns
            _dtGridDataTable.Columns.Add("VendorCodeAndName", typeof(string));
            _dtGridDataTable.Columns["VendorCodeAndName"].Expression = sexpression;

            columnsvisible.Add ("clmSpicePO");
            columnsvisible.Add ("clmRevisionNumber");
            columnsvisible.Add ("clmIPPO");
            columnsvisible.Add ("VendorCodeAndName");

            dgvSubmit.DataSource = _dtGridDataTable;

            // Header Text
            dgvSubmit.Columns["clmSpicePO"].HeaderText = "Spice PO #";
            dgvSubmit.Columns["clmRevisionNumber"].HeaderText = "Revision #";
            dgvSubmit.Columns["clmIPPO"].HeaderText = "IP PO #";
            dgvSubmit.Columns["VendorCodeAndName"].HeaderText = "Vendor Code - Name";

            // Width
            dgvSubmit.Columns["clmSpicePO"].Width = 90; ;
            dgvSubmit.Columns["clmRevisionNumber"].Width = 90;
            dgvSubmit.Columns["clmIPPO"].Width = 80;
            dgvSubmit.Columns["VendorCodeAndName"].Width = 250;

            // ReadOnly

            
            // Visible
            foreach (DataColumn dc in  _dtData.Columns)
            {
                if (!columnsvisible.Contains (dc.ColumnName))
                {
                    dgvSubmit.Columns[dc.ColumnName].Visible = false;

                }
            }

        
            // Backgroung color of cell

           
            // Column Header Alignment

            
            // Column Alignment


            // Columns frozen



            // New Display only columns in Datagridview
            dgvSubmit.Columns.Add("POStatusDesc", "PO Status");
            
            dgvicpoupdate.Name = "POUpdate";
            dgvicpoupdate.HeaderText = "PO Update";
            dgvSubmit.Columns.Add(dgvicpoupdate);


        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string  spiceponumber;
            string  sclmstatus;
            Boolean bSuccess;
            string  sippo;
            Int32   iippo;
            string  srequestid = String.Empty;
            short   pochangesequence;

            // HK : 17-12-2009 : Call method on iSeries
            WriteToiSeriesDTAQ writetoiseriesdtaq = new WriteToiSeriesDTAQ(_dbparamref, _username);

            //for (int i = 0; i < _dtData.Rows.Count; i++)
            for (int i = 0; i < dgvSubmit.Rows.Count; i++)
            {

                spiceponumber   = Convert.ToString(dgvSubmit["clmSpicePO",  i].Value);
                sippo           = Convert.ToString(dgvSubmit["clmIPPO",     i].Value);

                // If the status is Original Pending
                //sclmstatus = Convert.ToString(dgvSubmit["clmStatus", i].Value);
                sclmstatus = GetPoStatus(spiceponumber, out pochangesequence);

                // Get the Status from the database before sending the appropriate request 
                // to the dataqueue

                if (sclmstatus == "OP" || sclmstatus == "RP")
                {

                    //if (Int32.TryParse(sippo, out iippo))
                    if (pochangesequence == 0)
                    {
                        srequestid = "REQCRTPO";
                    }
                    else if (pochangesequence > 0)
                    {
                        srequestid = "REQCHGPO";
                    }

                    // HK : CJones : 22-12-2009 : Use the flag "REQCRTPO" for Submit To EAS
                    //bSuccess = writetoiseriesdtaq.WritePOtoDtaQ("REQCRTPO", spiceponumber);
                    bSuccess = writetoiseriesdtaq.WritePOtoDtaQ(srequestid, spiceponumber);

                    if (bSuccess == false)
                    {
                        MessageBox.Show(this.Text, "There was an error sending the Submit request for - " + spiceponumber + " to the iSeries dataqueue.");

                    }
                }
            }
            
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvSubmit_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == dgvSubmit.Columns["POUpdate"].Index)
            {
                string spath = Application.ExecutablePath;
                spath = spath.Replace("DisneyMenu.exe", "\\");
                string s = dgvSubmit["clmStatus", e.RowIndex].Value.ToString();

                switch (s)
                {
                    case "OP":
                        e.Value = Image.FromFile (spath + "green2.jpg");
                        break;

                    case "RP":
                        e.Value = Image.FromFile(spath + "green2.jpg");
                        break;

                     case "OA":
                        e.Value = Image.FromFile (spath + "red2.jpg");
                        break;

                    default :
                        e.Value = Image.FromFile(spath + "red2.jpg");
                        break;

                }
            }

            if (e.RowIndex > -1 && e.ColumnIndex == dgvSubmit.Columns["POStatusDesc"].Index)
            {
                string s = dgvSubmit["clmStatus", e.RowIndex].Value.ToString();
                e.Value = GetPOStatusDescription(s);
            }
        }

        private string GetPOStatusDescription(string postatus)
        {
            // HK : 12-01-2010 : Fix Bug 207.
            string code;
            string description = String.Empty;
            string fulldescription;
            int fulldescriptionlength;

            for (int i = 0; i < _ststusdescription.Rows.Count; i++)
            {
                fulldescription = Convert.ToString(_ststusdescription.Rows[i]["clmstatusdesc"]);
                code = fulldescription.Substring(0, 2);

                fulldescriptionlength = fulldescription.Length;
                if (code.ToString() == postatus.ToString())
                {
                    description = fulldescription.Substring(3, fulldescriptionlength - 4);

                    return description.Trim();
                }
            }

            switch (postatus)
            {
                case "OP":
                    return "Original Pending";

                case "RP":
                    return "Revised Pending";

                case "OA":
                    return "Original Approved";

                default :
                    return "Unknown Status";
            }
                
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
            
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
           Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
 
        }

        private void frmSubmitToEAS_Load(object sender, EventArgs e)
        {

        }

        private string GetPoStatus(string spiceponumber, out short changesequence)
        {
            string  status;
            status = _porder.GetPOStatus(spiceponumber, out changesequence);

            return status;

        }
    }
}