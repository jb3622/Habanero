using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Disney.Spice.POBO;
using System.Diagnostics;

namespace Disney.Spice.POUI
{
    public partial class frmDropShipMatrix : Form
    {

        public delegate void    OkButtonClickedEventHandler(object sender, DropShipMatrixEventArgs e);
        public event            OkButtonClickedEventHandler OnOkButtonClicked;

        public delegate void    CancelButtonClickedEventHandler(object sender, DropShipMatrixEventArgs e);
        public event            CancelButtonClickedEventHandler OnCancelButtonClicked;

        private DataTable       _dtDropShipMatrix;
        private DataTable       _dtStores;
        private PurchaseOrder   _porder;

        private string          _sStoreColumnNamePrefix = "Store_";
        //private int           _icolumnsFixedInDropShipMatrixDataTable = 8;
        private int             _icolumnsFixedInDropShipMatrixDataTable = 10;

        Boolean bOkToValidateQuantityEntered = true;
        Boolean bStoreQuantityOk = false;
        
        // Store column list        
        List<string> columnStoreValues = new List<string>();

        // To Do
        // List of Store Id (clmStore)

        // List of Store Name (clmStoreNAme)

        private int _itemquantityrounded = 0;

        private Boolean _bFormCancelClicked = false;

        // Default Constructor
        public frmDropShipMatrix()
        {
            InitializeComponent();
        }

        public frmDropShipMatrix(PurchaseOrder porder, DataTable dtDropShipMatrix, DataTable dtSelectedStores)
        {
            
            InitializeComponent();

            _dtDropShipMatrix   = dtDropShipMatrix;
            _dtStores           = dtSelectedStores;
            _porder             = porder;

            // Do not allow datagrid validation as we are initalising the objects
            bOkToValidateQuantityEntered = false;

            SetupDropShipMatrixDataTable();

            SetupDropShipMatrixGrid();

            // Do not allow datagrid validation as we are initalising the objects
            bOkToValidateQuantityEntered = true;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            _bFormCancelClicked = true;

            // Prepare to close and raise relavant events
            DropShipMatrixEventArgs e1 = new DropShipMatrixEventArgs(_dtDropShipMatrix);
            this.DialogResult = DialogResult.Cancel;

            // HK : 04-12-2009
            DeleteStoresToDropShipMatrixdataTable();
            
            Close();
        }

