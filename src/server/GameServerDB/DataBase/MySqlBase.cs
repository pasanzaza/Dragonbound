using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;

namespace GameServerDB
{
    public class MySqlBase
    {
        MySqlConnection Connection;
        MySqlDataReader SqlData;

        public int RowCount { get; set; }

        public void Init(string host, string user, string password, string database, int port)
        {
            Connection = new MySqlConnection("Server=" + host + ";User Id=" + user + ";Port=" + port + ";" + 
                                             "Password=" + password + ";Database=" + database + ";Allow Zero Datetime=True");

            try
            {
                Connection.Open();
                LogConsole.Show("Successfully connected to {0}:{1}:{2}", host, port, database);
            }
            catch (MySqlException ex)
            {
                LogConsole.Show("{0}", ex.Message);

                // Try auto reconnect on error (every 5 seconds)
                LogConsole.Show("Try reconnect in 5 seconds...");
                Thread.Sleep(5000);

                Init(host, user, password, database, port);
            }
        }

        public bool Execute(string sql, params object[] args)
        {
            StringBuilder sqlString = new StringBuilder();
            // Fix for floating point problems on some languages
            sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql);

            MySqlCommand sqlCommand = new MySqlCommand(sqlString.ToString(), Connection);

            try
            {
                List<MySqlParameter> mParams = new List<MySqlParameter>(args.Length);

                foreach (var a in args)
                    mParams.Add(new MySqlParameter("", a));

                sqlCommand.Parameters.AddRange(mParams.ToArray());
                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                LogConsole.Show("{0}", ex.Message);
                return false;
            }
        }

        public SQLResult Select(string sql, params object[] args)
        {
            SQLResult retData = new SQLResult();

            StringBuilder sqlString = new StringBuilder();
            // Fix for floating point problems on some languages
            sqlString.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, sql);

            MySqlCommand sqlCommand = new MySqlCommand(sqlString.ToString(), Connection);
            
            try
            {
                List<MySqlParameter> mParams = new List<MySqlParameter>(args.Length);

                foreach (var a in args)
                    mParams.Add(new MySqlParameter("", a));

                sqlCommand.Parameters.AddRange(mParams.ToArray());
                SqlData = sqlCommand.ExecuteReader(CommandBehavior.Default);
                retData.Load(SqlData);
                retData.Count = retData.Rows.Count;
                SqlData.Close();
            }
            catch (MySqlException ex)
            {
                LogConsole.Show("{0}", ex.Message);
            }

            return retData;
        }

        public void ExecuteBigQuery(string table, string fields, int fieldCount, int resultCount, object[] values, bool resultAsIndex = true)
        {
            if (values.Length > 0)
            {
                StringBuilder sqlString = new StringBuilder();

                sqlString.AppendFormat("INSERT INTO {0} ({1}) VALUES ", table, fields);

                for (int i = 0; i < resultCount; i++)
                {
                    sqlString.AppendFormat("(");

                    for (int j = 0; j < fieldCount; j++)
                    {
                        int index = resultAsIndex ? i : j;

                        if (j == fieldCount - 1)
                            sqlString.Append(String.Format(CultureInfo.GetCultureInfo("en-US").NumberFormat, "'{0}'", values[index]));
                        else
                            sqlString.Append(String.Format(CultureInfo.GetCultureInfo("en-US").NumberFormat, "'{0}', ", values[index]));
                    }

                    if (i == resultCount - 1)
                        sqlString.AppendFormat(");");
                    else
                        sqlString.AppendFormat("),");
                }

                MySqlCommand sqlCommand = new MySqlCommand(sqlString.ToString(), Connection);
                sqlCommand.ExecuteNonQuery();
            }

            return;
        }
    }
}
