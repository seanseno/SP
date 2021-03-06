﻿using IS.Admin.Setup;
using IS.Database;
using IS.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Admin.Model
{
    public class ProductsModel
    {
        public IList<Products> ItemList(string Keywords)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.Find(Keywords);
        }

        public Products FindWithProductId(string ProductId)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.FindWithProductId(ProductId);
        }
        public void AddItem(FrmAddProduct frm)
        {
            var factory = new ISFactory();

            //INSERT ITEM
            factory.ProductsRepository.Insert(frm._Products);
        }
        public bool CheckDup(string ProductId)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.ProductsStrategy.CheckDuplicate(ProductId);
        }
        public bool CheckEditDup(string name, int? itemId)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.ProductsStrategy.CheckEditDuplicate(name, itemId);
        }

        public Products LoadEdit(string itemId)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.FindWithProductId(itemId);
        }

        public bool CheckItemIfAlreadyInUse(string itemId)
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.ProductsStrategy.ItemAlreadyInUse(itemId);
        }


        public void InsertItem(Products model)
        {
            var factory = new ISFactory();
            factory.ProductsRepository.Insert(model);
        }
        public string GetNextId()
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.GetNextId();
        }
        public int GetTotalCount()
        {
            var factory = new ISFactory();
            return factory.ProductsRepository.GetTotalCount();
        }
    }
}
