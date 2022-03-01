using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace MySQL_Demo
{
    public class SqlConnect : IDisposable
    {
        private string _connectString;
        MySqlConnection _sqlConnect;

        public SqlConnect(string ip, string user, string password)
        {
            try
            {
                _connectString = "server=" + ip
                        + ";uid=" + user
                        + ";pwd=" + password
                        + ";database=ds"
                        + ";max pool size=1510";
                _sqlConnect = new MySqlConnection(_connectString);

                if (_sqlConnect.State != ConnectionState.Open)
                    _sqlConnect.Open();
            }
            catch
            { }
        }

        public bool IsConnected
        {
            get
            {
                if (_sqlConnect.State != ConnectionState.Open)
                {
                    _sqlConnect = new MySqlConnection(_connectString);
                    _sqlConnect.Open();
                }

                return _sqlConnect.State == ConnectionState.Open;
            }
        }

        protected MySqlConnection sqlConnect
        {
            get
            {
                if (IsConnected && _sqlConnect.Ping())
                    return _sqlConnect;
                else
                {
                    _sqlConnect = new MySqlConnection(_connectString);
                    _sqlConnect.Open();
                    return _sqlConnect;
                }
            }
            set
            {
                _sqlConnect = value;
            }
        }

        public void Dispose()
        {
            if (sqlConnect != null) sqlConnect.Dispose();
        }

        protected virtual bool SetData(string sqlCommand)
        {
            try
            {
                if (IsConnected)
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch { }

            return false;
        }

        protected virtual bool GetData(string sqlCommand, out DataTable dataTable)
        {
            dataTable = new DataTable();

            try
            {
                if (IsConnected)
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCommand, _sqlConnect))
                    {
                        Console.WriteLine("3");
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            Console.WriteLine($"{dr.FieldCount}");
                            for (int c = 0; c < dr.FieldCount; c++)
                            {
                                dataTable.Columns.Add(dr.GetName(c));
                            }

                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    Console.WriteLine("hasRow");
                                    dataTable.Rows.Add();

                                    Console.WriteLine($"dr.FileCOunt: {dr.FieldCount}");
                                    for (int c = 0; c < dr.FieldCount; c++)
                                    {
                                        if (dr.GetFieldType(c) == typeof(DateTime))
                                            dataTable.Rows[dataTable.Rows.Count - 1][c] = Convert.ToDateTime(dr[dr.GetName(c)]).ToString("yyyy-MM-dd HH:mm:ss");
                                        else
                                            dataTable.Rows[dataTable.Rows.Count - 1][c] = Convert.ToString(dr[dr.GetName(c)]);
                                    }
                                    Console.WriteLine($"end");
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            catch
            {

            }

            return false;
        }
    }
}
