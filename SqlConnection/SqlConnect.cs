using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace SqlConnection
{
    public class SqlConnectBase : IDisposable
    {
        private string _connectString;
        MySqlConnection _sqlConnect;

        public SqlConnectBase(string ip, string user, string password, string database)
        {
            try
            {
                _connectString = "server=" + ip
                        + ";uid=" + user
                        + ";pwd=" + password
                        + ";database=" + database
                        + ";max pool size=1510";
                _sqlConnect = new MySqlConnection(_connectString);

                if (_sqlConnect.State != ConnectionState.Open)
                    _sqlConnect.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
                    using (MySqlCommand cmd = new MySqlCommand(sqlCommand, _sqlConnect))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex) { }

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
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            for (int c = 0; c < dr.FieldCount; c++)
                            {
                                dataTable.Columns.Add(dr.GetName(c));
                            }

                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    dataTable.Rows.Add();

                                    for (int c = 0; c < dr.FieldCount; c++)
                                    {
                                        if (dr.GetFieldType(c) == typeof(DateTime))
                                            dataTable.Rows[dataTable.Rows.Count - 1][c] = Convert.ToDateTime(dr[dr.GetName(c)]).ToString("yyyy-MM-dd HH:mm:ss");
                                        else
                                            dataTable.Rows[dataTable.Rows.Count - 1][c] = Convert.ToString(dr[dr.GetName(c)]);
                                    }
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
