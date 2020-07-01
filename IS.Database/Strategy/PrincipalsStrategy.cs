﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using IS.Common.Utilities;

namespace IS.Database.Strategy
{
    public class PrincipalsStrategy : Helper
    {
        public bool CheckDuplicate(string PrincipalId,string PrincipalName)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT PrincipalName FROM vPrincipals WHERE PrincipalId = '" + PrincipalId + "' OR PrincipalName = '" + SingleQuoteCorrection.convert(PrincipalName) + "'";
                using (SqlCommand cmd = new SqlCommand(select,connection))
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


        public bool CheckEditDuplicate(string Name, int? PrincipalId)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT PrincipalName FROM vPrincipals WHERE PrincipalName = '" + Name + "' AND ID != " + PrincipalId;
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

        public bool PrincipalAlreadyInUse(string PrincipalId)
        {
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                var select = "SELECT * FROM vItems WHERE PrincipalId  = '" + PrincipalId + "'";
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
    }
}