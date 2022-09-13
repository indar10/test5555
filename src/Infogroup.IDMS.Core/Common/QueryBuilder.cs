using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infogroup.IDMS.Common
{
    public class QueryBuilder
    {
        private readonly List<string> whereList = new List<string>();
        private readonly List<string> havingList = new List<string>();
        private readonly List<TableJoin> joinList = new List<TableJoin>();
        private readonly List<SqlParameter> sqlParamList = new List<SqlParameter>();
        private string columns = string.Empty, fromTable = string.Empty, fromTableAlias = string.Empty, sortColumns = string.Empty, groupByColumns= string.Empty, noLock = string.Empty, distinct = string.Empty, offset = string.Empty;
        private int paramCounter = 0;

        public QueryBuilder()
        {
            AddNoLock();
        }

        public void AddSelect(string cols)
        {
            columns = cols;
        }

        public void AddFrom(string from, string alias)
        {
            fromTable = from;
            fromTableAlias = alias;
        }

        public void AddGroupBy(string columns)
        {
            groupByColumns = columns;
        }

        public void AddSort(string sort)
        {
            sortColumns = sort;
        }

        public TableJoin AddJoin(string tableName, string tableAlias, string joinFieldName, string joinTableName)
        {
            var join = new TableJoin(tableName, tableAlias, joinFieldName, joinTableName);
            joinList.Add(join);
            return join;
        }

        public TableJoin AddJoin(string tableName, string tableAlias, string joinFieldName, string joinTableName, string joinType, string joinTableColumn)
        {
            var join = new TableJoin(tableName, tableAlias, joinFieldName, joinTableName, joinType, joinTableColumn);
            joinList.Add(join);
            return join;
        }

        public void AddNoLock()
        {
            noLock = "WITH(NOLOCK)";
        }

        public void AddDistinct()
        {
            distinct = "DISTINCT";
        }


        public void AddHaving(string condition, string fieldName, string valueOperator, string[] values)
        {
            var valueClause = "";
            var sqlPararms = new List<SqlParameter>();

            switch (valueOperator)
            {
                case "IS NULL":
                    havingList.Add($"{(whereList.Count > 0 ? condition : string.Empty)} {fieldName} {valueOperator} {valueClause}");
                    break;
                case "IN":
                case "NOT IN":
                    (valueClause, sqlPararms) = HandleIN(values.ToList());
                    break;
                case "BETWEEN":
                    (valueClause, sqlPararms) = HandleBETWEEN(values.ToList());
                    break;
                case "LIKE":
                    (valueClause, sqlPararms) = HandleLIKE(values.ToList());
                    break;
                case "EQUALTO":
                    valueOperator = "=";
                    (valueClause, sqlPararms) = HandleEQUALTO(values.ToList());
                    break;
                case "NOTEQUALTO":
                    valueOperator = "<>";
                    (valueClause, sqlPararms) = HandleNOTEQUALTO(values.ToList());
                    break;
                case "EXISTS":
                    havingList.Add("(EXISTS(" + valueClause + "))");
                    break;
                case "GREATERTHAN":
                    valueOperator = ">";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                case "SMALLERTHAN":
                    valueOperator = "<";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                default:
                    break;
            }

            if (sqlPararms.Count() > 0)
            {
                havingList.Add($"{(havingList.Count > 0 ? condition : string.Empty)} {fieldName} {valueOperator} {valueClause}");
                foreach (SqlParameter sp in sqlPararms)
                    sqlParamList.Add(sp);
            }
        }

        public void AddWhereString(string query)
        {
            whereList.Add(query);
        }

        public void AddWhere(string condition, string fieldName, string valueOperator, string[] values)
        {
            var valueClause = "";
            var sqlPararms = new List<SqlParameter>();

            switch (valueOperator)
            {
                case "IS NULL":
                    whereList.Add($"{(whereList.Count > 0 ? condition : string.Empty)} {fieldName} {valueOperator} {valueClause}");
                    break;
                case "IN":
                case "NOT IN":
                    (valueClause, sqlPararms) = HandleIN(values.ToList());
                    break;
                case "BETWEEN":
                    (valueClause, sqlPararms) = HandleBETWEEN(values.ToList());
                    break;
                case "LIKE":
                    (valueClause, sqlPararms) = HandleLIKE(values.ToList());
                    break;
                case "NOT LIKE":
                    (valueClause, sqlPararms) = HandleNOTLIKE(values.ToList());
                    break;
                case "EQUALTO":
                    valueOperator = "=";
                    (valueClause, sqlPararms) = HandleEQUALTO(values.ToList());
                    break;
                case "NOTEQUALTO":
                    valueOperator = "<>";
                    (valueClause, sqlPararms) = HandleNOTEQUALTO(values.ToList());
                    break;
                case "EXISTS":
                    whereList.Add($"{(whereList.Count > 0 ? condition : string.Empty)}{fieldName}(EXISTS({values[0]}))");
                    break;
                case "GREATERTHAN":
                    valueOperator = ">";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                case "SMALLERTHAN":
                    valueOperator = "<";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                case "GREATERTHAN_OR_EQUALTO":
                    valueOperator = ">=";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                case "LESSTHAN_OR_EQUALTO":
                    valueOperator = "<=";
                    (valueClause, sqlPararms) = HandleGreatherThan(values.ToList());
                    break;
                default:
                    break;
            }

            if (sqlPararms.Count() > 0)
            {
                whereList.Add($"{(whereList.Count > 0 ? condition : string.Empty)} {fieldName} {valueOperator} {valueClause}");
                foreach (SqlParameter sp in sqlPararms)
                    sqlParamList.Add(sp);
            }
        }

        public void AddHaving(string condition, string fieldName, string valueOperator, string value)
        {
            AddHaving(condition, fieldName, valueOperator, new string[1] { value });
        }

        public void AddWhere(string condition, string fieldName, string valueOperator, string value)
        {
            AddWhere(condition, fieldName, valueOperator, new string[1] { value });
        }

        private (string, List<SqlParameter>) HandleEQUALTO(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    string name = $"@P{paramCounter}";
                    paramNames.Append($"{name},");
                    var value = p.Replace("_", "[_]");
                    sqlParams.Add(new SqlParameter(name, value.Trim()));
                    paramCounter++;
                }
            }

            return ($"({paramNames.ToString().TrimEnd(',')})", sqlParams);
        }

        private (string, List<SqlParameter>) HandleNOTEQUALTO(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values)
            {
                string name = $"@P{paramCounter}";
                paramNames.Append($"{name},");
                var value = p.Replace("_", "[_]");
                sqlParams.Add(new SqlParameter(name, value.Trim()));
                paramCounter++;

            }

            return ($"({paramNames.ToString().TrimEnd(',')})", sqlParams);
        }

        private (string, List<SqlParameter>) HandleGreatherThan(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values)
            {
                string name = $"@P{paramCounter}";
                paramNames.Append($"{name},");
                var value = p.Replace("_", "[_]");
                sqlParams.Add(new SqlParameter(name, value.Trim()));
                paramCounter++;

            }

            return ($"({paramNames.ToString().TrimEnd(',')})", sqlParams);
        }
        private (string, List<SqlParameter>) HandleSmallerThan(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values)
            {
                string name = $"@P{paramCounter}";
                paramNames.Append($"{name},");
                var value = p.Replace("_", "[_]");
                sqlParams.Add(new SqlParameter(name, value.Trim()));
                paramCounter++;

            }

            return ($"({paramNames.ToString().TrimEnd(',')})", sqlParams);
        }

        private (string, List<SqlParameter>) HandleIN(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            var intvalues = values.Where(x => x != null).Select(long.Parse).ToList();
            foreach (var p in intvalues)
            {
                string name = $"@P{paramCounter}";
                paramNames.Append($"{name},");
                sqlParams.Add(new SqlParameter(name, p));
                paramCounter++;
            }

            return ($"({paramNames.ToString().TrimEnd(',')})", sqlParams);
        }

        private (string, List<SqlParameter>) HandleBETWEEN(List<string> values)
        {
            string strparamNames = "";
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values.Take(2))
            {
                if (!string.IsNullOrEmpty(p))
                {
                    string name = $"@P{paramCounter}";
                    paramNames.Append($"{name} AND ");
                    var value = p.Replace("_", "[_]");
                    sqlParams.Add(new SqlParameter(name, value));
                    paramCounter++;
                }
            }

            //Remove extra AND from the end of the string
            if (paramNames.Length > 3)
                strparamNames = paramNames.ToString().Substring(0, paramNames.Length - 4);

            return (strparamNames, sqlParams);
        }

        private (string, List<SqlParameter>) HandleLIKE(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values.Take(1))
            {
                if (!string.IsNullOrEmpty(p))
                {
                    string name = $"@P{paramCounter}";
                    paramNames.Append($"{name}");
                    var value = p.Replace("_", "[_]");
                    sqlParams.Add(new SqlParameter(name, $"%{ value.Trim() }%"));
                    paramCounter++;
                }
            }

            return (paramNames.ToString(), sqlParams);
        }

        private (string, List<SqlParameter>) HandleNOTLIKE(List<string> values)
        {
            var paramNames = new StringBuilder();
            var sqlParams = new List<SqlParameter>();

            foreach (var p in values.Take(1))
            {
                if (!string.IsNullOrEmpty(p))
                {
                    string name = $"@P{paramCounter}";
                    paramNames.Append($"{name}");
                    var value = p.Replace("_", "[_]");
                    sqlParams.Add(new SqlParameter(name, $"%{ value.Trim() }%"));
                    paramCounter++;
                }
            }

            return (paramNames.ToString(), sqlParams);
        }

        public (string, List<SqlParameter>) Build()
        {
            var sql = $"SELECT {distinct} {columns} \r\n FROM {fromTable} {fromTableAlias} {noLock} \r\n {GetJoinSQL()} \r\n";

            if (whereList.Count() > 0)
                sql += $"WHERE {string.Join("\r\n ", whereList)}";
        
            if (groupByColumns.Length > 0)
                sql += $"\r\nGROUP BY {groupByColumns}\r\n";

            if (havingList.Count() > 0)
                sql += $"HAVING {string.Join("\r\n ", havingList)}";

            if (sortColumns.Length > 0)
                sql += $"\r\nORDER BY {sortColumns}\r\n";



            if (!string.IsNullOrEmpty(offset))
                sql += offset;

            return (sql, sqlParamList);
        }

        public (string, List<SqlParameter>) BuildCount()
        {
            var sql = $"SELECT COUNT(DISTINCT {fromTableAlias}.ID) \r\n FROM {fromTable} {fromTableAlias} {noLock} \r\n {GetJoinSQL()} \r\n";

            if (whereList.Count() > 0)
                sql += $"WHERE {string.Join("\r\n ", whereList)}";

            return (sql, sqlParamList);
        }
        public (string, List<SqlParameter>) BuildCountForBatchQueue()
        {
            var sql = $"SELECT COUNT(DISTINCT {fromTableAlias}.QueueId) \r\n FROM {fromTable} {fromTableAlias} {noLock} \r\n {GetJoinSQL()} \r\n";

            if (whereList.Count() > 0)
                sql += $"WHERE {string.Join("\r\n ", whereList)}";

            return (sql, sqlParamList);
        }

        private string GetJoinSQL()
        {
            var joins = new StringBuilder();

            foreach (var j in joinList)
            {
                if (string.IsNullOrEmpty(j.JoinTableColumn))
                    joins.Append($"{j.JoinType} {j.TableName} {j.TableAlias} {noLock} ON {j.JoinTable}.{j.JoinColumn} = {j.TableAlias}.{j.JoinColumn}{j.AdditionalClauses}\r\n");
                else
                    joins.Append($"{j.JoinType} {j.TableName} {j.TableAlias} {noLock} ON {j.JoinTable}.{j.JoinColumn} = {j.TableAlias}.{j.JoinTableColumn}{j.AdditionalClauses}\r\n");
            }

            return joins.ToString();
        }

        public void AddRootRelation(string condition, string expression)
        {
            whereList.Add($"{(whereList.Count > 0 ? condition : string.Empty)} {expression}");

        }
        public static string AddRelation(string expression1, string condition, string expression2) => $"( {expression1} {condition} {expression2} )";

        public string AddExpression(string fieldName, string valueOperator, string[] values)
        {

            var valueClause = "";
            var sqlPararms = new List<SqlParameter>();
            var expression = "";
            switch (valueOperator)
            {
                case "IS NULL":
                    expression = $"{fieldName} {valueOperator} {valueClause}";
                    break;
                case "IN":
                    (valueClause, sqlPararms) = HandleIN(values.ToList());
                    break;
                case "BETWEEN":
                    (valueClause, sqlPararms) = HandleBETWEEN(values.ToList());
                    break;
                case "LIKE":
                    (valueClause, sqlPararms) = HandleLIKE(values.ToList());
                    break;
                case "EQUALTO":
                    valueOperator = "=";
                    (valueClause, sqlPararms) = HandleEQUALTO(values.ToList());
                    break;
                case "NOTEQUALTO":
                    valueOperator = "<>";
                    (valueClause, sqlPararms) = HandleNOTEQUALTO(values.ToList());
                    break;
                default:
                    break;
            }

            if (sqlPararms.Count() > 0)
            {
                expression = $"{fieldName} {valueOperator} {valueClause}";
                foreach (SqlParameter sp in sqlPararms)
                    sqlParamList.Add(sp);
            }
            return expression;
        }
        public void AddOffset(string offsetQuery)
        {
            offset = offsetQuery;
        }
        public string AddExpression(string fieldName, string valueOperator, string value)
        {
            return AddExpression(fieldName, valueOperator, new string[1] { value });
        }
        public (string, List<SqlParameter>) BuildCountSelectionFieldCount()
        {
            var sql = $"SELECT Count(*) FROM( SELECT COUNT(DISTINCT {fromTableAlias}.ID) AS count \r\n FROM {fromTable} {fromTableAlias} {noLock} \r\n {GetJoinSQL()} \r\n";

            if (whereList.Count() > 0)
                sql += $"WHERE {string.Join("\r\n ", whereList)}";
            if (groupByColumns.Count() > 0)
                sql += $" GROUP BY {string.Join("\r\n ", groupByColumns)}) As TotalRows";

            return (sql, sqlParamList);
        }
        public (string, List<SqlParameter>) BuildCountProcessQueue()
        {
            var sql = $"SELECT COUNT({fromTableAlias}.ID) \r\n FROM {fromTable} {fromTableAlias} {noLock} \r\n {GetJoinSQL()} \r\n";

            if (whereList.Count() > 0)
                sql += $"WHERE {string.Join("\r\n ", whereList)}";

            return (sql, sqlParamList);
        }

    }
}
