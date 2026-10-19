using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Npgsql;

namespace DataAccessLayer
{
    public class CRUD
    {
        public static List<ProductModel> GetAllData()
        {
            const string Query = @"SELECT ""Id"" AS ""PId"", ""Name"" AS ""PName"", ""Price"" FROM ""Products""";

            List<ProductModel> products = new List<ProductModel>();

            NpgsqlConnection connection = DBHelper.GetConnection();
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) { 
            ProductModel model = new ProductModel();
                model.PId = Convert.ToInt32(reader["PId"]);
                model.PName = reader["PName"].ToString();
                model.Price = Convert.ToInt32(reader["Price"]);
                products.Add(model);

            }
            connection.Close();
            return products;
        }

        public static bool AddProduct(ProductModel product)
        {
            const string Query = @"INSERT INTO ""Products"" (""Name"", ""Price"") VALUES (@Name, @Price)";

            using (NpgsqlConnection connection = DBHelper.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.PName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteProduct(int id)
        {
            const string Query = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id";

            using (NpgsqlConnection connection = DBHelper.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool UpdateProduct(ProductModel product)
        {
            const string Query = @"UPDATE ""Products"" SET ""Name"" = @Name, ""Price"" = @Price WHERE ""Id"" = @Id";

            using (NpgsqlConnection connection = DBHelper.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.PName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Id", product.PId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
