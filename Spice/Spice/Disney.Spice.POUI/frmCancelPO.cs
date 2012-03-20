using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Disney.DA.IP400;
using Disney.Spice.POBO;

namespace Disney.Spice.POUI
{
    public partial class frmCancelPO : Form
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private DataTable _dtData;
        private Form _mdiparent;
        private DataTable _dtGridDataTable;
        private DataTable _ststusdescription;
        private PurchaseOrder _porder;

        public frmCancelPO(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1, DataTable dtData)
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
                MessageBox.Show("Traffic light images are missing");
            }

            // Get the Status of the POs
            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);

            // Get description for status from the DA
            DSSPPOScls dsspposcls = new DSSPPOScls(_dbparamref, _paramenv);

            DataTable ststusdescription = new DataTable();
            _ststusdescription = dsspposcls.GetStatusDescriptionsDataTable();

            SetupDataTable();
            SetupGrid();
        }

        private void EnableDisableReasonCode()
        {
            // HK : 26-012010 : Disable ReasonCode if status in CS, OC, RC, OS, RS
            string s;
            for (int i = 0; i < dgvCancelPO.Rows.Count; i++)
            {
                s = dgvCancelPO["clmStatus", i].Value.ToString();

                if (s == "CS" || s == "OC" || s == "RC" || s == "OS" || s == "RS" || s == "IC")
                {
                    dgvCancelPO["ReasonCode", i].ReadOnly = true;
                }
            }
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            string spiceponumber;
            string sclmstatus;
            Boolean bSuccess;
            string reasoncode;
            Boolean noreasoncode = false;
            int runningcounttotalrows = 0;
            int loopcounter = 0;
            short pochangesequence;

            string sippo;
            Int32 iippo;
            string srequestid;

            // HK : 17-12-2009 : Call method on iSeries
            WriteToiSeriesDTAQ writetoiseriesdtaq = new WriteToiSeriesDTAQ(_dbparamref, _username);

            loopcounter = 0;
            runningcounttotalrows = dgvCancelPO.Rows.Count;

            do
            {
                // If the status is Original Pending
                spiceponumber = Convert.ToString(dgvCancelPO["clmSpicePO", loopcounter].Value);
                
                sclmstatus = GetPoStatus(spiceponumber, out pochangesequence);
                reasoncode = Convert.ToString (dgvCancelPO.Rows[loopcounter].Cells["ReasonCode"].Value);
                sippo = Convert.ToString(dgvCancelPO["clmIPPO", loopcounter].Value);

                if (Int32.TryParse(sippo, out iippo))
                {
                    srequestid = "REQCANIPO";
                }
                else
                {
                    srequestid = "REQCANSPO";
                }

                // HK : FC : 21-12-2009 : If reason code is not provided, then dont 
                // close the form. Filter the records that are sent o users see 
                // only records that have not been sent.
                if (String.IsNullOrEmpty(reasoncode))
                {
                    noreasoncode = true; 
                }
                // HK : FC : 04-01-2010: 
                if (!(sclmstatus == "OS" || sclmstatus == "RS") && !String.IsNullOrEmpty(reasoncode))
                {
                    // HK : CJones : 22-12-2009 : Use the flag "REQCANSPO" for Submit To EAS
                    bSuccess = writetoiseriesdtaq.WritePOtoDtaQ(srequestid, spiceponumber, reasoncode);

                    if (bSuccess == false)
                    {
                        // increment the loop
                        loopcounter++;
                        MessageBox.Show(this.Text, "There was an error sending the Cancel request for - " + spiceponumber + " to the iSeries dataqueue.", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                    else
                    {
                        // Remove record at counter position
                        dgvCancelPO.Rows.RemoveAt(loopcounter);

                        runningcounttotalrows = dgvCancelPO.Rows.Count;
                    }
                }
                else
                {
                    loopcounter++;
                }

            } while (loopcounter < runningcounttotalrows);

            // At least one record has a empty reason code
            if (noreasoncode == false)
            {
                this.DialogResult = DialogResult.OK;
                Close();
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

        private void SetupDataTable()
        {
            string spiceponumber;
            string status;
            short changesequence;

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

        private void SetupGrid ()
        {
            List<String> columnsvisible = new List<string>();
            string sexpression = "clmVendor + ' - ' + clmVendorName";
            DataGridViewImageColumn dgvicpoupdate = new DataGridViewImageColumn();
            DataGridViewComboBoxColumn dgvcoboxreasoncode = new DataGridViewComboBoxColumn();
            DataGridViewTextBoxColumn dgvtxtreasoncode = new DataGridViewTextBoxColumn ();

            // Add compute columns
            _dtGridDataTable.Columns.Add("VendorCodeAndName", typeof(string));
            _dtGridDataTable.Columns["VendorCodeAndName"].Expression = sexpression;

            columnsvisible.Add("clmSpicePO");
            columnsvisible.Add("clmRevisionNumber");
            columnsvisible.Add("clmIPPO");
            columnsvisible.Add("VendorCodeAndName");
            columnsvisible.Add("ReasonCode");

            dgvCancelPO.DataSource = _dtGridDataTable;

            // Header Text
            dgvCancelPO.Columns["clmSpicePO"].HeaderText = "Spice Po #";
            dgvCancelPO.Columns["clmRevisionNumber"].HeaderText = "Revision #";
            dgvCancelPO.Columns["clmIPPO"].HeaderText = "IP PO #";
            dgvCancelPO.Columns["VendorCodeAndName"].HeaderText = "Vendor Code - Name";

            // Width
            dgvCancelPO.Columns["clmSpicePO"].Width = 90; ;
            dgvCancelPO.Columns["clmRevisionNumber"].Width = 90;
            dgvCancelPO.Columns["clmIPPO"].Width = 80;
            dgvCancelPO.Columns["VendorCodeAndName"].Width = 250;

            // Visible
            foreach (DataColumn dc in _dtData.Columns)
            {
                if (!columnsvisible.Contains(dc.ColumnName))
                {
                    dgvCancelPO.Columns[dc.ColumnName].Visible = false;
                }
            }

            // New Display only columns in Datagridview
            dgvCancelPO.Columns.Add("POStatusDesc", "PO Status");

            dgvicpoupdate.Name = "POUpdate";
            dgvicpoupdate.HeaderText = "PO Update";
            dgvCancelPO.Columns.Add(dgvicpoupdate);

            // Reason Code is now a standard cell and not a dropdown
            dgvtxtreasoncode.Name = "ReasonCode";
            dgvtxtreasoncode.HeaderText = "Reason Code";
            dgvtxtreasoncode.MaxInputLength = 30;
            dgvCancelPO.Columns.Add(dgvtxtreasoncode);

            //ReadOnly
            dgvCancelPO.Columns["clmSpicePO"].ReadOnly        = true;
            dgvCancelPO.Columns["clmRevisionNumber"].ReadOnly = true;
            dgvCancelPO.Columns["clmIPPO"].ReadOnly           = true;
            dgvCancelPO.Columns["VendorCodeAndName"].ReadOnly = true;
            dgvCancelPO.Columns["POStatusDesc"].ReadOnly      = true;
        }

        private void dgvCancelPO_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == dgvCancelPO.Columns["POUpdate"].Index)
            {
                if (imageList != null)
                {
                    string s = dgvCancelPO["clmStatus", e.RowIndex].Value.ToString();
                    switch (s)
                    {
                        case "OP":
                            e.Value = imageList.Images[2];  // Green
                            break;

                        case "OS":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        case "RS":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        case "CS":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        case "OC":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        case "RC":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        case "IC":
                            e.Value = imageList.Images[0];  // Red
                            break;

                        default:
                            e.Value = imageList.Images[1];  // Amber
                            break;
                    }
                }
            }

            if (e.RowIndex > -1 && e.ColumnIndex == dgvCancelPO.Columns["POStatusDesc"].Index)
            {
                string s = dgvCancelPO["clmStatus", e.RowIndex].Value.ToString();
                e.Value = GetPOStatusDescription(s);
            }
        }

        private string GetPOStatusDescription(string postatus)
        {
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

        private void frmCancelPO_Load(object sender, EventArgs e)
        {
            EnableDisableReasonCode();
        }
    }
}
