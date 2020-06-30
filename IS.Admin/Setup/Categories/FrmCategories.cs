﻿using IS.Admin.Model;
using IS.Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IS.Admin.Setup
{
    public partial class FrmCategories : Form
    {
        public FrmCategories()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAddCategory frm = new FrmAddCategory();
            frm.ShowDialog();
            this.LoadCategory();

        }

        private void LoadCategory()
        {
            grpLoading.Visible = true;
            grpLoading.Refresh();

            CategoriesModel Categories = new CategoriesModel();
            var response = Categories.CategoryList(this, txtSearch.Text);
            dgvSearch.AutoGenerateColumns = false;
            dgvSearch.DataSource = response;
            txtSearch.Focus();

            grpLoading.Visible = false;
            grpLoading.Refresh();
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var Category = new Categories
            {
                CategoryId = dgvSearch.CurrentRow.Cells[0].Value.ToString(),
                CategoryName = dgvSearch.CurrentRow.Cells[1].Value.ToString(),
            };

            if (e.ColumnIndex == 2)
            {
                FrmEditCategory frm = new FrmEditCategory(Category);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Record updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.LoadCategory();
                };

                //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");
            }
            if (e.ColumnIndex == 3)
            {
                var model = new CategoriesModel();
                if (model.CheckCategoryIfAlreadyInUse(Category.CategoryId))
                {
                    MessageBox.Show("You can not delete " + Category  + " because this Category already in use", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (MessageBox.Show("Are you sure do want to delete " + Category.CategoryName + ".", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {

                        model.DeleteCategory(Category);
                        this.LoadCategory();
                        MessageBox.Show(Category.CategoryName + " deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmCategories_Shown(object sender, EventArgs e)
        {
            this.LoadCategory();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            FrmCategoriesUploadExcel frm = new FrmCategoriesUploadExcel();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.LoadCategory();

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCategory();
        }
    }
}
