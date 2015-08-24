using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprocIt
{
	public class Sproc
	{

		public string ConnectionString { get; set; }

        public string StoredProcedure { get; set; }

		private List<SqlParameter> SqlParameters_;
        public List<SqlParameter> SqlParameters
		{
			get
			{
				return SqlParameters_ ?? (SqlParameters_ = new List<SqlParameter>());
			}

			set
			{
				SqlParameters_ = value;
			}
		}

		// Transforms are not being applied yet.
		private static Func<string, string> DefaultParameterNameTransform = parameterName => parameterName;
        private Func<string, string> TransformParameterName_;
        public Func<string, string> TransformParameterName
		{
			get
			{
				return TransformParameterName_ ?? DefaultParameterNameTransform;
			}

			set
			{
				TransformParameterName_ = value;
			}
		}


		public Sproc In(string parameterName, object value)
		{
			SqlParameters.Add(new SqlParameter(parameterName, value));
			return this;
		}


		public Sproc Out(string parameterName, object value)
		{
			SqlParameters.Add(new SqlParameter(parameterName, value) { Direction = ParameterDirection.Output });
			return this;
		}


		public Sproc Custom(SqlParameter sqlParameter)
		{
			SqlParameters.Add(sqlParameter);
			return this;
		}


		private T Use<T>(Func<SqlCommand, T> func)
		{
			using (var sqlConnection = new SqlConnection(ConnectionString))
			using (var sqlCommand = new SqlCommand(StoredProcedure, sqlConnection) { CommandType = CommandType.StoredProcedure })
			{
				SqlParameters.ForEach(sqlParameter => sqlCommand.Parameters.Add(sqlParameter));
				sqlConnection.Open();
				return func(sqlCommand);
			}
		}


		public int NonQuery()
		{
			return Use(sqlCommand => sqlCommand.ExecuteNonQuery());
		}


		public object GetScalar()
		{
			return Use(sqlCommand => sqlCommand.ExecuteScalar());
		}


		public DataSet GetDataSet()
		{
			return Use(sqlCommand =>
			{
				using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
				{
					DataSet dataSet = new DataSet();
					sqlDataAdapter.Fill(dataSet);
					return dataSet;
				}
			});
		}


		public IEnumerable<SqlParameter> GetOutputParameters()
		{
			Use(sqlCommand => sqlCommand.ExecuteNonQuery());
			return SqlParameters.Where(sqlParameter => sqlParameter.Direction == ParameterDirection.Output || sqlParameter.Direction == ParameterDirection.InputOutput);
		}


	}
}