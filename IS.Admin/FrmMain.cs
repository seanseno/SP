﻿using IS.Admin.Model;
using IS.Admin.Reports;
using IS.Admin.Setup;
using IS.Admin.Setup.Cashier;
using IS.Admin.Transactions;
using IS.Admin.Trasactions;
using IS.Common.Utilities;
using IS.Database.Enums;
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
        private void FrmMain_Load(object sender, EventArgs e)
        {
            FrmLogin frm = new FrmLogin();
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {
                timer1.Start();
                panel1.Visible = true;
                lblLogin.Text = "Current Login: " + Globals.LoginName;
                MenuEnable();
            }
        }

        private void MenuEnable()
        {
            var administratorsModel = new AdministratorsModel();
            var admin = administratorsModel.FindAdministratorWithLoginname(Globals.LoginName.ToString());
            if (admin != null)
            {

                if (admin.UserType == (int)EnumUserType.Member)
                {
                    menuToolStripMenuItem.Visible = true;
                    logOffToolStripMenuItem.Visible = true;
                    exitToolStripMenuItem.Visible = true;

                    transactionsToolStripMenuItem.Visible = true;
                    stocksToolStripMenuItem.Visible = true;
                    ongoingStocksDataToolStripMenuItem.Visible = true;
                }
                else if (admin.UserType == (int)EnumUserType.SuperAdministrator)
                {
                    DisableAllMenu(true);
                }
                else if (admin.UserType == (int)EnumUserType.Administrator)
                {

                    

                }
                else if (admin.UserType == (int)EnumUserType.InventoryClerk)
                {
                    menuToolStripMenuItem.Visible = true;
                    logOffToolStripMenuItem.Visible = true;
                    exitToolStripMenuItem.Visible = true;
                    transactionsToolStripMenuItem.Visible = true;
                    stocksToolStripMenuItem.Visible = true;
                    stocksDataToolStripMenuItem.Visible = true;
                    verifyingStocksDataToolStripMenuItem.Visible = true;
                    allStocksToolStripMenuItem.Visible = true;
                }
                else if (admin.UserType == (int)EnumUserType.FinanceManager)
                {
                    menuToolStripMenuItem.Visible = true;
                    logOffToolStripMenuItem.Visible = true;
                    exitToolStripMenuItem.Visible = true;

                    reportToolStripMenuItem.Visible = true;
                    salesToolStripMenuItem.Visible = true;
                    sockDataProfitToolStripMenuItem.Visible = true;
                }
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

        private void cashiersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCashiers frm = new FrmCashiers();
            frm.ShowDialog();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProducts frm = new FrmProducts();
            frm.ShowDialog();
        }

        private void principalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPrincipals frm = new FrmPrincipals();
            frm.ShowDialog();
        }

        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCategories frm = new FrmCategories();
            frm.ShowDialog();
        }


        private void sockDataProfitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmStocksDataReport frm = new FrmStocksDataReport();
            frm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToLongTimeString();
            lblDate.Text = DateTime.Now.ToString("MMMM dd, yyyy - dddd");
        }

        private void ongoingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void allStocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmStocks frm = new FrmStocks();
            frm.ShowDialog();
        }

        private void ongoingStocksDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmOngoingStocksData frm = new FrmOngoingStocksData();
            frm.ShowDialog();
        }

        private void verifyingStocksDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmValidateOngoingStocksData frm = new FrmValidateOngoingStocksData();
            frm.ShowDialog();
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisableAllMenu(false);
            panel1.Visible = false;
            FrmLogin frm = new FrmLogin();
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {
                timer1.Start();
                panel1.Visible = true;
                lblLogin.Text = "Current Login: " + Globals.LoginName;
                MenuEnable();
            }
        }
        private void DisableAllMenu(bool value)
        {
            //MENU
            menuToolStripMenuItem.Visible = value;
            logOffToolStripMenuItem.Visible = value;
            exitToolStripMenuItem.Visible = value;

            //Transactions
            transactionsToolStripMenuItem.Visible = value;
            stocksToolStripMenuItem.Visible = value;
            stocksDataToolStripMenuItem.Visible = value;
            ongoingStocksDataToolStripMenuItem.Visible = value;
            verifyingStocksDataToolStripMenuItem.Visible = value;
            allStocksToolStripMenuItem.Visible = value;

            //setup
            setupToolStripMenuItem.Visible = value;
            categoriesToolStripMenuItem.Visible = value;
            principalsToolStripMenuItem.Visible = value;
            productsToolStripMenuItem.Visible = value;

            //Utilities
            utilitiesToolStripMenuItem.Visible = value;
            administratorToolStripMenuItem.Visible = value;
            cashiersToolStripMenuItem.Visible = value;

            //reports
            reportToolStripMenuItem.Visible = value;
            salesToolStripMenuItem.Visible = value;

            sockDataProfitToolStripMenuItem.Visible = value;
        }
        private void stocksDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmStocksData frm = new FrmStocksData();
            frm.ShowDialog();
        }

        private void productPriceHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProductPriceHistory frm = new FrmProductPriceHistory();
            frm.ShowDialog();
        }
    }
}
