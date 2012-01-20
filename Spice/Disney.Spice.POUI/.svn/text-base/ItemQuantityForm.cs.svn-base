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
    public partial class ItemQuantityForm : Form
    {
        // This form is called when the "Quantity" is validated in 
        // the PO Line Items datagrid. If the user entered a valid value which does  
        // not cause this form to be called the the normal validation process should 
        // handle the display the entered value in the grid and validation should 
        // succeed on that record. If the user entered a quantity which causes this 
        // form to be called and the user rounds UP or rounds down the quantity then
        // the process ASH originally coded and the normal validation that follows, 
        // all of that throws away the new quantity rounded up or down by the user 
        // in thsis form
        
        public delegate void QuantityRoundedEventHandler(object sender, int iroundedquantity);

        public event QuantityRoundedEventHandler OnQuantityRounded;

        private int _iqtyentered;
        private int _ipackqty;
        private int _irounddownqty;
        private int  _iroundupqty;

        POItemDetails _poitemdetails;
        
        private int _datagridrowindex;
        private int _iroundedquantity;

        public ItemQuantityForm(int qtyentered, int packQty, ref POItemDetails poitemdetails)
        {
            InitializeComponent();

            _iqtyentered = qtyentered;
            _ipackqty = packQty;
            _poitemdetails = poitemdetails;
       
            CalculateQuantities();
                      
            lblUpQty.Text = _iroundupqty.ToString();
            lblDownQty.Text = _irounddownqty.ToString();
        }

        // Capture the datagridview row index so that 
        // correct line item on main PO Entry Form is 
        // updated
        public ItemQuantityForm(int qtyentered, int packQty, ref POItemDetails poitemdetails, int datagridrowindex)
        {
            InitializeComponent();

            _iqtyentered = qtyentered;
            _ipackqty = packQty;
            _poitemdetails = poitemdetails;
            
            _datagridrowindex = datagridrowindex;

            CalculateQuantities();

            lblUpQty.Text = _iroundupqty.ToString();
            lblDownQty.Text = _irounddownqty.ToString();

            if (_irounddownqty == 0)
            {
                rdBtnDown.Enabled = false;
                lblDownQty.Enabled = false;
                rdBtnUp.Checked = true;
                btnOK.Focus();
            }
            else
            {
                int differenceup = Math.Abs(_iroundupqty - _iqtyentered);
                int differencedown = Math.Abs(_iqtyentered - _irounddownqty);

                // Round DOWN quantity is nearer to entered quantity
                if (differenceup > differencedown)
                {
                    rdBtnDown.Checked = true;
                    btnOK.Focus();
                }
                else
                {
                    // Round UP quantity is nearer to entered quantity
                    rdBtnUp.Checked = true;
                    btnOK.Focus();
                }
            }
        }

        // Custom constructor for calling this form from 
        // PO Hits and maybe from Drop Shi PO
        // We dont need , ref POItemDetails poitemdetails as it does 
        // nothing anyway. All quantities rounded are handled 
        // by our event handlers.
        public ItemQuantityForm(int qtyentered, int packQty)
        {
            InitializeComponent();

            _iqtyentered = qtyentered;
            _ipackqty = packQty;

            CalculateQuantities();

            lblUpQty.Text = _iroundupqty.ToString();
            lblDownQty.Text = _irounddownqty.ToString();

            if (_irounddownqty == 0)
            {
                rdBtnUp.Checked = true;
                rdBtnDown.Enabled = false;
                lblDownQty.Enabled = false;
                btnOK.Focus();
            }
            else
            {
                int differenceup = Math.Abs(_iroundupqty - _iqtyentered);
                int differencedown = Math.Abs(_iqtyentered - _irounddownqty);

                // Round DOWN quantity is nearer to entered quantity
                if (differenceup > differencedown)
                {
                    rdBtnDown.Checked = true;
                    btnOK.Focus();
                }
                else
                {
                    // Round UP quantity is nearer to entered quantity
                    rdBtnUp.Checked = true;
                    btnOK.Focus();
                }
            }
        }

        private void CalculateQuantities()
        {
            int minMultiplier = (_iqtyentered / _ipackqty);
            _irounddownqty = (minMultiplier * _ipackqty);
            _iroundupqty = ((minMultiplier + 1) * _ipackqty);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdBtnUp.Checked)
            {
                // HK : 02-12-2009 : When ASH sets the _poitemdetails.Itemquantity = rounded quantity
                // it wil crash if called from the PO Hits form and Drop Ship Matrix from as the said
                // object will be null. Only when called from the main PO Entry form will the 
                // said object refer to a valid PO Line Items object
                if (_poitemdetails != null)
                {
                    _poitemdetails.Itemquantity = _iroundupqty;
                }
                _iroundedquantity = _iroundupqty;
            }

            if (rdBtnDown.Checked)
            {
                if (_poitemdetails != null)
                {
                    _poitemdetails.Itemquantity = _irounddownqty;
                }
                
                _iroundedquantity = _irounddownqty;
            }

            // Let existing code handle the change to new rounded quantity 
            // for the PO Item Details (_poitemdetails) class object 
            //_poitemdetails.RaiseItemorQtyChanged(0); //Rowindex param  
            if (_poitemdetails != null)
            {
               // _poitemdetails.RaiseItemorQtyChanged(_poitemdetails.Itemindex - 1); //Rowindex param  
                // HK : Update the correct grid record
                //_poitemdetails.RaiseItemorQtyChanged(_datagridrowindex); //Rowindex param  
            }
            
            this.DialogResult = DialogResult.OK;

            RaiseQuantityRounderEvent(_iroundedquantity);

            this.Close();          
        }

        private void rdBtnUp_CheckedChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
        }

        private void rdBtnDown_CheckedChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RaiseQuantityRounderEvent(int iroundedquantity)
        {
            if (OnQuantityRounded != null)
            {
                OnQuantityRounded (this, iroundedquantity);
            }
        }
    }
}