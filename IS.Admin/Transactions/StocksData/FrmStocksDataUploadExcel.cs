﻿
using ExcelDataReader;
using IS.Admin.Model;
using IS.Common.Reader;
using IS.Common.Utilities;
using IS.Database;
using IS.Database.Entities;
using IS.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ZXing;

namespace IS.Admin.Transactions
{
    public partial class FrmStocksDataUploadExcel : BaseForm
    {
        DataTableCollection tableCollection;
        DataTable dt;

        ISFactory factory = new ISFactory();
            
        public FrmStocksDataUploadExcel()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Workbook|*.xlsx;*.xls", ValidateNames = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        tableCollection = ReadExcel.Read(ofd.FileName);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return;
                    }
                    txtFileName.Text = ofd.FileName;
                    cboSheet.Items.Clear();
                    foreach (DataTable dt in tableCollection)
                    {
                        cboSheet.Items.Add(dt.TableName);
                    }
                    dgvExcel.DataSource = null;
                    lblTotal.Text = "Total record(s) : " + cboSheet.Items.Count.ToString("N0");
                }
            }
        }

        private void cboSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt = tableCollection[cboSheet.SelectedIndex];
            dgvExcel.DataSource = dt;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFileName.Text))
                {
                    MessageBox.Show("Excel file is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (cboSheet.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select sheet!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    if (MessageBox.Show("Are you sure do want to continue?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        StocksDataModel request = new StocksDataModel();

                        ProductsModel pModel = new ProductsModel();
                        progressBar1.Maximum = dt.Rows.Count;
                        progressBar1.Minimum = 0;

                        progressBar1.Value = 0;
                        int progressCount = 0;

                        var ErrorList = new List<StocksData>();

                        IList<StocksData> list = new List<StocksData>();

                        int rowIndex = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            rowIndex++;
                            if (string.IsNullOrEmpty(row[0].ToString().ToUpper()) || 
                                string.IsNullOrEmpty(row[2].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[4].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[6].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[7].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[8].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[9].ToString().ToUpper()) ||
                                string.IsNullOrEmpty(row[10].ToString().ToUpper()))
                            {
                                MessageBox.Show(string.Format("Row {0} is not valid, please check the row information!", rowIndex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dgvExcel.Rows[rowIndex - 1].Selected = true;
                                return;
                            }
                            else if (factory.ProductsRepository.GetList().Where(x=>x.ProductId == row[2].ToString() && x.Active == (int)EnumActive.Active).Count() <= 0)
                            {
                                MessageBox.Show(string.Format("Product ID:{0} does not exist!", row[2].ToString().ToUpper()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dgvExcel.Rows[rowIndex - 1].Selected = true;
                                return;
                            }
                            else if (factory.StocksDataRepository.StocksDataStrategy.CheckOngoingStockData(row[2].ToString().ToUpper()))
                            {
                                MessageBox.Show("Ongoing Stock detected!" + "\n" + "Product Id: " + row[2].ToString().ToUpper() + "." + "\n" + "Please validate first before you can procced in this transaction!", "Information!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                                
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            progressCount++;
                            StocksData stocksData = new StocksData();
                            stocksData.Loginname = Globals.LoginName;
                            stocksData.PrincipalId = row[0].ToString().ToUpper();
                            stocksData.ProductId = row[2].ToString().ToUpper();
                            stocksData.Quantity = Convert.ToInt32(row[4].ToString());
                            stocksData.SupplierPrice = Convert.ToDecimal(row[5].ToString());
                            stocksData.TotalAmount = Convert.ToDecimal(row[6].ToString());
                            stocksData.RemainingQuantity = Convert.ToInt32(row[7].ToString());
                            stocksData.DeliveryDate = Convert.ToDateTime(row[8].ToString());
                            stocksData.ExpirationDate = Convert.ToDateTime(row[9].ToString());
                            stocksData.Remarks = row[10].ToString().ToUpper();
                            stocksData.CategoryId = factory.ProductsRepository.GetList().Where(x => x.ProductId == stocksData.ProductId).FirstOrDefault().CategoryId;
                            request.InsertStockData(stocksData);
                            progressBar1.Value = progressCount;
                        }
                        MessageBox.Show("Stocks Data Uploaded!", "Completed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
