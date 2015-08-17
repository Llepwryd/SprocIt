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

		public List<SqlParameter> SqlParameters { get; set; }

		public ParameterNameSettings ParameterNameSettings { get; set; }


		public Sproc Initialize()
		{
			if (SqlParameters == null)
				SqlParameters = new List<SqlParameter>();

			if (ParameterNameSettings == null)
				ParameterNameSettings = new ParameterNameSettings();

			return this;
		}


		public Sproc Transform()
		{
			ParameterNameSettings.ShouldTransformParameterName = true;
			return this;
		}


		public Sproc Prefix()
		{
			ParameterNameSettings.ShouldPrefixParameterName = true;
			return this;
		}


		private string ConditionallyPrefixParameterName(string parameterName)
		{
			return ParameterNameSettings.ShouldPrefixParameterName ? ParameterNameSettings.ParameterNamePrefix + parameterName : parameterName;
		}


		private string ConditionallyTransformParameterName(string parameterName)
		{
			return ParameterNameSettings.ShouldTransformParameterName ? ParameterNameSettings.TransformParameterName(parameterName) : parameterName;
		}


		private string ConditionallyAlterParameterName(string parameterName)
		{
			return ParameterNameSettings.ShouldTransformParameterNameBeforePrefixing ?
				ConditionallyPrefixParameterName(ConditionallyTransformParameterName(parameterName)) :
				ConditionallyTransformParameterName(ConditionallyPrefixParameterName(parameterName));
		}


		public Sproc In(string parameterName, object value)
		{
			SqlParameters.Add(new SqlParameter(ConditionallyAlterParameterName(parameterName), value));
			return this;
		}


		public Sproc Out(string parameterName, object value)
		{
			SqlParameters.Add(new SqlParameter(ConditionallyAlterParameterName(parameterName), value) { Direction = System.Data.ParameterDirection.Output });
			return this;
		}


		public Sproc Custom(SqlParameter sqlParameter)
		{
			SqlParameters.Add(sqlParameter);
			return this;
		}


		private void AddSqlParametersToSqlCommand(SqlCommand sqlCommand)
		{
			if (SqlParameters != null)
				SqlParameters.ForEach(sqlParameter => sqlCommand.Parameters.Add(sqlParameter));
		}


		public DataSet GetDataSet(string pattern = null, string replacement = null)
		{
			DataSet dataSet = new DataSet();

			using (var sqlConnection = new SqlConnection(ConnectionString))
			using (var sqlCommand = new SqlCommand(StoredProcedure, sqlConnection) { CommandType = CommandType.StoredProcedure })
			{

				AddSqlParametersToSqlCommand(sqlCommand);

				using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
				{
					sqlConnection.Open();
					sqlDataAdapter.Fill(dataSet);
				}
			}

			return dataSet;
		}



		public IEnumerable<SqlParameter> GetOutputParameters(string pattern = null, string replacement = null)
		{
			using (var sqlConnection = new SqlConnection(ConnectionString))
			using (var sqlCommand = new SqlCommand(StoredProcedure, sqlConnection) { CommandType = CommandType.StoredProcedure })
			{
				AddSqlParametersToSqlCommand(sqlCommand);
				sqlConnection.Open();
				sqlCommand.ExecuteNonQuery();
			}

			return SqlParameters.Where(sqlParameter => sqlParameter.Direction == ParameterDirection.Output || sqlParameter.Direction == ParameterDirection.InputOutput);
		}

	}
}