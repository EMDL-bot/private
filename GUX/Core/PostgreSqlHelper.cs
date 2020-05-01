using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Core
{
    public class PostgreSqlHelper : IDisposable
    {
        private NpgsqlConnection connection = null;
        private String connectionString = "dbConnection";
        private NpgsqlDataAdapter sqlData = new NpgsqlDataAdapter();
        public NpgsqlCommand sqlCmd = new NpgsqlCommand();

        private String connectionFormat = "Server={0};Database={1};User Id={2};Password={3};Port={4}";

        public String ConnectionString
        {
            set { connectionString = value; }
        }

        #region Constructors

        public PostgreSqlHelper()
        {
        }

        public PostgreSqlHelper(string pConnectionString)
        {
            if (!string.IsNullOrEmpty(pConnectionString.Trim()))
            {
                connectionString = pConnectionString;
            }
        }

        public PostgreSqlHelper(string pIP, string pDbName, string pUserID, string pUserPW, string pPort)
        {
            connectionString = string.Format(this.connectionFormat, pIP, pDbName, pUserID, pUserPW, pPort);
        }

        public PostgreSqlHelper(string pIP, string pDbName, string pUserID, string pUserPW)
        {
            connectionString = string.Format(this.connectionFormat, pIP, pDbName, pUserID, pUserPW, "5432");
        }

        #endregion

        public string CmdText
        {
            set { sqlCmd.CommandText = value; }
        }

        public CommandType cmdType
        {

            set { sqlCmd.CommandType = value; }
        }

        public void AddParameter(string Param, NpgsqlDbType Db_Type, object values, ParameterDirection Param_io, int nSize)
        {
            NpgsqlParameter param1 = new NpgsqlParameter(Param, Db_Type);
            if (Param_io == ParameterDirection.Input || Param_io == ParameterDirection.InputOutput)
                param1.Value = values;
            param1.Direction = Param_io;
            param1.Size = nSize;
            sqlCmd.Parameters.Add(param1);
        }

        #region Add Parameter TO Query

        public void AddParameter(string Param, NpgsqlDbType Db_Type, object values)
        {
            NpgsqlParameter param1 = new NpgsqlParameter(Param, Db_Type);
            param1.Value = values;
            sqlCmd.Parameters.Add(param1);
        }

        public void AddParameter(string Param, NpgsqlDbType Db_Type, object values, ParameterDirection Param_io)
        {
            NpgsqlParameter param1 = new NpgsqlParameter(Param, Db_Type);
            if (Param_io == ParameterDirection.Input || Param_io == ParameterDirection.InputOutput)
                param1.Value = values;
            param1.Direction = Param_io;
            sqlCmd.Parameters.Add(param1);
        }

        public object GetParameterValue(NpgsqlCommand command, string parameterName)
        {
            return command.Parameters[parameterName].Value;
        }

        public object GetParameter(string param)
        {
            return sqlCmd.Parameters[param].Value;
        }

        #endregion

        #region Generating SqlCommand

        private NpgsqlCommand PrepareCommand(CommandType commandType, string commandText)
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(this.connectionString);
            }
            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                connection.Open();
            }
            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);
            command.CommandType = commandType;
            return command;
        }

        public NpgsqlCommand GetStoreProcedureCommand(string spname)
        {
            return PrepareCommand(CommandType.StoredProcedure, spname);
        }

        public NpgsqlCommand GetSqlQueryCommand(string query)
        {
            return PrepareCommand(CommandType.Text, query);
        }
        #endregion

        #region Direct Quer

        public int DirectNonQuery(string query)
        {
            NpgsqlCommand sc = GetSqlQueryCommand(query);
            int iResult = ExecuteNonQuery(sc);
            return iResult;
        }


        public DataTable DirectQuery(string query)
        {
            NpgsqlCommand sc = GetSqlQueryCommand(query);
            return LoadDataTable(sc, string.Empty);
        }

        public DataTable DirectQuery(string query, string tableName)
        {
            NpgsqlCommand sc = GetSqlQueryCommand(query);
            return LoadDataTable(sc, tableName);
        }

        public int StringNonQuery(string sQuery)
        {
            return new PostgreSqlHelper().DirectNonQuery(sQuery);
        }
        #endregion

        #region Database Related Command

        public int ExecuteNonQuery(NpgsqlCommand command)
        {
            return command.ExecuteNonQuery();
        }

        public int ExcuteNonQuery()
        {
            return sqlExcuteNonQuery(0);
        }

        public int sqlExcuteNonQuery(int Transaction)
        {
            try
            {
                sqlData.SelectCommand = sqlCmd;
                sqlCmd.Connection = new NpgsqlConnection(this.connectionString);
                sqlCmd.Connection.Open();
                if (Transaction > 0)
                    sqlCmd.Transaction = sqlCmd.Connection.BeginTransaction(IsolationLevel.ReadCommitted);

                int rtn = sqlCmd.ExecuteNonQuery();

                if (Transaction > 0)
                    sqlCmd.Transaction.Commit();
                sqlCmd.Connection.Close();

                return rtn;
            }
            catch
            {
                if (Transaction > 0)
                    sqlCmd.Transaction.Rollback();

                return -1;
            }
            finally
            {
            }
        }

        public object ExecuteScalar(NpgsqlCommand command)
        {
            return command.ExecuteScalar();
        }

        public NpgsqlDataReader ExecuteReader(NpgsqlCommand command)
        {
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public NpgsqlDataReader ExecuteReader(NpgsqlCommand command, CommandBehavior commandBehavior)
        {
            return command.ExecuteReader(commandBehavior);
        }

        public DataTable LoadDataTable(NpgsqlCommand command, string tableName)
        {
            try
            {
                using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(command))
                {
                    using (DataTable dt = new DataTable(tableName))
                    {
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                return null;
            }
        }

        public DataSet LoadDataSet(NpgsqlCommand command, string[] tableNames)
        {
            using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(command))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    if (tableNames != null)
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            try
                            {
                                ds.Tables[i].TableName = tableNames[i];
                            }
                            catch
                            {
                            }
                        }
                    }

                    return ds;
                }
            }
        }

        public DataSet LoadDataSet()
        {
            //
            DataSet ds = new DataSet();
            try
            {
                sqlData.SelectCommand = sqlCmd;
                sqlCmd.Connection = new NpgsqlConnection(this.connectionString);

                sqlData.Fill(ds);

                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }

        private NpgsqlTransaction PrepareTransaction(IsolationLevel isolationLevel)
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(this.connectionString);
            }
            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                connection.Open();
            }
            return connection.BeginTransaction(isolationLevel);
        }

        public NpgsqlTransaction BeginTransaction()
        {
            return PrepareTransaction(IsolationLevel.ReadCommitted);
        }

        public NpgsqlTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return PrepareTransaction(isolationLevel);
        }

        public void Commit(NpgsqlTransaction transaction)
        {
            if (transaction != null)
                transaction.Commit();
        }

        public void RollBack(NpgsqlTransaction transaction)
        {
            if (transaction != null)
                transaction.Rollback();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Destructor

        ~PostgreSqlHelper()
        {
            Dispose();
        }
        #endregion

        void IDisposable.Dispose()
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
