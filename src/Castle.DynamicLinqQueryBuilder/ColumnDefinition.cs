using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Defines the columns to be filtered against in the UI component of jQuery Query Builder
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        public ColumnDefinition()
        {
            PrettyOutputTransformer = o => o;
        }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [DataMember]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [DataMember]
        public string Field { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember]
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        [DataMember]
        public string Input { get; set; }
        /// <summary>
        /// Gets or sets the multiple.
        /// </summary>
        /// <value>
        /// The multiple.
        /// </value>
        [DataMember]
        public bool? Multiple { get; set; }
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        [DataMember]
        public string Values { get; set; }
        /// <summary>
        /// Gets or sets the operators.
        /// </summary>
        /// <value>
        /// The operators.
        /// </value>
        [DataMember]
        public List<string> Operators { get; set; }
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        [DataMember]
        public string Template { get; set; }
        /// <summary>
        /// Gets or sets the item bank not filterable.
        /// </summary>
        /// <value>
        /// The item bank not filterable.
        /// </value>
        [DataMember]
        public bool? ItemBankNotFilterable { get; set; }
        /// <summary>
        /// Gets or sets the item bank not column.
        /// </summary>
        /// <value>
        /// The item bank not column.
        /// </value>
        [DataMember]
        public bool? ItemBankNotColumn { get; set; }
        /// <summary>
        /// Gets or sets the pretty output transformer.
        /// </summary>
        /// <value>
        /// The pretty output transformer.
        /// </value>
        [IgnoreDataMember]
        public Func<object, object> PrettyOutputTransformer { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the optgroup.
        /// </summary>
        /// <value>
        /// The optgroup.
        /// </value>
        [DataMember]
        public string optgroup { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The Value.
        /// </value>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Additional data not used by QueryBuilder but that will be added to the output rules object. Use this to store any functional data 
        /// </summary>
        [DataMember]
        public BuildTableLayoutDetails Data { get; set; }
    } /// <summary>
      /// BuildTableLayoutDetails
      /// </summary>
    public class BuildTableLayoutDetails
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// iDataLength
        /// </summary>
        public int iDataLength { get; set; }
        /// <summary>
        /// cFieldName
        /// </summary>
        public string cFieldName { get; set; }
        /// <summary>
        /// cTableName
        /// </summary>
        public string cTableName { get; set; }
        /// <summary>
        /// iShowTextBox
        /// </summary>
        public bool iShowTextBox { get; set; }
        /// <summary>
        /// iShowListBox
        /// </summary>
        public bool iShowListBox { get; set; }
        /// <summary>
        /// iFileOperations
        /// </summary>
        public bool iFileOperations { get; set; }
        /// <summary>
        /// iShowDefault
        /// </summary>
        public int iShowDefault { get; set; }
        /// <summary>
        /// cFieldType
        /// </summary>
        public string cFieldType { get; set; }
        /// <summary>
        /// iDisplayOrder
        /// </summary>
        public int iDisplayOrder { get; set; }
        /// <summary>
        /// iIsListSpecific
        /// </summary>
        public bool iIsListSpecific { get; set; }
        /// <summary>
        /// cFieldDescription
        /// </summary>
        public string cFieldDescription { get; set; }
        /// <summary>
        /// iBTID
        /// </summary>
        public int iBTID { get; set; }
        /// <summary>
        /// IsFavourite
        /// </summary>
        public bool IsFavourite { get; set; }
        /// <summary>
        /// cDataType
        /// </summary>
        public string cDataType { get; set; }        
        /// <summary>
        /// ID
        /// </summary>
        public int iDBID { get; set; }


    }
}
