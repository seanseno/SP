﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using IS.Database.Models;
using System.Linq;

namespace IS.Database.Strategy
{
    public class ProductsStrategy : Helper
    {
        ISFactory factory = new ISFactory();
        public bool CheckDuplicate(string ProductId)
        {
            return factory.ProductsRepository.GetList()
                .Where(x => x.ProductId.Contains(ProductId)).ToList().Count() > 0;
        }


        public bool CheckEditDuplicate(string Description, int? itemId)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT BrandName FROM items WHERE Description = '" + Description + "'  AND ID != " + itemId;
                using (SqlCommand cmd = new SqlCommand(select, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
        public bool ItemAlreadyInUse(string ProductId)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = " SELECT ProductId FROM Sales " +
                            " WHERE ProductId = '" + ProductId + "'" +
                            " UNION " +
                            " SELECT ProductId FROM TempSales " +
                            " WHERE ProductId = '" + ProductId + "'";

                using (SqlCommand cmd = new SqlCommand(select, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public bool CheckIfProductExist(string ProductId)
        {
            return factory.ProductsRepository.GetList().Where(x=>x.ProductId == ProductId).Count() > 0 ;
        }

        public ProductDiscounted GetDiscountInfo(string ProductId, int Qty,bool isSenior)
        {
            var PD = new ProductDiscounted();

            var set = factory.SettingsRepository.GetList().FirstOrDefault();
            var prod = factory.ProductsRepository.GetList().Where(x => x.ProductId == ProductId).FirstOrDefault();

            PD.ProductId = ProductId;
            PD.Qty = Qty;
            PD.Price = prod.Price;

            if (!isSenior)
            {
                PD.Discounted = (Qty * prod.Price) * ((set.PWDDiscount) / 100);
                PD.TotalPrice = (Qty * prod.Price) - PD.Discounted;
                PD.PriceDiscounted = (prod.Price) - (prod.Price * ((set.PWDDiscount) / 100));
                return PD;
            }
            else
            {
                PD.Discounted = (Qty * prod.Price) * ((set.SeniorDiscount) / 100);
                PD.TotalPrice = (Qty * prod.Price) - PD.Discounted;
                PD.PriceDiscounted = (prod.Price) - (prod.Price * ((set.SeniorDiscount) / 100));
                return PD;
            }
        }

        public ProductDiscounted GetPromoProduct(string ProductId, int PromoId, int Qty)
        {
            var PD = new ProductDiscounted();
            var prod = factory.ProductsRepository.GetList().Where(x => x.ProductId == ProductId).FirstOrDefault();
            var promo = factory.PromoDetailsRepository.GetList().Where(x => x.ProductId == ProductId && x.PromoId == PromoId).FirstOrDefault();

            PD.ProductId = ProductId;
            PD.Qty = Qty;
            PD.Price = prod.Price;

            PD.Discounted = (prod.Price - promo.Price) * Qty;
            PD.TotalPrice = (Qty * promo.Price);
            PD.PriceDiscounted = promo.Price;

            return PD;
        }
    }
}
