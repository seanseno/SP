﻿using IS.Common.Utilities;
using IS.Database.Entities;
using IS.Database.Enums;
using IS.Database.Strategy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace IS.Database.Repositories
{
    public class RequestOrderItemsRepository : Helper
    {
        public void Insert(RequestOrderItems model)
        {

        }

        public void Update(RequestOrderItems model)
        {
          
        }

        public void Delete(RequestOrderItems model)
        {

        }

        public IList<RequestOrderItems> GetList()
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT * From RequestOrderItems" +
                            " ORDER BY OrderDate DESC ";
                using (SqlCommand cmd = new SqlCommand(select, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<RequestOrderItems> Items = new List<RequestOrderItems>();
                        while (reader.Read())
                        {
                            var item = new RequestOrderItems
                            {
                                Id = reader.GetInt32(0),
                                AdministratorId = reader.GetInt32(1),
                                RequestOrderName = reader.GetString(2),
                                OrderDate = reader.GetDateTime(3),
                                InsertTime = reader.GetDateTime(4),
                            };

                            Items.Add(item);
                        }
                        return Items;
                    }
                }
            }
        }

        public RequestOrderItems GetOrderRequestInfoWithId(int Id)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT A.Fullname,R.OrderDate From RequestOrderItems as R" +
                            "   LEFT JOIN Administrators as A on A.Id =R.AdministratorId " +
                            " WHERE R.Id = " + Id;
                using (SqlCommand cmd = new SqlCommand(select, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        
                        if(reader.HasRows)
                        {
                            reader.Read();
                            var item = new RequestOrderItems
                            {
                                AdminName = reader.GetString(0),
                                OrderDate = reader.GetDateTime(1),
                            };
                            return item;
                        }
   
                        return null;
                    }
                }
            }
        }
        public RequestOrderItemsStrategy RequestOrderItemsStrategy => new RequestOrderItemsStrategy();
    }
}