        private void SetupDropShipMatrixGrid()
        {
            // Header Text
            dgvDropShipMatrix.Columns["Class"].HeaderText = "Class";
            dgvDropShipMatrix.Columns["Vendor"].HeaderText = "Vendor";
            dgvDropShipMatrix.Columns["Style"].HeaderText = "Style";
            dgvDropShipMatrix.Columns["Color"].HeaderText = "Color";
            dgvDropShipMatrix.Columns["Size"].HeaderText = "Size";
            dgvDropShipMatrix.Columns["Description"].HeaderText = "Description";
            dgvDropShipMatrix.Columns["Quantity"].HeaderText = "Total \n\rQty";
            dgvDropShipMatrix.Columns["QuantityAssigned"].HeaderText = "Qty \n\rAssigned";
            
            // Width
            dgvDropShipMatrix.Columns["Class"].Width = 50;
            dgvDropShipMatrix.Columns["Vendor"].Width = 50;
            dgvDropShipMatrix.Columns["Style"].Width = 50;
            dgvDropShipMatrix.Columns["Color"].Width = 50;
            dgvDropShipMatrix.Columns["Size"].Width = 50;
            dgvDropShipMatrix.Columns["Description"].Width = 160;
            dgvDropShipMatrix.Columns["Quantity"].Width = 50;
            dgvDropShipMatrix.Columns["QuantityAssigned"].Width = 60;

            // Readonly
            dgvDropShipMatrix.Columns["Class"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Vendor"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Style"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Color"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Size"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Description"].ReadOnly = true;
            dgvDropShipMatrix.Columns["Quantity"].ReadOnly = true;
            dgvDropShipMatrix.Columns["QuantityAssigned"].ReadOnly = true;

            // Visible 
            // HK : 28-11-2009 : Add ItemIndex
            dgvDropShipMatrix.Columns["ItemIndex"].Visible = false;
            dgvDropShipMatrix.Columns["Pack"].Visible = false;
            // HK : 14-01-2010 : Show the Class, vendor, Styl;e, Color, Size columns
            // so that the Description gets more meaning.
            //dgvDropShipMatrix.Columns["Class"].Visible = false;
            //dgvDropShipMatrix.Columns["Vendor"].Visible = false;
            //dgvDropShipMatrix.Columns["Style"].Visible = false;
            //dgvDropShipMatrix.Columns["Color"].Visible = false;
            //dgvDropShipMatrix.Columns["Size"].Visible = false;

            // Backgroung color of cell
            dgvDropShipMatrix.Columns["Class"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Vendor"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Style"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Color"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Size"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Description"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["Quantity"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dgvDropShipMatrix.Columns["QuantityAssigned"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;

            // Column Header Alignment
            //dgvDropShipMatrix.Columns["Description"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Column Alignment
            dgvDropShipMatrix.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDropShipMatrix.Columns["QuantityAssigned"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Columns frozen
            //dgvDropShipMatrix.Columns["Description"].Frozen = true;
            //dgvDropShipMatrix.Columns["Quantity"].Frozen = true;
            //dgvDropShipMatrix.Columns["QuantityAssigned"].Frozen = true;

            // /////////////////////////////////////////////////////////
            // Store columns dynamically created
            SetStoreColumnHeader();

        }

        private void SetStoreColumnHeader()
        {

            string sStore;
            string sStoreName;

            for (int i = 0; i < columnStoreValues.Count; i++)
            {
                sStore = (string)_dtStores.Rows[i]["clmStore"];
                sStoreName = (string)_dtStores.Rows[i]["clmStoreName"];

                dgvDropShipMatrix.Columns[columnStoreValues[i]].HeaderText = sStore + " - \n\r" + sStoreName;
                dgvDropShipMatrix.Columns[columnStoreValues[i]].Width = 75;

                dgvDropShipMatrix.Columns[columnStoreValues[i]].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
        }

        // Calculate the quantity assigned
        // HK : 28-11-2009 : If the quantity assigned is > then total 
        // quantity then display the quantity assigned in red.
        // This situation could happen if the user went to the main
        // PO Entry form and changed the quantity on a line item and 
        // then came back in the Drop Ship Materix PO form.

        private int CalculateQuantityAssigned()
        {
            int iSummedQuantity = 0;
            int iQuantity       = 0;
            int iTemp;

            for (int i = 0; i < _dtDropShipMatrix.Rows.Count; i++)
            {
                // 
                iSummedQuantity = 0;
                iQuantity       = Convert.ToInt32 (_dtDropShipMatrix.Rows[i]["Quantity"]);
                
                for (int j = _icolumnsFixedInDropShipMatrixDataTable; j < _dtDropShipMatrix.Columns.Count; j++)
                {
                    iTemp = Convert.ToInt32(_dtDropShipMatrix.Rows[i][j]);

                    iSummedQuantity = iSummedQuantity + iTemp;

                    _dtDropShipMatrix.Rows[i]["QuantityAssigned"] = iSummedQuantity;

                    // If the quantity assigned is > then total 
                    // quantity then display the quantity assigned in red.

                }

                /*
                if (iSummedQuantity > iQuantity)
                {
                    dgvDropShipMatrix["QuantityAssigned", i].Style.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    dgvDropShipMatrix["QuantityAssigned", i].Style.BackColor = System.Drawing.SystemColors.Control;
                    //dgvDropShipMatrix.Columns["QuantityAssigned"].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
                }
                */

            }

            return iSummedQuantity;
        }

        private void InitStoreQuantity(int iInitValue, int iRowIndex)
        {
            // At this stage the store columns have been added to the 
            // drop ship matrid po datatable
            // So initalise the values to 0

            int istorecolumnsount       = columnStoreValues.Count;
            int itotalcolumncount       = _dtDropShipMatrix.Columns.Count;
            int itotalstorecolumnadded  = _dtStores.Rows.Count;

            for (int j = 0; j < itotalstorecolumnadded; j++)
            {
                _dtDropShipMatrix.Rows[iRowIndex][_icolumnsFixedInDropShipMatrixDataTable + j] = iInitValue.ToString();

            }

        }

        private void InitStoreQuantity(int iInitValue)
        {
            // At this stage the store columns have been added to the 
            // drop ship matrid po datatable
            // So initalise the values to 0

            int istorecolumnsount = columnStoreValues.Count;
            int itotalcolumncount = _dtDropShipMatrix.Columns.Count;
            int itotalstorecolumnadded = _dtStores.Rows.Count;

            for (int i = 0; i < _dtDropShipMatrix.Rows.Count; i++)
            {
                for (int j = 0; j < itotalstorecolumnadded; j++)
                {
                    _dtDropShipMatrix.Rows[i][_icolumnsFixedInDropShipMatrixDataTable + j] = iInitValue.ToString();

                }
            }

        }

        private void InitStoreQuantity(int iInitValue, string sStoreColumnName)
        {

            for (int i = 0; i < _dtDropShipMatrix.Rows.Count; i++)
            {
                _dtDropShipMatrix.Rows[i][sStoreColumnName] = iInitValue;
            }
        }

        private void RefreshStore()
        {
            // This should be called when _dtDropShipMatrix has already been populated
            // on prior open of this form 

            string  sStore;
            string  sStoreName;
            int     iTotalStoreColumnsinDropShipMatrixDataTable;

            iTotalStoreColumnsinDropShipMatrixDataTable = _dtDropShipMatrix.Columns.Count - _icolumnsFixedInDropShipMatrixDataTable;
            
            // The newly selected stores (if any by the user) are in the dtStores datatable
            // So remove any stores the user has deselected
            // The datatable _dtDropShipMatrix will have the stores selectd by the 
            // user on the previous run

            // To remove de selected stores we must loop from the _dtDropShipMatrix datatable
            // and check that the stores in _dtDropShipMatrix exists in the _dtStores datatable.
            // _dtStores will always have the recently selected by the user
            for (int i = _icolumnsFixedInDropShipMatrixDataTable; i < _dtDropShipMatrix.Columns.Count;  )
            {
                sStore = _dtDropShipMatrix.Columns[i].ToString();

                // Remove the Store column default prefix from the column name to 
                // get actual datatable column name; 
                StringBuilder sbStoreColumnName = new StringBuilder(sStore);

                // HK : To do remove the 6 below to Length _sStoreColumnNamePrefix
                sbStoreColumnName.Remove(0, 6);
                if (_dtStores.Select("clmStore = '" + sbStoreColumnName.ToString() + "'").Length == 0)
                {
                    _dtDropShipMatrix.Columns.Remove(sStore);

                }
                else
                {
                    // Add this column to the store column list 'columnStoreValues'
                    columnStoreValues.Add(sStore);

                    i++;
                }
            }

            // Now Add any new stores selected by the user

            // To Add the newly selected stores we must loop through the 
            // _dtStores datatable and add the store column to _dtDropShipMatrix
            // if one does not exists
            for (int i = 0; i < _dtStores.Rows.Count; i ++)
            {
                sStore      = (string)_dtStores.Rows[i]["clmStore"].ToString ();
                sStoreName  = (string)_dtStores.Rows[i]["clmStoreName"].ToString ();
                sStore      = _sStoreColumnNamePrefix + sStore;

                if (!_dtDropShipMatrix.Columns.Contains(sStore))
                {
                    _dtDropShipMatrix.Columns.Add(sStore);

                    // Add this column to the store column list 'columnStoreValues'
                    columnStoreValues.Add(sStore);

                    // If a store is added then we must initalise the store 
                    // quantity to 0 for all items in _dtDropShipMatrix

                    InitStoreQuantity(0, sStore);
                }
            }

        }

        
        // ///////////////////////////////////////////////////////////////
        // HK : 27-11-2009
        // Should implement a IEnumerable<POItemDetails> on class PurchaseOrder 
        // enummerate through List<POItemDetails> _lstpoLineItemDetails;
        // But for the time being we implement a for loop
        private int FindPoItem (int iitemindex, short iClass, int iVendor, short iStyle, short iColor, short iSize, ref int iQuantity)
        {
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                // HK : 28-11-2009 : Add ItemIndex
                if ((_porder.lstpoLineItemDetails[i].Itemindex == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].Classcode  == iClass ) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode  == iStyle ) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode  == iColor ) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize   == iSize  ))
                
                {
                    iQuantity = _porder.lstpoLineItemDetails[i].Itemquantity;
                    return i;
                }

            }

