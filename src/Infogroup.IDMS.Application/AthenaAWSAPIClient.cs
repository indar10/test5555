using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Athena;
using Amazon.Athena.Model;

namespace Infogroup.IDMS
{
    public class AthenaAPIClient
    {
        public AmazonAthenaClient _client = null;

        public string Database_Name { get; private set; }
        public string Output_Location { get; private set; }

        public AthenaAPIClient()
        {
            _client = LoadConfiguration();
        }

        public void ExecuteNonQuery(string sql)
        {
            RunSQL(sql, "N");
        }

        public DataTable ExecuteQuery(string sql)
        {
            return RunSQL(sql, "Q");
        }
        public DataTable RunSQL(string sql, string querytype)
        {
            var req = new StartQueryExecutionRequest();
            var qe = new QueryExecutionContext();
            var rc = new ResultConfiguration();
            rc.OutputLocation = Output_Location;
            qe.Database = Database_Name;
            req.QueryString = sql;
            req.QueryExecutionContext = qe;
            req.ResultConfiguration = rc;
            //var result = _client.StartQueryExecution(req);
            var result = _client.StartQueryExecutionAsync(req);

            //Wait for Query to Finish.
            Wait(result.Result.QueryExecutionId);

            if (querytype == "N")
                return null;

            //Get Result if Query was successful
            return GetResult(result.Result.QueryExecutionId);
        }

        public DataTable GetResult(string queryExecutionId)
        {
            var getQueryResultsRequest = new GetQueryResultsRequest() { QueryExecutionId = queryExecutionId };
            var getQueryResultSet = _client.GetQueryResultsAsync(getQueryResultsRequest).Result.ResultSet;
            var columnInfoList = getQueryResultSet.ResultSetMetadata.ColumnInfo;
            var rows = getQueryResultSet.Rows;
            return ProcessRow(rows, columnInfoList);
        }

        public DataTable ProcessRow(List<Row> rows, List<ColumnInfo> columnInfoList)
        {
            var table = new DataTable();

            var columns = new List<string>();
            foreach (var columnInfo in columnInfoList)
            {
                table.Columns.Add(columnInfo.Name);
            }

            bool firstRow = true;
            foreach (var row in rows)
            {
                if (firstRow)
                {
                    firstRow = false;
                    continue;
                }

                var tr = table.NewRow();
                int index = 0;
                foreach (var datum in row.Data)
                {
                    tr[index] = datum.VarCharValue;
                    index++;
                }
                table.Rows.Add(tr);
            }

            return table;
        }

        public void Wait(string queryExecutionId)
        {
            GetQueryExecutionRequest getQueryExecutionRequest = new GetQueryExecutionRequest() { QueryExecutionId = queryExecutionId };
            bool isQueryStillRunning = true;
            var queryState = "";
            while (isQueryStillRunning)
            {
                var getQueryExecutionResponse = _client.GetQueryExecutionAsync(getQueryExecutionRequest);
                //var getQueryExecutionResponse = _client.getq  GetQueryExecution(getQueryExecutionRequest);
                queryState = getQueryExecutionResponse.Result.QueryExecution.Status.State.ToString();
                //queryState = getQueryExecutionResponse.QueryExecution.Status.State.ToString();
                
                if (queryState == QueryExecutionState.FAILED.ToString())
                {
                    throw new Exception("Query Failed to run with Error Message: " + getQueryExecutionResponse.Result.QueryExecution.Status.StateChangeReason);
                }
                else if (queryState == QueryExecutionState.CANCELLED.ToString())
                {
                    throw new Exception("Query was cancelled.");
                }
                else if (queryState == QueryExecutionState.SUCCEEDED.ToString())
                {
                    isQueryStillRunning = false;
                }
                else
                {
                    Thread.Sleep(5 * 1000);
                }
            }
        }

        public AmazonAthenaClient LoadConfiguration()
        {
            var json = File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "default_aws_profile.json"));

            var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;

            var key = root.GetProperty("app-user-key").GetString();
            var secret = root.GetProperty("app-user-secret").GetString();
            var BucketRegion = RegionEndpoint.GetBySystemName(root.GetProperty("region").GetString());
            Database_Name = root.GetProperty("athena-database-name").GetString();
            Output_Location = root.GetProperty("athena-output-location").GetString();

            var creds = new Amazon.Runtime.BasicAWSCredentials(key, secret);
            var c = new AmazonAthenaConfig();
            c.LogMetrics = true;
            c.DisableLogging = false;
            c.LogResponse = true;
            c.RegionEndpoint = BucketRegion;
            return new AmazonAthenaClient(creds, c);
        }
    }
}
