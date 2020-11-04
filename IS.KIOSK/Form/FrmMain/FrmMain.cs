﻿using IS.Common.Utilities;
using IS.Database;
using IS.Database.Entities;
using IS.Database.Enums;
using IS.KIOSK.Model;
using IS.Library.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IS.KIOSK
{
    public partial class FrmMain : Form
    {
        public TempLedgerSales _TempLedgerSales { get; set; }
        public IList<TempSales> _TempOrderList { get; set; }
        public IList<Products> _ItemList { get; set; }

        public Cashiers _Cashier { get; set; }
        ISFactory factory = new ISFactory();
        MainModel mainModel = new MainModel();
        public decimal _TotalPrice { get; set; }
        int CountErrorlabel = 0;
        public string _CustomerName { get; set; }
        public string _AdditionalInfo { get; set; }
        public bool _IsDicounted { get; set; }
        public FrmMain()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            _IsDicounted = false;
            panel1.Visible = true;
            timer2.Start();

            string LogoPath = ThemesUtility.Logo();
            if (!string.IsNullOrEmpty(LogoPath))
            {
                if (File.Exists(LogoPath))
                {
                    pnlLogo.BackgroundImage = Image.FromFile(ThemesUtility.Logo());
                }
            }
            lblCompanyName.Text = ThemesUtility.CompanyName();
            BackColor = ThemesUtility.BackColor();

            frmLogin frm = new frmLogin(this);
            var response = frm.ShowDialog();
            if (response == DialogResult.OK)
            {
                lblLogin.Text = "Current Login: " + Globals.LoginName;
                this.ActiveControl = txtCustomerName;
            }
            
        }
        private void load()
        {
            _TempLedgerSales = factory.TempLedgerSalesRepository.FindDefault(this._Cashier.CashierId,txtCustomerName.Text.ToUpper());
            _TempOrderList = mainModel.LoadTempOders(this,txtCustomerName.Text).Item1;
            _TotalPrice = mainModel.LoadTempOders(this, txtCustomerName.Text).Item2;

            if (_TempOrderList.Where(x => x.Discount > 0).Count() > 0)
            {
                _IsDicounted = true;
            }
            else
            {
                _IsDicounted = false;
            }
            dgvList.AutoGenerateColumns = false;
            dgvList.DataSource = _TempOrderList;
            dgvList.StandardTab = true;

            lblTotal.Text = String.Format("{0:n}", _TotalPrice);

        }


        private void CallKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 112) //help
            {
               this.btnHelp_Click(sender, e);
            }
            if (e.KeyValue == 113) //load
            {
                this.btnSearch_Click(sender, e);
            }
            if (e.KeyValue == 114) //load
            {
                this.btnLoad_Click(sender, e);
            }

            if (e.KeyValue == 115) //check out
            {
                this.btnCheckOut_Click(sender, e);;
            }
            if (e.KeyValue == 116) // Remove all
            {
                this.btnRemoveAll_Click(sender, e);
            }
            if (e.KeyValue == 117) // Save 
            {
                this.btnSave_Click(sender, e);;
            }
            if (e.KeyValue == 118) // Return Item
            {
                this.btnReturnItem_Click(sender, e);
            }
            if (e.KeyValue == 119) // Reprint
            {
                this.btnReprint_Click(sender, e);
            }
            if (e.KeyValue == 123) // Exit
            {
                this.btnExit_Click(sender, e);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!lblError.Visible)
            {
                lblError.Visible = true;
            }
            else
            {
                lblError.Visible = false;
            }
            CountErrorlabel++;
            if (CountErrorlabel >= 6)
            {
                lblError.Visible = true;
                timer1.Stop();
                CountErrorlabel = 0;
            }
        }

        private void dgvSearch_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //lblError.Visible = false;
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            //CallKeyPress(sender, e);
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 18)
            {
                //txtSearch.Focus();

            }
            CallKeyPress(sender, e);
            e.Handled = false;
        }

        private void dgvOrders_KeyDown(object sender, KeyEventArgs e)
        {
            //CallKeyPress(sender, e);
            if (e.KeyValue == 46)
            {

                var GenericName = dgvList.CurrentRow.Cells[1].Value?.ToString();
                var BranName = dgvList.CurrentRow.Cells[2].Value?.ToString();
                var Description = dgvList.CurrentRow.Cells[3].Value?.ToString();
                var Params = new List<string>();
                if (this._TempOrderList != null)
                {
                    if (!string.IsNullOrEmpty(GenericName))
                    {
                        Params.Add(GenericName);
                    }
                    if (!string.IsNullOrEmpty(BranName))
                    {
                        Params.Add(BranName);
                    }
                    if (!string.IsNullOrEmpty(Description))
                    {
                        Params.Add(Description);
                    }

                    if (MessageBox.Show("Removing " + string.Join(" ", Params) + ".", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        MainModel mainModel = new MainModel();
                        mainModel.DeleteTempOrder(this, (int)dgvList.CurrentRow.Cells[0].Value);
                        load();
                    }
                }
                //txtSearch.Focus();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure do you want to exit.", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            if (this._TempOrderList != null)
            {
                if (this._TempOrderList.Count() > 0)
                {
                    if (_IsDicounted == true && string.IsNullOrEmpty(txtCustomerName.Text))
                    {
                        MessageBox.Show("Product discounted Detected!, Customer Name is required!", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCustomerName.Focus();
                    }
                    else if (_IsDicounted == true && string.IsNullOrEmpty(txtAdditionalInfo.Text))
                    {
                        MessageBox.Show("Product discounted Detected!, Additional Info is required!", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtAdditionalInfo.Focus();
                    }
                    else
                    {
                        FrmCheckOut frm = new FrmCheckOut(this);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            load();
                            txtCustomerName.Text = "";
                            txtAdditionalInfo.Text = "";
                        }
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (factory.TempLedgerSalesRepository.TempLedgerSalesStrategy.CheckTempLedgerHasSales(this._TempLedgerSales))
            {
                MessageBox.Show("Ongoing sales detected, please save first.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                FrmLoadSaveOrders frm = new FrmLoadSaveOrders(this);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    load();
                    txtCustomerName.Text = "";
                    txtAdditionalInfo.Text = "";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this._TempOrderList != null)
            {
                if (this._TempOrderList.Count() > 0)
                {
                    if (MessageBox.Show("Are you sure do you want to save orders.", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        mainModel.SaveOrders(this);
                        load();
                        txtCustomerName.Text = "";
                        txtAdditionalInfo.Text = "";
                        return;
                    }
                }
            }
            MessageBox.Show("No orders detected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //txtSearch.Focus();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (this._TempOrderList != null)
            {
                if (this._TempOrderList.Count() > 0)
                {
                    if (MessageBox.Show("Are you sure do you want to delete all order", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        MainModel mainModel = new MainModel();
                        mainModel.DeleteAllTempOrder(this);
                        load();
                        MessageBox.Show("Orders deleted", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("No orders detected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //txtSearch.Focus();
        }


        private void FrmMain_Shown(object sender, EventArgs e)
        {
            load();
        }

        private void dgvSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FrmModalSearchProducts frm = new FrmModalSearchProducts();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                lblError.Visible = false;
                if (factory.TempSalesRepository.TempSalesStrategy.CheckIfOrderExist(this._TempLedgerSales, frm._ProductId))
                {
                    lblError.Text = string.Format("{0} already added! ", frm._ProductName);
                    timer1.Start();
                }
                else if (!factory.StocksDataRepository.StocksDataStrategy.CheckStock(frm._ProductId))
                {
                    lblError.Text = string.Format("{0} is out of stock!", frm._ProductName);
                    timer1.Start();
                }
                else
                {
                    frmMultiplier frmMultiplier = new frmMultiplier(this, frm._ProductId);
                    if (frmMultiplier.ShowDialog() == DialogResult.OK)
                    {
                        load();
                    }
                }
            }
            else
            {
                lblError.Visible = false;
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToLongTimeString();
            lblDate.Text = DateTime.Now.ToString("MMMM ddd yyyy - dddd");
        }

        private void btnReturnItem_Click(object sender, EventArgs e)
        {
            FrmReturnItem frm = new FrmReturnItem();
            frm.ShowDialog();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            FrmHelp frm = new FrmHelp();
            frm.ShowDialog();
        }

        private void txtAdditionalInfo_KeyUp(object sender, KeyEventArgs e)
        {
            _AdditionalInfo = txtAdditionalInfo.Text;
        }

        private void txtCustomerName_KeyUp(object sender, KeyEventArgs e)
        {
            _CustomerName = txtCustomerName.Text;
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure do you want to re-print your last transaction?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var xx = factory.CashiersRepository.GetList();
                var user = factory.CashiersRepository.GetList().Where(x => x.Loginname.ToUpper().Trim() == Globals.LoginName.ToUpper().Trim()).FirstOrDefault();
                var response = factory.LedgerSalesRepository.GetList().Where(x => x.CashierId == user.CashierId).OrderByDescending(y => y.Id).FirstOrDefault();
                PrintReceipt(response.Id);
            }
        }

        public void PrintReceipt(int LedgerId)
        {
            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                var Items = factory.SalesRepository.GetSalesDetailListReport().Where(x => x.LedgerId == LedgerId).OrderBy(y => y.Id);

                var Date = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "Date").FirstOrDefault();
                e1.Graphics.DrawString(DateTime.Now.ToString("MMMM dd, yyyy"), new Font("Times New Roman", Date.Size), Brushes.Black, new RectangleF(Date.X, Date.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

                var ReceiptNo = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "ReceiptNo").FirstOrDefault();
                e1.Graphics.DrawString(string.Format("{0:000000000000}", LedgerId), new Font("Times New Roman", ReceiptNo.Size), Brushes.Black, new RectangleF(ReceiptNo.X, ReceiptNo.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

                var SoldTo = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "SoldTo").FirstOrDefault();
                e1.Graphics.DrawString(Items.ToList().FirstOrDefault().CustomerName, new Font("Times New Roman", SoldTo.Size), Brushes.Black, new RectangleF(SoldTo.X, SoldTo.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));


                var Products = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "Products").FirstOrDefault();
                var ProductsQty = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "ProductsQty").FirstOrDefault();
                var ProductsPrice = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "ProductsPrice").FirstOrDefault();

                decimal TotalAmountPrice = 0;
                foreach (var itm in Items)
                {
                    string product = string.Empty;
                    decimal TotalPrice = Convert.ToDecimal(Convert.ToDecimal(itm?.Qty) * itm?.price);
                    TotalAmountPrice += TotalPrice;
                    var descList = WordWrap.Wrap(itm.ProductName, 50);
                    //int Count = 0;
                    //foreach (var desc in descList)
                    //{
                    //    if (Count == 0)
                    //    {
                    //        product += desc + "\n";
                    //    }
                    //    else
                    //    {
                    //        product += "--" + desc + "\n";
                    //    }

                    //    Count++;
                    //}

                    e1.Graphics.DrawString(itm.Qty?.ToString("N0"), new Font("Times New Roman", ProductsQty.Size), Brushes.Black, new RectangleF(ProductsQty.X, Products.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                    e1.Graphics.DrawString(TotalPrice.ToString("N2"), new Font("Times New Roman", ProductsPrice.Size), Brushes.Black, new RectangleF(ProductsPrice.X, Products.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                    foreach (string desc in descList)
                    {
                        e1.Graphics.DrawString(desc, new Font("Times New Roman", Products.Size), Brushes.Black, new RectangleF(Products.X, Products.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
                        Products.Y += 12;
                    }
                   
                }

                var Total = factory.PrinterCoordinatesRepository.GetList().Where(x => x.PrintingType == (int)PrinterType.Kiosk && x.PrintingLabel == "Total").FirstOrDefault();
                e1.Graphics.DrawString(TotalAmountPrice.ToString("N2"), new Font("Times New Roman", Total.Size), Brushes.Black, new RectangleF(Total.X, Total.Y, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

            };
            try
            {
                //p.Print();
                PrintDialog printDialog1 = new PrintDialog();
                printDialog1.Document = p;
                DialogResult result = printDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    p.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