            iQuantity = 0;
            return -1;

        }

        private void RefreshItems()
        {
            // First remove any items(s) the user has removed / deleted
            // on the main PO Entry form

            // To do this loop through the dtDropShipMatrix and see 
            // if the itmes in dtDropShipMatrix still exists in the 
            // PO Items collection

            int     iitemindex;
            short   iClass;
            int     iVendor;
            short   iStyle;
            short   iColor;
            short   iSize;
            int     iQuantity       = 0;

            string sselectexpr;

            for (int i = 0; i < _dtDropShipMatrix.Rows.Count; )
            {
                // HK : 28-11-2009 : Add ItemIndex
                iitemindex  = Convert.ToInt16 (_dtDropShipMatrix.Rows[i]["ItemIndex"]);
                iClass      = Convert.ToInt16 (_dtDropShipMatrix.Rows[i]["Class"]);
                iVendor     = Convert.ToInt32 (_dtDropShipMatrix.Rows[i]["Vendor"]);
                iStyle      = Convert.ToInt16 (_dtDropShipMatrix.Rows[i]["Style"]);
                iColor      = Convert.ToInt16 (_dtDropShipMatrix.Rows[i]["Color"]);
                iSize       = Convert.ToInt16 (_dtDropShipMatrix.Rows[i]["Size"]);

                // Item not found, so delete the record
                if (FindPoItem(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref iQuantity) == -1)
                {
                    _dtDropShipMatrix.Rows[i].Delete();

                }

                else
                {
                    // Check to see if the existing PO Items's Quantity has changed.
                    // If it has channged then we must reflect it in DropShip 
                    // Matrix datatable (_dtDropShipMatrix)
                    if (Convert.ToInt32 (_dtDropShipMatrix.Rows[i]["Quantity"]) != iQuantity)
                    {
                        _dtDropShipMatrix.Rows[i]["Quantity"] = iQuantity;
                    }

                    // Increment the counter and to check the next counter
                    i++;
                }

            }

            // Now we add any new items that the user has added
            // For this we loop through the _porder.lstpoLineItemDetails
            // list collection. If an item is not found in the 
            // datatable _dtDropShipMatrix it must be newly 
            // added by the user
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                // HK : 28-11-2009 : Add ItemIndex
                iitemindex  = _porder.lstpoLineItemDetails[i].Itemindex;
                iClass      = _porder.lstpoLineItemDetails[i].Classcode;
                iVendor     = _porder.lstpoLineItemDetails[i].Vendorcode;
                iStyle      = _porder.lstpoLineItemDetails[i].Stylecode;
                iColor      = _porder.lstpoLineItemDetails[i].Colorcode;
                iSize       = _porder.lstpoLineItemDetails[i].Itemsize;

                sselectexpr = "ItemIndex = '"   + iitemindex.ToString() + "' and " +
                              "Class = '"       + iClass.ToString()   + "' and " +
                              "Vendor = '"      + iVendor.ToString()  + "' and " +
                              "Style = '"       + iStyle.ToString()   + "' and " +
                              "Color = '"       + iColor.ToString()   + "' and " +
                              "Size = '"        + iSize.ToString()    + "'";

                // PO Item not found. So it must be newly added by user
                if (_dtDropShipMatrix.Select(sselectexpr).Length == 0)
                {
                    // HK : 28-11-2009 : Add ItemIndex
                    _dtDropShipMatrix.Rows.Add(_porder.lstpoLineItemDetails[i].Itemindex,
                        // 11-01-2010 : Added APP1
                            _porder.lstpoLineItemDetails[i].APP1,
                            _porder.lstpoLineItemDetails[i].Classcode,
                            _porder.lstpoLineItemDetails[i].Vendorcode,
                            _porder.lstpoLineItemDetails[i].Stylecode,
                            _porder.lstpoLineItemDetails[i].Colorcode,
                            _porder.lstpoLineItemDetails[i].Itemsize,
                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                            _porder.lstpoLineItemDetails[i].Itemquantity);

                    // Initalise the store columns to 0, otherwise CalculateQuantity 
                    // will throw a exception
                    InitStoreQuantity(0, i);


                }

            }

        }

        private void SetupDropShipMatrixDataTable()
        {

            // First time round
            // HK : 04-12-2009 : If no items exists on the PO Entry form
            // then this will crash. S

            // Add store columns first time round only.
            // RefreshItems() should take care of adding any itmes 
            // added after the first run.

            //AddStoresToDropShipMatrixdataTable();

            if (_dtDropShipMatrix.Rows.Count == 0)
            {
                // HK : 04-12-2009 : Now moved out. See above
                // Add store columns. 
                AddStoresToDropShipMatrixdataTable();

                // Add All PO Line Items
                AddItemsToDropShipMatrixDataTable();

                // Init store quantity to 0
                InitStoreQuantity(0);

                // Set the datasource of the display grid
                dgvDropShipMatrix.DataSource = _dtDropShipMatrix;

            }

            // Subsequent times round
            else
            {
                // Refresh the stores
                // Remove / Add any new stores
                RefreshStore();

                // Refresh the items
                // Remove / Add any deleted stores
                RefreshItems();


                // Assign the binding source
                dgvDropShipMatrix.DataSource = _dtDropShipMatrix;
            }

            CalculateQuantityAssigned();

        }

        private Boolean ValidateQuantityAssigned()
        {
            // If the the 'Quantity Total' equals 'Quantity Assigned'.
            // then warn the user

            for (int i = 0; i < _dtDropShipMatrix.Rows.Count; i++)
            {
                if (Convert.ToInt32 (_dtDropShipMatrix.Rows [i]["QuantityAssigned"]) != 
                    Convert.ToInt32 (_dtDropShipMatrix.Rows [i]["Quantity"]))
                {
                    return false;
                }
            }

            return true;
        }

        private void AddItemsToDropShipMatrixDataTable()
        {
            //DataRow dr;
            //dr = new DataRow ();
            //dr["Class"] = _porder.lstpoLineItemDetails[count].Classcode

            if (_porder.lstpoLineItemDetails.Count > 0)
            {
                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {
                    // 28-11-2009 : Add ItemIndex
                    _dtDropShipMatrix.Rows.Add( _porder.lstpoLineItemDetails[i].Itemindex,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].Classcode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                _porder.lstpoLineItemDetails[i].Itemquantity);
                }

            }
        }


        private void DeleteStoresToDropShipMatrixdataTable()
        {
            // If the user has not added aby items on the main PO Entry  then
            // delete the columns.
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                foreach (DataRow dr in _dtStores.Rows)
                {
                    string columnXTemp = _sStoreColumnNamePrefix + dr["clmStore"].ToString();
                    _dtDropShipMatrix.Columns.Remove (columnXTemp);

                }
            }
        }

        private void AddStoresToDropShipMatrixdataTable()
        {

            foreach (DataRow dr in _dtStores.Rows)
            {

                string columnXTemp = _sStoreColumnNamePrefix + dr["clmStore"].ToString();
                if (!columnStoreValues.Contains(columnXTemp))
                {
                    //Read each row value, if it's different from others provided, add to the list of values and creates a new Column with its value.
                    //columnStoreValues.Add(columnXTemp);

                    // Add the store column only if it does not exists.
                    // The issue is, when user has no items on the main Po Entry form and 
                    // and clicks drop ship matrix, selects some stores and goes into the 
                    // Drop Ship Matrix form, this method (AddStoresToDropShipMatrixdataTable) 
                    // will add the store column irrespectively of wtehter it exists or not.
                    // If on subsequent loading of the Drop Ship Matrix form the user has 
                    // some items in the main PO Entry form this method will add the same column 
                    // again and crash
                    //if (_dtDropShipMatrix.Columns.Contains(columnXTemp) == false)
                    //{
                        columnStoreValues.Add(columnXTemp);
                        _dtDropShipMatrix.Columns.Add(columnXTemp);
                    //}
                    /*
                    else
                    {
                        columnStoreValues.Remove(columnXTemp);
                        _dtDropShipMatrix.Columns.Remove(columnXTemp);
                    }
                    */ 
                }
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // ////////////////////////////////////////////////////////
            // Validate the Quantiy Assigned against Total Quantity and 
            // warn the user appropriately.

            if (!ValidateQuantityAssigned())
            {
                DialogResult dlgRslt;
                
                dlgRslt = MessageBox.Show ("Quantity Assigned to store(s) do not match. \n\r\n\r" +
                                           "You will not be able to create this PO\n\r\n\r" +
                                           "Do you wish to continue?", "PO Entry",
                                           MessageBoxButtons.YesNo, 
                                           MessageBoxIcon.Warning, 
                                           MessageBoxDefaultButton.Button1);
                
                if (dlgRslt == DialogResult.No)
                {
                    return;
                }
                else
                {
                    // ////////////////////////////////////////////////////////

                    // Prepare to close and raise relavant events
                    DropShipMatrixEventArgs e1 = new DropShipMatrixEventArgs(_dtDropShipMatrix);
                    this.DialogResult = DialogResult.OK;
                    RaiseCancelButtonClickedEvent(e1);
                    Close();
                }

            }
            else
            {
                // ////////////////////////////////////////////////////////

                // Prepare to close and raise relavant events
                DropShipMatrixEventArgs e1 = new DropShipMatrixEventArgs(_dtDropShipMatrix);
                this.DialogResult = DialogResult.OK;
                RaiseCancelButtonClickedEvent(e1);

                DeleteStoresToDropShipMatrixdataTable();

                Close();
            }

        }

        public class DropShipMatrixEventArgs : EventArgs
        {

            private DataTable _dtdropdhipmatrix = null;

            public DataTable dtdropdhipmatrix
            {
                get
                {
                    return _dtdropdhipmatrix;
                }

                set
                {
                    _dtdropdhipmatrix = value;
                }

            }

            public DropShipMatrixEventArgs(DataTable dt)
            {
                dtdropdhipmatrix = dt;
            }

        }

        private void RaiseOkButtonClickedEvent(DropShipMatrixEventArgs e)
        {

            if (OnOkButtonClicked != null)
            {
                OnOkButtonClicked(this, e);
            }
        }

        private void RaiseCancelButtonClickedEvent(DropShipMatrixEventArgs e)
        {
            if (OnCancelButtonClicked != null)
            {
                OnCancelButtonClicked(this, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Always Invert on the store column

            dgvDropShipMatrix["QuantityAssigned", 0].Style.BackColor = System.Drawing.Color.Red;

            /*

            DataTable returnTable = new DataTable();
            List<string> columnStoreValues = new List<string>();

            foreach (DataRow dr in _dtDropShipMatrix.Rows)
            {

                string columnXTemp = dr["clmStore"].ToString();
                if (!columnStoreValues.Contains(columnXTemp))
                {
                    //Read each row value, if it's different from others provided, add to the list of values and creates a new Column with its value.
                    columnStoreValues.Add(columnXTemp);
                    returnTable.Columns.Add(columnXTemp);
                }
            }

            dgvDropShipMatrix.DataSource = returnTable;
            */
        }

        private int GetSummedQuantity(string sColumnName, int rowindex)
        {
            int iTemp = 0;
            int iSummedQuantity = 0;
            string sStoreColumn;
            
            for (int i = 0; i < columnStoreValues.Count; i++)
            {
                // Get the Store column name
                sStoreColumn = columnStoreValues[i];

                // If it is not the one we are validating then
                // sum up the value
                if (!sStoreColumn.Equals(sColumnName))
                {
                    Debug.Print("Processing quantity for store: " + sStoreColumn);

                    iTemp = Convert.ToInt32(_dtDropShipMatrix.Rows[rowindex][columnStoreValues[i]]);
                    Debug.Print("Quantity for store: " + sStoreColumn + ": " + iTemp.ToString());

                    iSummedQuantity = iSummedQuantity + iTemp;
                }
                else
                {
                    Debug.Print("Skipping processing quantity for current store: " + sStoreColumn);
                }
            }

            return iSummedQuantity;
        }

        private void dgvDropShipMatrix_CellValidatingWithRounding(object sender, DataGridViewCellValidatingEventArgs e)
        {

            string      sColumnName;
            string      sStoreColumn;
            int         iSummedQuantity         = 0;
            int         iTemp;
            int         iquantityalreadrentered;
            int         iQuantity;
            string      squantityentered;
            int         iquantityentered;
            string      squantityincell;
            int         iquantityincell;
            int         iquantityassigned;
            
            // Validation against case pack quantity
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            short icasepackqty = 0;

            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                Debug.Print("Cell Validating Cancelled as user hit Cancel button");
                return;
            }

            if (bOkToValidateQuantityEntered)
            {
                // Get the column name the grid is trying to validate
                sColumnName             = dgvDropShipMatrix.Columns[e.ColumnIndex].Name;

                // If it is a valid store column
                if (columnStoreValues.Contains(sColumnName))
                {
                    Debug.Print("Drop Ship Matrix column name being validate: " + sColumnName);
                    
                    // Get the quantity the user is trying to enter
                    squantityentered        = e.FormattedValue.ToString();
                    // Get the quantity already in the cell into which user is entering a new quantity
                    squantityincell         = Convert.ToString (_dtDropShipMatrix.Rows[e.RowIndex][sColumnName]);

                    // Try and convert the quantity entered into a integer
                    // If not then throw a error
                    
                    if (!String.IsNullOrEmpty(squantityentered) && Int32.TryParse(squantityentered, out iquantityentered))
                    {

                        // Get the total quantity on this item
                        iQuantity = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex]["Quantity"]);
                        Debug.Print("Total Quantity: " + iQuantity);

                        // //////////////////////////////////////////////////////////////////////
                        // HK : 4-12-2009 : If validation fails then comment the below block.
                        // We need to validate this quantity entered against the cas pack qty

                        iitemindex = Convert.ToInt16(dgvDropShipMatrix.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                        iClass = Convert.ToInt16(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Class"].Value);
                        iVendor = Convert.ToInt32(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Vendor"].Value);
                        iStyle = Convert.ToInt16(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Style"].Value);
                        iColor = Convert.ToInt16(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Color"].Value);
                        iSize = Convert.ToInt16(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Size"].Value);

                        GetItemCasePackQty (iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                        // HK : 08-12-2009 : Since 0 is a valid quantity, we must not 
                        // open the quantity rounding form.
                        
                        if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                        {
                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                            e.Cancel = false;

                            // HK : 09-12-2009 : If user selected a rounded value and clicked OK in 
                            // the quantity rounded form ItemQuantityForm, this rounded quantity 
                            // can be rejected by the following validation. In other words even 
                            // though the rounded quantiy is valid as far as the rounding requirements 
                            // go, they may not be valid as far as the summing of the quantities 
                            // across the stores go.

                        }
                        else
                        {
                            Debug.Print("Case Pack Quantity is invalid. ");
                            // HK : 15-12-2009 : Hint the case pack qty to the user
                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Please enter a valid Case Pack Quantity. Distro for this item is : " + Convert.ToString(icasepackqty); ;
                            e.Cancel = true;

                            return;
                        }
                        

                        // At the point the quantity entered is valid against the case pack quantity.
                        // Hence the next level of validation to check this quantity entered can be 
                        // can be done.

                        // //////////////////////////////////////////////////////////////////////
                        
                        // Quantity entered as string
                        squantityentered = e.FormattedValue.ToString();
                        Debug.Print("Quantity Entered: " + squantityentered);

                        // Quantity already entered as integer
                        iquantityalreadrentered = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex][e.ColumnIndex]);
                        Debug.Print("Quantity entered by user: " + iquantityalreadrentered);

                        // Sum the ammount across all columns on the row EXCEPT the current column for which the 
                        // value is being changed. i.e EXCEPT for the current column being validated
                        for (int i = 0; i < columnStoreValues.Count; i++)
                        {
                            // Get the Store column name
                            sStoreColumn = columnStoreValues[i];

                            // If it is not the one we are validating then
                            // sum up the value
                            if (!sStoreColumn.Equals(sColumnName))
                            {
                                Debug.Print("Processing quantity for store: " + sStoreColumn);
                                
                                iTemp = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex][columnStoreValues[i]]);
                                Debug.Print("Quantity for store: " + sStoreColumn + ": " + iTemp.ToString());
                                
                                iSummedQuantity = iSummedQuantity + iTemp;
                            }
                            else
                            {
                                Debug.Print("Skipping processing quantity for current store: " + sStoreColumn);
                            }
                        }

                        Debug.Print("Quantity summed across all stores except current store: " + iSummedQuantity.ToString());

                        //if (iSummedQuantity + Convert.ToInt32(squantityentered) <= iQuantity)

                        // If summed quantity + quantity entered <= total quantity 
                        // then our validation is ok
                        
                        // HK : 09-12-2009 : If the user has selected any suggested rounded quantity
                        // then the entered quantity is the rounded quantity and not the 
                        // quantity as typed by user in the cell.
                        if (_itemquantityrounded > 0)
                        {
                            squantityentered = _itemquantityrounded.ToString();
                        }
                        
                        if (iSummedQuantity + Convert.ToInt32(squantityentered) <= iQuantity)
                        {
                            Debug.Print("Validatiion successful: ");
                            Debug.Print("Summed item quantity: " + iSummedQuantity + " + " + "Quantity entered:" + squantityentered + " LESS THAN or EQUALS " + "Total Quantity" + iQuantity.ToString());

                            
                            _dtDropShipMatrix.Rows[e.RowIndex][sColumnName] = Convert.ToInt32(squantityentered);
                            //_dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"] = (iSummedQuantity) + Convert.ToInt32(squantityentered);
                            dgvDropShipMatrix.Rows[e.RowIndex].Cells["QuantityAssigned"].Value = (iSummedQuantity) + Convert.ToInt32(squantityentered);

                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                            e.Cancel = false;

                            // HK : 09-12-2009 : If user selected a rounded value and clicked OK in 
                            // the quantity rounded form ItemQuantityForm, this rounded quantity 
                            // can be rejected by the following validation. In other words even 
                            // though the rounded quantiy is valid as far as the rounding requirements 
                            // go, they may not be valid as far as the summing of the quantities 
                            // across the stores go.
                            // This quantity has been rounded and is validate as Ok.
                            if (_itemquantityrounded > 0)
                            {
                                bStoreQuantityOk = true;
                            }
                        }
                        else
                        {
                            Debug.Print("Validatiion failed: ");
                            Debug.Print("Summed item quantity: " + iSummedQuantity + " + " + "Quantity entered:" + squantityentered + " NOT EQUALS " + "Total Quantity" + iQuantity.ToString());
                            // Validation fails
                            // HK : 15-12-2009 : Fix Bug : 136
                            // Rephrase the error message to make the meaning more clear to usr.
                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Total Quantity Assigned to all stores should be less than or equal to Total quantity for item";
                            e.Cancel = true;

                            // HK : 9-12-2009
                            // If the Quantity rounding form popped up it will have set 
                            // the rounded quantity selected by user. This must be reset 
                            // back to 0 as validation has failed. Otherwise once the 
                            // validating event completes the validated event will pick 
                            // up the rounded quantity and cause summing problems
                            _itemquantityrounded = 0;

                        }
                    }

                    else
                    {
                        Debug.Print("Validatiion failed: ");
                        // HK : 15-12-2009 : Fix Bug : 136
                        // Rephrase the error message to make the meaning more clear to usr.
                        dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Total Quantity Assigned to all stores should be less than or equal to Total quantity for item";
                        e.Cancel = true;
                    }

                }

            }
            
        }

        private void dgvDropShipMatrix_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int iquantity;
            int iquantityqssigned;

            // //////////////////////////////////////////////////////////////////////////////
            // HK : 30-11-2009 : Check the column index. if column index = -1 then dot format
            // //////////////////////////////////////////////////////////////////////////////
            if (!(e.ColumnIndex == -1))
            {
                // HK : 14-01-2010 : This code block will never get executed as the validation 
                // now in place will ensure that QuantityAssigned can never be greater than 
                // Total Quantity assignable across stores.
                // Handle the formating for the column "QuantityAssigned"
                if (!dgvDropShipMatrix.Rows[e.RowIndex].IsNewRow && dgvDropShipMatrix.Columns[e.ColumnIndex].Name == "QuantityAssigned")
                {
                    iquantity = Convert.ToInt32(dgvDropShipMatrix.Rows[e.RowIndex].Cells["Quantity"].Value);
                    iquantityqssigned = Convert.ToInt32(e.Value.ToString());

                    if (e.Value != null && (iquantityqssigned > iquantity))
                    {

                        //_cellbackgroundcolor = e.CellStyle.BackColor;
                        e.CellStyle.BackColor = System.Drawing.Color.Red;

                        // Uncommment below if font style needs to be changed

                        //Font newFont = new Font(dtgrdPOLinesView.Font, FontStyle.Strikeout);
                        //dtgrdPOLinesView[e.ColumnIndex, e.RowIndex].Style.Font = newFont;
                    }

                }
            }

        }

        private void dgvDropShipMatrix_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int iquantity;
            int iquantityassigned;
            string sColumnName;
            
            // Get the column name the grid is trying to validate
            sColumnName             = dgvDropShipMatrix.Columns[e.ColumnIndex].Name;

            // If it is a valid store column
            if (columnStoreValues.Contains(sColumnName))
            {
                /*
                iquantity         = Convert.ToInt32 (_dtDropShipMatrix.Rows[e.RowIndex]["Quantity"]);
                iquantityassigned = Convert.ToInt32 (_dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"]);

                if (iquantityassigned > iquantity)
                {
                    dgvDropShipMatrix["QuantityAssigned", e.RowIndex].Style.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    dgvDropShipMatrix["QuantityAssigned", e.RowIndex].Style.BackColor = System.Drawing.SystemColors.Control;
                }
                */
                
                // ///////////////////////////////////////////////////////////////
                // Rounding stuff
                // Un comment if existing validation without rounding does not work 
                // correctly

                // HK : 09-12-2009 : If user selected a rounded value and clicked OK in 
                // the quantity rounded form ItemQuantityForm, this rounded quantity 
                // can be rejected by the following validation. In other words even 
                // though the rounded quantiy is valid as far as the rounding requirements 
                // go, they may not be valid as far as the summing of the quantities 
                // across the stores go.
                // You only want to put the rounded quantity in the cell if the summing 
                // validation succeeds.
                if (bStoreQuantityOk == true)
                {
                    if (_itemquantityrounded > 0)
                    {
                        dgvDropShipMatrix.Rows[e.RowIndex].Cells[sColumnName].Value = _itemquantityrounded;

                        // HK : 11-12-2009 : Assign the "QuantityAssigned" now
                        //_dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"] = GetSummedQuantity(sColumnName, e.RowIndex) + _itemquantityrounded;
                        dgvDropShipMatrix.Rows[e.RowIndex].Cells["QuantityAssigned"].Value = GetSummedQuantity(sColumnName, e.RowIndex) + _itemquantityrounded;

                        _itemquantityrounded = 0;
                        bStoreQuantityOk = false;

                    }
                }
                else
                {
                    // HK : 09-12-2009 : Force rounded quantity to be 0.
                    _itemquantityrounded = 0;
                    bStoreQuantityOk = false;
                }


                // ///////////////////////////////////////////////////////////////


            }
            
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Open IE Help File link into inline help 
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        // HK : 07-12-2009 :  Same as FindHitItemCasePackQty. However the 
        // name has to be changed to give more meaning to the logic 
        // performed by the method
        private int GetItemCasePackQty(int iitemindex, short iClass, int iVendor,
                                    short iStyle, short iColor, short iSize,
                                    ref short iCasepackqty)
        {

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                // HK : 28-11-2009 : Add ItemIndex
                if ((_porder.lstpoLineItemDetails[i].Itemindex == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].Classcode == iClass) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode == iStyle) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode == iColor) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize == iSize))
                {
                    iCasepackqty = _porder.lstpoLineItemDetails[i].Casepackqty;
                    return i;
                }

            }

            iCasepackqty = 0;
            return -1;
        }

        
        private bool ValidateQuantity(string svalue, int packQty)
        {
            bool bisValid;
            int itemqtyinput;

            // ///////////////////////////////////////////////////////////////
            // HK : 08-12-2009 : Since 0 is a valid quantity, we must not 
            // open the quantity rounding form if the user has entered a 0 quantity

            Int32.TryParse(svalue, out itemqtyinput);

            if (itemqtyinput == 0)
            {
                bisValid = true;
                return bisValid;
            }
            // ///////////////////////////////////////////////////////////////

            // HK : 11-01-2010 : Open the rounding form irrespective of 
            // whether the quantity entered is less than case pack quantity or not 

            //if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
            {

                if (itemqtyinput % packQty != 0)
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty);

                    itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);

                    if (itemqtyform.ShowDialog(this) == DialogResult.OK)
                    {
                        bisValid = true;
                    }

                    itemqtyform = null;

                }

                bisValid = true;
            }
            else
            {
                bisValid = false;

            }

            return bisValid;

        }


        // HK: 09-12-2009 : Maybe obsolete
        private bool ValidateQuantity(string svalue, int packQty, Boolean bObsolete)
        {
            bool bisValid;
            int itemqtyinput;

            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
            {
                if (itemqtyinput % packQty != 0)
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty);

                    itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);

                    if (itemqtyform.ShowDialog(this) == DialogResult.OK)
                    {
                        bisValid = true;
                    }
                }

                bisValid = true;
            }
            else
            {
                bisValid = false;

            }

            return bisValid;

        }

        void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        {
            _itemquantityrounded = iroundedquantity;
        }

        private void ObsoleteValidation_onDatagrid()
        {
            // //////////////////////////////////////////////////////////////////////
            // HK : FC : Do not validate quantity if 0 or null is entered by user
            // Once again comment this block and the block below to have default 
            // validation
            /*
            if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                // HK : 03-12-2009 : Not exactly right but we cannot
                // set an intger to null.
                //_polinedetails.Itemquantity = 0;
                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                e.Cancel = false;

                return;
            }

            // HK : 08-12-2009 : If quantity entered = 0 then take away the 
            // quantity already in the cell from the Quantiy Assigned and allow 
            // navigation to continue.
            if (!Int32.TryParse(squantityincell, out iquantityincell))
            {
                Debug.Print("Validatiion failed: ");
                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Please enter a valid quantity";
                e.Cancel = true;
                return;
            }

            if (!Int32.TryParse(squantityentered, out iquantityentered))
            {
                Debug.Print("Validatiion failed: ");
                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Please enter a valid quantity";
                e.Cancel = true;
                return;
            }

            // If quantity in cell =0 and quantity entered = 0
            if (iquantityincell == 0 && iquantityentered == 0)
            {
                //_polinedetails.Itemquantity = 0;
                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                e.Cancel = false;

                return;

            }
                    
            // If quanntity in cell > 0 and quantity entered = 0 then
            // Substract quantity assigned by quantiy in cell
            if (iquantityincell > 0 && iquantityentered == 0)
            {

                //_dtDropShipMatrix.Rows[e.RowIndex][sColumnName] = Convert.ToInt32(squantityentered);
                iquantityassigned = Convert.ToInt32 (_dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"]);
                _dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"] = iquantityassigned - iquantityincell; //Convert.ToInt32(squantityincell);

                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                e.Cancel = false;

                //_polinedetails.Itemquantity = 0;
                dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                e.Cancel = false;

                return;

            }
            */
            // //////////////////////////////////////////////////////////////////////

        }

        // HK : 11-12-2009 : Use this validation if the Item Rounding validation causes problems.
        // This is plain simple summing validation across all stores. It does not include Item 
        // quantity rounding suggestion i.e does not validate itme quantity against pack quantity
        private void dgvDropShipMatrix_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string sColumnName;
            string sStoreColumn;
            int iSummedQuantity = 0;
            int iTemp;
            int iquantityalreadrentered;
            int iQuantity;
            string squantityentered;
            int iquantityentered;

            if (bOkToValidateQuantityEntered)
            {
                // Get the column name the grid is trying to validate
                sColumnName = dgvDropShipMatrix.Columns[e.ColumnIndex].Name;

                // If it is a valid store column
                if (columnStoreValues.Contains(sColumnName))
                {
                    Debug.Print("Drop Ship Matrix column name being validate: " + sColumnName);

                    // Get the quantity the user is trying to enter
                    squantityentered = e.FormattedValue.ToString();

                    // Try and convert the quantity entered into a integer
                    // If not then throw a error
                    if (!String.IsNullOrEmpty(squantityentered) && Int32.TryParse(squantityentered, out iquantityentered))
                    {

                        // Get the total quantity on this item
                        iQuantity = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex]["Quantity"]);
                        Debug.Print("Total Quantity: " + iQuantity);

                        // Quantity entered as string
                        squantityentered = e.FormattedValue.ToString();
                        Debug.Print("Quantity Entered: " + squantityentered);

                        // Quantity entered as integer
                        iquantityalreadrentered = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex][e.ColumnIndex]);
                        Debug.Print("Quantity entered by user: " + iquantityalreadrentered);

                        // Sum the ammount across all columns on the row EXCEPT the current column for which the 
                        // value is being changed. i.e EXCEPT for the current column being validated
                        for (int i = 0; i < columnStoreValues.Count; i++)
                        {
                            // Get the Store column name
                            sStoreColumn = columnStoreValues[i];

                            // If it is not the one we are validating then
                            // sum up the value
                            if (!sStoreColumn.Equals(sColumnName))
                            {
                                Debug.Print("Processing quantity for store: " + sStoreColumn);

                                iTemp = Convert.ToInt32(_dtDropShipMatrix.Rows[e.RowIndex][columnStoreValues[i]]);
                                Debug.Print("Quantity for store: " + sStoreColumn + ": " + iTemp.ToString());

                                iSummedQuantity = iSummedQuantity + iTemp;

                            }
                            else
                            {
                                Debug.Print("Skipping processing quantity for current store: " + sStoreColumn);
                            }
                        }

                        Debug.Print("Quantity summed across all stores except current store: " + iSummedQuantity.ToString());

                        //if (iSummedQuantity + Convert.ToInt32(squantityentered) <= iQuantity)

                        // If summed quantity + quantity entered <= total quantity 
                        // then our validation is ok
                        if (iSummedQuantity + Convert.ToInt32(squantityentered) <= iQuantity)
                        {
                            Debug.Print("Validatiion successful: ");
                            Debug.Print("Summed item quantity: " + iSummedQuantity + " + " + "Quantity entered:" + squantityentered + " LESS THAN or EQUALS " + "Total Quantity" + iQuantity.ToString());


                            _dtDropShipMatrix.Rows[e.RowIndex][sColumnName] = Convert.ToInt32(squantityentered);
                            _dtDropShipMatrix.Rows[e.RowIndex]["QuantityAssigned"] = (iSummedQuantity) + Convert.ToInt32(squantityentered);

                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "";
                            e.Cancel = false;
                        }
                        else
                        {
                            Debug.Print("Validatiion failed: ");
                            Debug.Print("Summed item quantity: " + iSummedQuantity + " + " + "Quantity entered:" + squantityentered + " NOT EQUALS " + "Total Quantity" + iQuantity.ToString());
                            // Validation fails
                            dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Quantity Assigned should be less than or equal to Total quantity";
                            e.Cancel = true;
                        }
                    }

                    else
                    {
                        Debug.Print("Validatiion failed: ");
                        dgvDropShipMatrix.Rows[e.RowIndex].ErrorText = "Quantity Assigned should be less than or equal to Total quantity";
                        e.Cancel = true;
                    }

                }

            }
        }


    }

}