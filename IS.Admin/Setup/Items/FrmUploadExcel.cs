﻿
using ExcelDataReader;
using IS.Admin.Model;
using IS.Common.Reader;
using IS.Database.Entities;
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

namespace IS.Admin.Setup
{
    public partial class FrmUploadExcel : Form
    {
        DataTableCollection tableCollection;
        DataTable dt;
        public FrmUploadExcel()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private IList<string> GetColumnList()
        {
            IList<string> list = new List<string>();

            list.Add("--Select--");
            foreach (DataColumn c in dt.Columns)
            {
                list.Add(c.ColumnName);
            }
            return list;
        }
        private void LoadColumnCombo()
        {
            cbo1.DataSource = this.GetColumnList();
            cbo2.DataSource = this.GetColumnList();
            cbo3.DataSource = this.GetColumnList();
            cbo4.DataSource = this.GetColumnList();
            cbo5.DataSource = this.GetColumnList();
            cbo6.DataSource = this.GetColumnList();
            cbo7.DataSource = this.GetColumnList();
            cbo8.DataSource = this.GetColumnList();
            cbo9.DataSource = this.GetColumnList();
            cbo10.DataSource = this.GetColumnList();
            cbo11.DataSource = this.GetColumnList();
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
                }
            }
        }

        private void cboSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt = tableCollection[cboSheet.SelectedIndex];

            //List<DataRow> list = dt.AsEnumerable().ToList();
            //List<DataRow> list = new List<DataRow>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    list.Add(dr);
            //}
            dgvExcel.DataSource = dt;
            LoadColumnCombo();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Excel file is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (cboSheet.SelectedIndex < 0)
            {
                MessageBox.Show("Please select sheet!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dt.Columns.Count < 9)
            {
                MessageBox.Show("Invalid record detected, must be greater than or equal 9 columns", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!CheckIfColumnSet())
            {
                MessageBox.Show("Column not yet set!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!CheckIfColumnSetSame())
            {
                MessageBox.Show("Invalid same column set!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                if (MessageBox.Show("Are you sure do want to continue?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    progressBar1.Maximum = dt.Rows.Count;
                    progressBar1.Minimum = 0;

                    progressBar1.Value = 0;
                    int progressCount = 0;

                    var ErrorItemList = new List<Items>();

                    IList<UploadExcelItem> UploadExcelItemList = new List<UploadExcelItem>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new UploadExcelItem();
                        item.Categories = row[cbo1.SelectedIndex - 1].ToString().ToUpper();
                        item.Companies = row[cbo2.SelectedIndex - 1].ToString().ToUpper();
                        item.GenericName = row[cbo3.SelectedIndex - 1].ToString().ToUpper();
                        item.BrandName = row[cbo4.SelectedIndex - 1].ToString().ToUpper();
                        item.Description = row[cbo5.SelectedIndex - 1].ToString().ToUpper();
                        if (!row.IsNull(cbo6.SelectedIndex - 1))
                        {
                            item.SupplierPrice = Convert.ToDecimal(row[cbo6.SelectedIndex - 1]);
                        }
                        else
                        {
                            item.SupplierPrice = 0;
                        }

                        if (!row.IsNull(cbo7.SelectedIndex - 1))
                        {
                            item.SellingPricePerPiece = Convert.ToInt32(row[cbo7.SelectedIndex - 1]);
                        }
                        else
                        {
                            item.SellingPricePerPiece = 0;
                        }
                        if (!row.IsNull(cbo8.SelectedIndex - 1))
                        {
                            item.Quantity = Convert.ToInt32(row[cbo8.SelectedIndex - 1]);
                        }
                        else
                        {
                            item.Quantity = 0;
                        }

                        if (DateTime.TryParse(row[cbo9.SelectedIndex - 1].ToString(), out DateTime dateManufactured))
                        {
                            item.DateManufactured = dateManufactured;
                        }
                        else
                        {
                            item.DateManufactured = DateTime.Now;
                        }

                        if (DateTime.TryParse(row[cbo10.SelectedIndex - 1].ToString(), out DateTime expirationDate))
                        {
                            item.ExpirationDate = expirationDate;
                        }
                        else
                        {
                            item.ExpirationDate = DateTime.Now;
                        }

                        if (DateTime.TryParse(row[cbo11.SelectedIndex - 1].ToString(), out DateTime deliveryDate))
                        {
                            item.DeliveryDate = deliveryDate;
                        }
                        else
                        {
                            continue;
                        }

                        UploadExcelItemList.Add(item);
                    }

                    var listGroup = UploadExcelItemList.GroupBy(s => s.DeliveryDate).OrderBy(o => o.Key).Select(s =>s.Key);
                    if (MessageBox.Show(listGroup.Count() + " group(s) has been found, do you want to continue?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        IList<DateTime> dateGroup = new List<DateTime>();
                        foreach (var itm in listGroup)
                        {
                            dateGroup.Add(itm);
                        }
                        foreach (var date in dateGroup)
                        {
                            RequestOrderItemsModel request = new RequestOrderItemsModel();
                            var RequestOrderId = request.InsertRequestOrderItemsWithUploadItem(date);

                            var Group = UploadExcelItemList.Where(s => s.DeliveryDate == date).ToList();
                            foreach (var upload in Group)
                            {
                                var itemModel = new ItemsModel();
                                var item = new Items();
                                item.RequestOrderId = RequestOrderId;
                                try
                                {
                                    item.CategoryName = upload.Categories;
                                    item.CompanyName = upload.Companies;
                                    item.GenericName = upload.GenericName;
                                    item.BrandName = upload.BrandName;
                                    item.Description = upload.Description;
                                    item.SupplierPrice = upload.SupplierPrice;
                                    item.SellingPricePerPiece = upload.SellingPricePerPiece;
                                    item.Stock = upload.Quantity;
                                    item.DateManufactured = upload.DateManufactured;
                                    item.ExpirationDate = upload.ExpirationDate;
                                    item.DateReceived = upload.DeliveryDate;
                                    item.BarCode = "";
                                    var ResponseUpload = itemModel.UploadExcel(item);
                                }
                                catch (Exception ex)
                                {
                                    ErrorItemList.Add(item);
                                }
                                finally
                                {
                                    progressCount++;
                                    progressBar1.Value = progressCount;
                                }
                            }
                        }
                        if (ErrorItemList.Count > 0)
                        {
                            MessageBox.Show("Item uploaded! but some items does not uploaded, Please check the Item information.", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FrmNotUploaded frm = new FrmNotUploaded(ErrorItemList);
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                this.DialogResult = DialogResult.OK;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Item Uploaded!", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    var itemModel = new ItemsModel();
                    //    var item = new Items();
                    //    item.RequestOrderId = RequestOrderId;
                    //    try
                    //    {
                    //        item.CategoryName = row[cbo1.SelectedIndex - 1].ToString().ToUpper();
                    //        item.CompanyName = row[cbo2.SelectedIndex - 1].ToString().ToUpper();
                    //        item.GenericName = row[cbo3.SelectedIndex - 1].ToString().ToUpper();
                    //        item.BrandName = row[cbo4.SelectedIndex - 1].ToString().ToUpper();
                    //        item.Description = row[cbo5.SelectedIndex - 1].ToString().ToUpper();

                    //        lblProgressbar.Text = "..../" + item.Description;
                    //        lblProgressbar.Refresh();
                    //        if (!row.IsNull(cbo6.SelectedIndex - 1))
                    //        {
                    //            item.SupplierPrice = Convert.ToDecimal(row[cbo6.SelectedIndex - 1]);
                    //        }
                    //        else
                    //        {
                    //            item.SupplierPrice = 0;
                    //        }

                    //        if (!row.IsNull(cbo7.SelectedIndex - 1))
                    //        {
                    //            item.SellingPricePerPiece = Convert.ToInt32(row[cbo7.SelectedIndex - 1]);
                    //        }
                    //        else
                    //        {
                    //            item.SellingPricePerPiece = 0;
                    //        }
                    //        if (!row.IsNull(cbo8.SelectedIndex - 1))
                    //        {
                    //            item.Stock = Convert.ToInt32(row[cbo8.SelectedIndex - 1]);
                    //        }
                    //        else
                    //        {
                    //            item.Stock = 0;
                    //        }

                    //        if (DateTime.TryParse(row[cbo9.SelectedIndex - 1].ToString(), out DateTime dateManufactured))
                    //        {
                    //            item.DateManufactured = dateManufactured;
                    //        }
                    //        else
                    //        {
                    //            item.DateManufactured = DateTime.Now;
                    //        }

                    //        if (DateTime.TryParse(row[cbo10.SelectedIndex - 1].ToString(), out DateTime expirationDate))
                    //        {
                    //            item.ExpirationDate = expirationDate;
                    //        }
                    //        else
                    //        {
                    //            item.ExpirationDate = DateTime.Now;
                    //        }

                    //        item.BarCode = "";


                    //        var ResponseUpload = itemModel.UploadExcel(item);
                    //        if (ResponseUpload != null)
                    //        {
                    //            ErrorItemList.Add(ResponseUpload);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ErrorItemList.Add(item);
                    //    }
                    //    finally
                    //    {
                    //        progressCount++;
                    //        progressBar1.Value = progressCount;
                    //    }
                    //}
                    //if (ErrorItemList.Count > 0)
                    //{
                    //    MessageBox.Show("Item uploaded! but some items does not uploaded, Please check the Item information.", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    FrmNotUploaded frm = new FrmNotUploaded(ErrorItemList);
                    //    if (frm.ShowDialog() == DialogResult.OK)
                    //    {
                    //        this.DialogResult = DialogResult.OK;
                    //    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Item Uploaded!", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    this.DialogResult = DialogResult.OK;
                    //}

                }
            }
        }

        private bool CheckIfColumnSet()
        {
            bool isSet = true;
            if(cbo1.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo2.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo3.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo4.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo5.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo6.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo7.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo8.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo9.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo10.SelectedIndex == 0)
            {
                isSet = false;
            }
            else if (cbo11.SelectedIndex == 0)
            {
                isSet = false;
            }
            return isSet;
        }

        private bool CheckIfColumnSetSame()
        {
            bool isSet = true;
            if (cbo1.SelectedIndex == cbo2.SelectedIndex ||
                cbo1.SelectedIndex == cbo3.SelectedIndex ||
                cbo1.SelectedIndex == cbo4.SelectedIndex ||
                cbo1.SelectedIndex == cbo5.SelectedIndex ||
                cbo1.SelectedIndex == cbo6.SelectedIndex ||
                cbo1.SelectedIndex == cbo7.SelectedIndex ||
                cbo1.SelectedIndex == cbo8.SelectedIndex ||
                cbo1.SelectedIndex == cbo9.SelectedIndex ||
                cbo1.SelectedIndex == cbo10.SelectedIndex ||
                cbo1.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo2.SelectedIndex == cbo3.SelectedIndex ||
                cbo2.SelectedIndex == cbo4.SelectedIndex ||
                cbo2.SelectedIndex == cbo5.SelectedIndex ||
                cbo2.SelectedIndex == cbo6.SelectedIndex ||
                cbo2.SelectedIndex == cbo7.SelectedIndex ||
                cbo2.SelectedIndex == cbo8.SelectedIndex ||
                cbo2.SelectedIndex == cbo9.SelectedIndex ||
                cbo2.SelectedIndex == cbo10.SelectedIndex ||
                cbo2.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo3.SelectedIndex == cbo4.SelectedIndex ||
                cbo3.SelectedIndex == cbo5.SelectedIndex ||
                cbo3.SelectedIndex == cbo6.SelectedIndex ||
                cbo3.SelectedIndex == cbo7.SelectedIndex ||
                cbo3.SelectedIndex == cbo8.SelectedIndex ||
                cbo3.SelectedIndex == cbo9.SelectedIndex ||
                cbo3.SelectedIndex == cbo10.SelectedIndex ||
                cbo3.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo4.SelectedIndex == cbo5.SelectedIndex ||
                cbo4.SelectedIndex == cbo6.SelectedIndex ||
                cbo4.SelectedIndex == cbo7.SelectedIndex ||
                cbo4.SelectedIndex == cbo8.SelectedIndex ||
                cbo4.SelectedIndex == cbo9.SelectedIndex ||
                cbo4.SelectedIndex == cbo10.SelectedIndex ||
                cbo4.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo5.SelectedIndex == cbo6.SelectedIndex ||
                cbo5.SelectedIndex == cbo7.SelectedIndex ||
                cbo5.SelectedIndex == cbo8.SelectedIndex ||
                cbo5.SelectedIndex == cbo9.SelectedIndex ||
                cbo5.SelectedIndex == cbo10.SelectedIndex ||
                cbo5.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo6.SelectedIndex == cbo7.SelectedIndex ||
                cbo6.SelectedIndex == cbo8.SelectedIndex ||
                cbo6.SelectedIndex == cbo9.SelectedIndex ||
                cbo6.SelectedIndex == cbo10.SelectedIndex ||
                cbo6.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo7.SelectedIndex == cbo8.SelectedIndex ||
                cbo7.SelectedIndex == cbo9.SelectedIndex ||
                cbo7.SelectedIndex == cbo10.SelectedIndex ||
                cbo7.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (
                cbo8.SelectedIndex == cbo9.SelectedIndex ||
                cbo8.SelectedIndex == cbo10.SelectedIndex ||
                cbo8.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (cbo9.SelectedIndex == cbo10.SelectedIndex ||
                cbo9.SelectedIndex == cbo11.SelectedIndex)
            {
                isSet = false;
            }
            else if (cbo10.SelectedIndex == cbo11.SelectedIndex )
            {
                isSet = false;
            }
            return isSet;
        }

        private void FrmUploadExcel_Load(object sender, EventArgs e)
        {

        }


    }
}
