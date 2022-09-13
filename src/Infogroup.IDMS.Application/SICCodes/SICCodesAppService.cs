using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.SICCodes.Dtos;
using Infogroup.IDMS.Validation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace Infogroup.IDMS.SICCodes
{
    [AbpAuthorize]
    public class SICCodesAppService : IDMSAppServiceBase, ISICCodesAppService
    {
        private readonly ISICCodesRepository _customSICCodeRepository;
        private readonly Dictionary<string, string> Style;
        private List<SICCodeDto> _SICCodeData;

        public SICCodesAppService(ISICCodesRepository customSICCodeRepository)
        {
            _customSICCodeRepository = customSICCodeRepository;
            Style = new Dictionary<string, string> {
                { "F", "node-franchise" },
                { "I", "node-industry" },
                { "B", "node-bothfni" },
                { "CF", "child-franchise" },
                { "CI", "child-industry" },
                { "CB", "child-bothfni" },
                { "CN", "child-none" }
            };
        }

        public List<TreeNode> GetSICCode(GetAllSICCodesInput input)
        {
            var values = new List<SICCode>();
            const int MAXRECORDS = 500;
            if (string.IsNullOrEmpty(input.Filter))
                return new List<TreeNode>();
            try
            {
                var sicLenth = Convert.ToInt32(input.cType);
                var isCodeFilter = ValidationHelper.IsNumeric(input.Filter);
                var dynamicWhereClause = GetDynamicWhereClause(input, isCodeFilter);

                var countQuery = GetSICCountQuery(input, dynamicWhereClause);
                var totalCount = _customSICCodeRepository.GetSICCodesCount(countQuery);

                if (totalCount > MAXRECORDS) throw new UserFriendlyException(L("MaxSICLimitExeceded"));

                _SICCodeData = totalCount == 0 ? new List<SICCodeDto>() :
                    _customSICCodeRepository.GetSICCodes(GetAllSICDataQuery(input, dynamicWhereClause, isCodeFilter));
                var nodes = _SICCodeData
                 .GroupBy(sic => new { sic.SICCode, sic.SICDescription, sic.Indicator })
                 .Select(sic => new TreeNode
                 {
                     Label = $"{sic.Key.SICCode} - {sic.Key.SICDescription}",
                     Data = new NodeData
                     {
                         cSICCode = sic.Key.SICCode,
                         cDescription = sic.Key.SICDescription,
                         cIndicator = string.IsNullOrWhiteSpace(sic.Key.Indicator) ? "N" : sic.Key.Indicator,
                         Level = 1,
                         SicLength = sicLenth
                     },
                     StyleClass = string.IsNullOrWhiteSpace(sic.Key.Indicator) ? string.Empty : Style[sic.Key.Indicator]
                 }).ToList();
                nodes.ForEach(node =>
                {
                    var children = GetRelatedSICCode(node.Data.cSICCode, input.IsSortyBySICCode);
                    node.Leaf = children.Count > 0 ? false : true;
                    node.Children = children;
                });
                return nodes;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        private List<TreeNode> GetRelatedSICCode(string cSICCode, bool IsSortyBySICCode)
        {
            var result = new List<TreeNode>();
            try
            {
                var query = _SICCodeData
                    .Where(sic => sic.SICCode.Equals(cSICCode) && !string.IsNullOrEmpty(sic.RelatedSICCode));
                query = IsSortyBySICCode ? query.OrderBy(sic => sic.RelatedSICCode) : query.OrderBy(sic => sic.RelatedSICDescription);
                var distinctQuery = query.GroupBy(sic => new { sic.RelatedSICCode, sic.RelatedSICDescription, sic.RelatedIndicator });
                var relatedSICs = distinctQuery.Select(sic => new TreeNode
                {
                    Label = $"{sic.Key.RelatedSICCode} - {sic.Key.RelatedSICDescription}",
                    Leaf = true,
                    StyleClass = string.IsNullOrWhiteSpace(sic.Key.RelatedIndicator) ? Style["CN"] : Style[$"C{sic.Key.RelatedIndicator}"],
                    Data = new NodeData
                    {
                        cSICCode = sic.Key.RelatedSICCode,
                        cDescription = sic.Key.RelatedSICDescription,
                        cIndicator = string.IsNullOrWhiteSpace(sic.Key.RelatedIndicator) ? "N" : sic.Key.RelatedIndicator,
                        Level = 2
                    }
                }).ToList();
                return relatedSICs;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public FranchiseNIndusdustry GetFranchiseNIndustryCode(string cSICCode, string cIndicator)
        {
            var result = new FranchiseNIndusdustry();
            try
            {
                if (cIndicator.Equals("B") || cIndicator.Equals("F"))
                {
                    result.Franchises = _customSICCodeRepository.GetFranchiseIndustryBySIC(GetFranchiseBySICQuery(cSICCode));
                }
                if (cIndicator.Equals("B") || cIndicator.Equals("I"))
                {
                    result.Industries = _customSICCodeRepository.GetFranchiseIndustryBySIC(GetIndustryBySICQuery(cSICCode));
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public SmartAddOutputDto ValidateSICCodes(ValidateSICCodesInputDto input)
        {
            var result = new SmartAddOutputDto { SICSelections = new List<List<DropdownOutputDto>>() };
            try
            {
                result.SICSelections.AddRange(Enumerable.Repeat(new List<DropdownOutputDto>(), 3));
                var inputCodes = input.SearchText.Split(',')
                    .Select(code => code.Trim())
                    .Where(code => !string.IsNullOrEmpty(code))
                    .ToArray();

                var inputLookup = inputCodes.ToLookup(code => code.Contains("-"));
                var individualInputs = inputLookup[false].ToArray();
                var rangeInputs = inputLookup[true].ToList();

                var query = GetSICSmartAddQuery(individualInputs);
                (List<SICCode> SICCodesList, List<string> validCodes) = _customSICCodeRepository.GetSICCodesForSmartAdd(query);

                for (var index = 1; index <= 3; index += 1)
                    result.SICSelections[index - 1] = SICCodesList
                        .Where(SICData => SICData.cType == (index * 2).ToString())
                        .Select(data => new DropdownOutputDto { Value = data.cSICCode, Label = data.cSICDescription })
                        .ToList();

                var invalidRanges = ProcessRangeInputs(rangeInputs, result.SICSelections);

                var invalidCodes = individualInputs.Except(validCodes).ToList();
                invalidCodes.AddRange(invalidRanges);
                var invalidInputs = invalidCodes.ToArray();

                var dynamicMessage = invalidInputs.Length > 1 ? $"s {string.Join(", ", invalidInputs, 0, invalidInputs.Length - 1)} and {invalidCodes.LastOrDefault()} are" :
                    invalidInputs.Length == 1 ? $" {invalidInputs.FirstOrDefault()} is" : string.Empty;
                if (!string.IsNullOrEmpty(dynamicMessage))

                    result.WarningMessage = L("InvalidCodesWarning", dynamicMessage);
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }



        #region Helpers
        private string GetLogicalOperator(string filter)
        {
            filter = filter.ToUpper();
            var hasAnd = filter.Contains(" AND ");
            var hasOr = filter.Contains(" OR ");
            if (hasAnd && hasOr) throw new UserFriendlyException(L("BothAndOrPresent"));
            return hasOr ? "OR" : "AND";
        }

        private Tuple<string, List<SqlParameter>> GetAllSICDataQuery(GetAllSICCodesInput input, Tuple<string, List<SqlParameter>> dynamicWhereClause, bool isCodeFilter)
        {
            var query = new QueryBuilder();
            query.AddSelect(@" LTRIM(RTRIM(SIC.cSICCode)) as Code
                              ,LTRIM(RTRIM(SIC.cSICDescription)) as Description
                              ,LTRIM(RTRIM(SIC.cIndicator)) as Indicator
                              ,LTRIM(RTRIM(RelatedSIC.cRelatedSICCode)) as RelatedCode
                              ,LTRIM(RTRIM(RelatedSIC.cRelatedSICDescription)) as RelatedDescription
                              ,LTRIM(RTRIM(RelatedSIC.cIndicator)) as RelatedIndicator");
            query.AddFrom("tblSICCode", "SIC");
            query.AddJoin("tblSICCodeRelated", "RelatedSIC", "cSICCode", "SIC", "LEFT JOIN", "cSICCode");
            query.AddNoLock();
            query.AddWhere("AND", "SIC.cType", "EQUALTO", input.cType);
            if (dynamicWhereClause.Item2.Count > 0)
                query.AddWhereString($" AND  ( {dynamicWhereClause.Item1} )");
            query.AddSort(!isCodeFilter && input.IsSortyBySICCode ? "SIC.cSICCode ASC" : "SIC.cSICDescription ASC");
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.AddRange(dynamicWhereClause.Item2);
            return new Tuple<string, List<SqlParameter>>(sql, sqlParams);
        }

        private Tuple<string, List<SqlParameter>> GetSICCountQuery(GetAllSICCodesInput input, Tuple<string, List<SqlParameter>> dynamicWhereClause)
        {
            var query = new QueryBuilder();
            query.AddFrom("tblSICCode", "SIC");
            query.AddNoLock();
            query.AddWhere("AND", "SIC.cType", "EQUALTO", input.cType);
            if (dynamicWhereClause.Item2.Count > 0)
                query.AddWhereString($" AND  ( {dynamicWhereClause.Item1} )");
            (string sql, List<SqlParameter> sqlParams) = query.BuildCount();
            sqlParams.AddRange(dynamicWhereClause.Item2);
            return new Tuple<string, List<SqlParameter>>(sql, sqlParams);
        }

        private Tuple<string, List<SqlParameter>> GetDynamicWhereClause(GetAllSICCodesInput input, bool isCodeFilter)
        {
            string[] searchData;
            string cLogicalOperator;
            var whereClauseList = new List<string>();
            var parameters = new List<SqlParameter>();
            if (isCodeFilter)
            {
                cLogicalOperator = "OR";
                searchData = input.Filter.Split(',').Select(inputText => inputText.Trim())
                   .Where(inputText => !string.IsNullOrWhiteSpace(inputText))
                   .ToArray();
                for (int index = 0; index < searchData.Length; index++)
                {
                    whereClauseList.Add($" SIC.cSICCode LIKE @Filter{index} ");
                    parameters.Add(new SqlParameter($"@Filter{index}", $"{searchData[index]}%"));
                }

            }
            else
            {
                cLogicalOperator = GetLogicalOperator(input.Filter);
                searchData = input.Filter.ToUpper().Split(new string[] { $" {cLogicalOperator} " }, StringSplitOptions.None);
                searchData = searchData.Select(inputText => inputText.Trim())
                    .Where(inputText => !string.IsNullOrWhiteSpace(inputText))
                    .ToArray();
                for (int index = 0; index < searchData.Length; index++)
                {
                    whereClauseList.Add($" SIC.cSICDescription LIKE @Filter{index} ");
                    parameters.Add(new SqlParameter($"Filter{index}", $"%{searchData[index]}%"));
                }
            }
            var whereClause = string.Join(cLogicalOperator, whereClauseList);
            return new Tuple<string, List<SqlParameter>>(whereClause, parameters);
        }

        private Tuple<string, List<SqlParameter>> GetFranchiseBySICQuery(string cSICCode)
        {
            var query = new QueryBuilder();
            query.AddSelect(@" LTRIM(RTRIM(franchise.cConvertedFranchise)) as Code
                              ,LTRIM(RTRIM(franchise.cFranchiseName)) as Description");
            query.AddFrom("tblSICFranchiseCode", "franchise");
            query.AddNoLock();
            query.AddWhere("AND", "franchise.cSICCode", "EQUALTO", cSICCode);
            query.AddSort("Code ASC");
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql, sqlParams);
        }

        private Tuple<string, List<SqlParameter>> GetIndustryBySICQuery(string cSICCode)
        {
            var query = new QueryBuilder();
            query.AddSelect(@" LTRIM(RTRIM(industry.cIndustrySpecificCode)) as Code
                              ,LTRIM(RTRIM(industry.cIndustrySpecificDescription)) as Description");
            query.AddFrom("tblIndustryCode", "industry");
            query.AddNoLock();
            query.AddWhere("AND", "industry.cSICCode", "EQUALTO", cSICCode);
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql, sqlParams);
        }

        private Tuple<string, List<SqlParameter>> GetSICSmartAddQuery(string[] inputCodes)
        {
            var sqlParams = new List<SqlParameter>();
            if (inputCodes.Length > 0)
            {
                var whereClauseParamValue = string.Join(",", inputCodes);
                sqlParams.Add(new SqlParameter($"@Codes", whereClauseParamValue));
            }
            var sqlQuery = $@"SELECT LTRIM(RTRIM(SIC.cSICCode)) as Code
                                    ,LTRIM(RTRIM(SIC.cSICDescription)) as Description
                                    ,LTRIM(RTRIM(SIC.cType)) as Type
                              FROM tblSICCode SIC
                              WHERE SIC.iPrimaryFlag = 1
                             {(sqlParams.Count > 0 ? $"AND cSICCode IN (SELECT value FROM STRING_SPLIT(@Codes, ','))" : string.Empty)}";

            return new Tuple<string, List<SqlParameter>>(sqlQuery, sqlParams);
        }

        private List<string> ProcessRangeInputs(List<string> rangeInputs, List<List<DropdownOutputDto>> SICSelections)
        {
            List<string> validCodes;
            const string pattern = "^\\s*\\d{{{0}}}\\s*(-[\\s\\S]*)*$";
            Regex regex;
            for (var index = 1; index <= 3; index++)
            {
                regex = new Regex(string.Format(pattern, index * 2));
                validCodes = rangeInputs.Where(a => regex.Match(a).Success).ToList();
                rangeInputs =rangeInputs.Except(validCodes).ToList();
                SICSelections[index - 1].AddRange(
                    validCodes.Select(rangeValue => new DropdownOutputDto { Value = rangeValue, Label = string.Empty })
                    .ToList());
            }

            return rangeInputs;
        }

        #endregion
    }
}