using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace DataAccessLayer
{
    public class DBHelper
    {
        public static NpgsqlConnection GetConnection() {
            return new NpgsqlConnection("Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.gnfwvblwkdpyyglfipra;Password=FULLSTACK@123;SSL Mode=Require;Trust Server Certificate=true");
        }
    }
}
