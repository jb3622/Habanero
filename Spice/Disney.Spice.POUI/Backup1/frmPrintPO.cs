using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Disney.DA.IP400;
using Disney.Spice.POBO;

namespace Disney.Spice.POUI
{
    public partial class frmPrintPO : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private DataTable _dtData;
        private Form _mdiparent;
        private DataTable _dtGridDataTable;
        private DataTable _ststusdescription;
        PurchaseOrder _porder;

        public frmPrintPO(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1, DataTable dtData)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;
            _mdiparent = Form1;
            _dtData = dtData;

            // Load image list
            string spath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            try
            {
                imageList.Images.Add(Image.FromFile(Path.Combine(spath, "Red.png")));
                imageList.Images.Add(Image.FromFile(Path.Combine(spath, "Amber.png")));
                imageList.Images.Add(Image.FromFile(Path.Combine(spath, "Green.png")));
            }
            catch (Exception)
            {
                imageList = null;
                MessageBox.Show("Traffic light images are missing");
            }

            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);

            DSSPPOScls dsspposcls = new DSSPPOScls(_dbparamref);

            DataTable ststusdescription = new DataTable();
            _ststusdescription = dsspposcls.GetStatusDescriptionsDataTable();

            SetupDataTable();

            SetupGrid();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string spiceponumber;
            string sclmstatus;
            Boolean bSuccess;
            short pochangesequence;

            // HK : 17-12-2009 : Call method on iSeries
            WriteToiSeriesDTAQ writetoiseriesdtaq = new WriteToiSeriesDTAQ(_dbparamref, _username);

            //for (int i = 0; i < _dtData.Rows.Count; i++)
            for (int i = 0; i < dgvPrintPO.Rows.Count; i++)
            {
                spiceponumber = Convert.ToString(dgvPrintPO["clmSpicePO", i].Value);
                
                // If the status is Original Pending
                //sclmstatus = Convert.ToString(_dtData.Rows[i]["clmStatus"]);
                //sclmstatus = Convert.ToString(dgvPrintPO["clmStatus", i].Value);
                sclmstatus = GetPoStatus(spiceponumber, out pochangesequence);

                if (sclmstatus == "OA" || sclmstatus == "RA")
                {
                    
                    // HK : CJones : 22-12-2009 : Use the flag "REQPRTPO" for Print
                    bSuccess = writetoiseriesdtaq.WritePOtoDtaQ("REQPRTPO", spiceponumber, _paramenv.Printer1, _paramenv.Printer2, _paramenv.Printer3);

                    if (bSuccess == false)
                    {
                        MessageBox.Show(this.Text, "There was an error sending the Print request for - " + spiceponumber + " to the iSeries dataqueue", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            Close();
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

        private void SetupDataTable()
        {
            string spiceponumber;
            string status;
            short changesequence;

            DataTable dtTemp;
            dtTemp = _dtData.Copy();

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

            columnsvisible.Add("clmSpicePO");
            columnsvisible.Add("clmRevisionNumber");
            columnsvisible.Add("clmIPPO");
            columnsvisible.Add("VendorCodeAndName");

            dgvPrintPO.DataSource = _dtGridDataTable;

            // Header Text
            dgvPrintPO.Columns["clmSpicePO"].HeaderText = "Spice PO #";
            dgvPrintPO.Columns["clmRevisionNumber"].HeaderText = "Revision #";
            dgvPrintPO.Columns["clmIPPO"].HeaderText = "IP PO #";
            dgvPrintPO.Columns["VendorCodeAndName"].HeaderText = "Vendor Code - Name";

            // Width
            dgvPrintPO.Columns["clmSpicePO"].Width = 90; ;
            dgvPrintPO.Columns["clmRevisionNumber"].Width = 90;
            dgvPrintPO.Columns["clmIPPO"].Width = 80;
            dgvPrintPO.Columns["VendorCodeAndName"].Width = 250;


            // Visible
            foreach (DataColumn dc in _dtData.Columns)
            {
                if (!columnsvisible.Contains(dc.ColumnName))
                {
                    dgvPrintPO.Columns[dc.ColumnName].Visible = false;
                }
            }

            // New Display only columns in Datagridview
            dgvPrintPO.Columns.Add("POStatusDesc", "PO Status");

            dgvicpoupdate.Name = "POUpdate";
            dgvicpoupdate.HeaderText = "PO Update";
            dgvPrintPO.Columns.Add(dgvicpoupdate);
        }

        private void dgvPrintPO_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == dgvPrintPO.Columns["POUpdate"].Index)
            {
                if (imageList != null)
                {
                    string s = dgvPrintPO["clmStatus", e.RowIndex].Value.ToString();
                    switch (s)
                    {
                        case "OA":
                            e.Value = imageList.Images[2];  // Green
                            break;

                        case "RA":
                            e.Value = imageList.Images[2];  // Green
                            break;

                        default:
                            e.Value = imageList.Images[0];  // Red
                            break;
                    }
                }
            }

            if (e.RowIndex > -1 && e.ColumnIndex == dgvPrintPO.Columns["POStatusDesc"].Index)
            {
                string s = dgvPrintPO["clmStatus", e.RowIndex].Value.ToString();
                e.Value = GetPOStatusDescription(s);
            }
        }

        private string GetPOStatusDescription(string postatus)
        {
            // HK : 12-01-2010
            // This fixes Bug 207
            string code;
            string description = String.Empty;
            string fulldescription;
            int fulldescriptionlength;

            for (int i = 0; i < _ststusdescription.Rows.Count; i++)
            {
                fulldescription = Convert.ToString(_ststusdescription.Rows[i]["clmstatusdesc"]);
                code = fulldescription.Substring (0, 2);

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

                default:
                    return "Unknown Status";
            }

        }

        private string GetPoStatus(string spiceponumber, out short changesequence)
        {
            string status;
            status = _porder.GetPOStatus(spiceponumber, out changesequence);

            return status;

        }

    }

}