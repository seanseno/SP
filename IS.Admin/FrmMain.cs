﻿using IS.Admin.Setup;
using IS.Admin.Setup.Cashier;
using IS.Admin.Trasactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IS.Admin
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSales frm = new FrmSales();
            frm.ShowDialog();
        }

        private void logOfffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogin frm = new FrmLogin();
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {

            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            FrmLogin frm = new FrmLogin();
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {
                
            }
        }

        private void administratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAdministrators frm = new FrmAdministrators();
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {

            }
        }

        private void addUpdateCashierInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCashiers frm = new FrmCashiers();
            frm.ShowDialog();
        }

        private void cashOnHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCashierOnhand frm = new FrmCashierOnhand();
            frm.ShowDialog();
        }

        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCategories frm = new FrmCategories();
            frm.ShowDialog();
        }

        private void itemsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmItems frm = new FrmItems();
            frm.ShowDialog();
        }

        private void uploadItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadExcel frm = new FrmUploadExcel();
            var response = frm.ShowDialog();
        }

        private void stocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmStocks frm = new FrmStocks();
            frm.ShowDialog();
        }

        private void requestOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmItemRequestOrderList frm = new FrmItemRequestOrderList();
            frm.ShowDialog();
        }

        private void receivedOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmReceivedItems frm = new FrmReceivedItems();
            frm.ShowDialog();
        }

        private void principalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPrincipals frm = new FrmPrincipals();
            frm.ShowDialog();
        }
    }
}
