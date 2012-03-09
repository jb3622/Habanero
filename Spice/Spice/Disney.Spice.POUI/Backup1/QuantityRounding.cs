using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Disney.Spice.POUI
{
    public partial class QuantityRounding : Form
    {
        // Class fields
        private Int32 downqty, upqty, orderqty;

        // Properties
        private Int32 roundedqty = 0;
        public Int32 RoundedQuantity
        {
            get { return roundedqty; }
        }

        public QuantityRounding(Int32 OrderQty, Int32 DistroQty)
        {
            InitializeComponent();

            orderqty = OrderQty; 

            // Calculate Up/Down Quantities
            Int32 Multiplier = OrderQty / DistroQty;
            downqty = Multiplier * DistroQty;
            upqty   = (Multiplier + 1) * DistroQty;
        }

        private void QuantityRounding_Load(object sender, EventArgs e)
        {
            lblDownQty.Text = downqty.ToString();
            lblUpQty.Text   = upqty.ToString();

            if (downqty == 0)
            {
                rdoDown.Enabled = false;
                rdoUp.Checked   = true;
            }
            else
            {
                // Calculate the nearest to passed quantity, select the 
                // approiate radio button
                int diffup = Math.Abs(upqty - orderqty);
                int diffdown = Math.Abs(orderqty - downqty);

                if (diffup > diffdown)
                {
                    rdoDown.Checked = true;
                }
                else
                {
                    // Round UP quantity is nearer to entered quantity
                    rdoUp.Checked = true;
                }

                btnOk.Focus();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (rdoUp.Checked == true)
                roundedqty = upqty;
            else
                roundedqty = downqty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            roundedqty = orderqty;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}