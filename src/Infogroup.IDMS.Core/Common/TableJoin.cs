using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Common
{
    public class TableJoin
    {
        public TableJoin(string tableName, string tableAlias, string joinColumn, string joinTable)
        {
            TableName = tableName;
            TableAlias = tableAlias;
            JoinColumn = joinColumn;
            JoinTable = joinTable;
            JoinType = "INNER JOIN";
            AdditionalClauses = "";
        }

        public TableJoin(string tableName, string tableAlias, string joinColumn, string joinTable, string joinType, string joinTableColumn)
        {
            TableName = tableName;
            TableAlias = tableAlias;
            JoinColumn = joinColumn;
            JoinTable = joinTable;
            JoinType = joinType;
            JoinTableColumn = joinTableColumn;
            AdditionalClauses = "";
        }

        public TableJoin And(string leftColumn, string valueOperator, string rightColumn)
        {
            //var clauseBuilder = new StringBuilder(AdditionalClauses);
            if (valueOperator.Equals("EQUALTO")) valueOperator = "=";
            else if (valueOperator.Equals("NOTEQUALTO")) valueOperator = "<>";
            //clauseBuilder.AppendLine($" AND {leftColumn} {valueOperator} {rightColumn}");
            AdditionalClauses = $"{AdditionalClauses}{Environment.NewLine} AND {leftColumn} {valueOperator} {rightColumn}";
            return this;
        }
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public string JoinColumn { get; set; }
        public string JoinTable { get; set; }
        public string JoinType { get; set; }
        public string JoinTableColumn { get; set; }
        public string AdditionalClauses { get; set; }
    }
}
