using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace Disney.Spice.EASUI
{
    public partial class frmPoModificationSummary : Form
    {

        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private string _spiceponumber;
        private short _spicepoversion;
        private DataTable _dtData;
        private Form _mdiparent;
        private DataTable _dtGridDataTable;
        private DataSet _dsSummary;


        public frmPoModificationSummary()
        {
            InitializeComponent();
        }

        public frmPoModificationSummary(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1, string spiceponumber, short spicepoversion)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;
            _mdiparent = Form1;

            _spiceponumber = spiceponumber;
            _spicepoversion = spicepoversion;

            this.Text = this.Text + " PO " + _spiceponumber + " / " + _spicepoversion.ToString();

            SetupDataTable();

            SetupGrid();

            // hourglass cursor
            Cursor.Current = Cursors.Default;
        }

        private void SetupDataTable()
        {
            //DataSet dsSummary;
            Disney.DA.EAS.DSSPPOM dsspom = new Disney.DA.EAS.DSSPPOM(_dbparamref, _username);

            _dsSummary = dsspom.GetSummaryDataSet(_spiceponumber, _spicepoversion);

        }

        private void SetupGrid()
        {

            List<String> columnsvisible = new List<string>();
            columnsvisible.Add("Details");

            // Set the datasource
            dgvPoHeader.DataSource = _dsSummary.Tables[0];
            dgvPOLines.DataSource = _dsSummary.Tables[1];

            // Header Text
            dgvPoHeader.Columns["Details"].HeaderText = "Changes";
            dgvPOLines.Columns["Details"].HeaderText = "Changes";

            // HK : CJ: 21-12-2009 : Use AutoFill for column(s)
            // Width
            //dgvPoHeader.Columns["Details"].Width =  670;
            //dgvPOLines.Columns["Details"].Width = 670;

            // Visible
            foreach (DataColumn dc in _dsSummary.Tables[0].Columns)
            {
                if (!columnsvisible.Contains(dc.ColumnName))
                {
                    dgvPoHeader.Columns[dc.ColumnName].Visible = false;
                }
            }

            foreach (DataColumn dc in _dsSummary.Tables[1].Columns)
            {
                if (!columnsvisible.Contains(dc.ColumnName))
                {
                    dgvPOLines.Columns[dc.ColumnName].Visible = false;
                }
            }

            // AutoFill column(s)
            dgvPoHeader.Columns["Details"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvPOLines.Columns["Details"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            

        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");                     
        }
    }
}