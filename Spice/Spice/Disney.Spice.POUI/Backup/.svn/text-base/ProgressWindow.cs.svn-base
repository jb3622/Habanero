using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Disney.Spice.POUI
{
    public partial class ProgressWindow : Form
    {

        private int pmaxsize;
        private int progressbarvalue;

        public int Progressbarvalue
        {
            get { return prgbarOrders.Value; }
            set { prgbarOrders.Value = value; }
        }

        public ProgressWindow(int pmaxsize)
        {
            InitializeComponent();

            //Max Size =  # of orders
            prgbarOrders.Maximum = pmaxsize;

                        
        }

        public void UpdateProgressBar(int value)
        {
            
            progressbarvalue = value;
            if (progressbarvalue > prgbarOrders.Maximum)
            {
                prgbarOrders.Value = prgbarOrders.Maximum;
                
               
            }
            UpdateProgress();
        }
      
        private  void UpdateProgress()
        {
            //Can only update controls from within the form
            prgbarOrders.Value = (progressbarvalue < prgbarOrders.Maximum) ?  progressbarvalue:prgbarOrders.Maximum;
            lblProgressValue.Text = prgbarOrders.Value.ToString() + " / " + prgbarOrders.Maximum;

            this.Invalidate();
        
        }

} 
}