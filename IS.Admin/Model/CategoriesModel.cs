﻿using IS.Admin.Setup;
using IS.Database;
using IS.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Admin.Model
{
    public class CategoriesModel
    {
        public IList<Categories> CategoryList(FrmCategories frm, string Keywords)
        {
            var factory = new ISFactory();
            return factory.CategoriesRepository.Find(Keywords);
        }
        public IList<Categories> CategoryListWithSelect()
        {
            var factory = new ISFactory();
            return factory.CategoriesRepository.FindWithSelect();
        }

        public void AddCategory(FrmAddCategory frm)
        {
            var factory = new ISFactory();
            factory.CategoriesRepository.Insert(frm._Categories);
        }
        public bool CheckDup(FrmAddCategory frm)
        {
           var factory = new ISFactory();
           return factory.CategoriesRepository.CategoriesStrategy.CheckDuplicate(frm._Categories.CategoryName);
        }
        public bool CheckEditDup(string name, int? CategoryId)
        {
            var factory = new ISFactory();
            return factory.CategoriesRepository.CategoriesStrategy.CheckEditDuplicate(name, CategoryId);
        }
        public void UpdateCategory(Categories Category)
        {
            var factory = new ISFactory();
            factory.CategoriesRepository.Update(Category);
        }

        public void DeleteCategory(Categories Category)
        {
            var factory = new ISFactory();
            factory.CategoriesRepository.Delete(Category);
        }
        public Categories LoadEdit(int? CategoryId)
        {
            var factory = new ISFactory();
            return factory.CategoriesRepository.FindWithId(CategoryId);
        }

        public bool CheckCategoryIfAlreadyInUse(int? CategoryId)
        {
            var factory = new ISFactory();
            return factory.CategoriesRepository.CategoriesStrategy.CategoryAlreadyInUse(CategoryId);
        }
    }
}