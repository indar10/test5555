using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using System;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Infogroup.IDMS.Tests.SegmentSelections
{
    // ReSharper disable once InconsistentNaming
    public class SegmentSelectionAppService_Create_Tests : SegmentSelectionAppServiceTestBase
    {
        
        [Fact]
        public async Task Should_Create_SegmentSelection()
        {            
            await CreateSegmentSelectionAndTestAsync(656167, 992, 3370638, "", "", "N",
                        "OR", "", "ABINUMBER", "", "tblMain_12705_201802", "T", "IN", "UNIT_TEST", 1, 1, "saArthakp");
            
        }        

        private async Task CreateSegmentSelectionAndTestAsync(int campaignId, int databaseId,int SegmentId, string cDescriptions, string cFileName, string cGrouping, string cJoinOperator,
            string cQuestionDescription, string cQuestionFieldName, string cSystemFileName, string cTableName, string cValueMode,
            string cValueOperator, string cValues, int iGroupNumber, int iGroupOrder, string cCreatedBy)
        {

            var selections = new List<SegmentSelectionDto>();

            selections.Add(
                new SegmentSelectionDto
                {
                    cDescriptions = cDescriptions,
                    cFileName = cFileName,
                    cGrouping = cGrouping,
                    cJoinOperator = cJoinOperator,
                    cQuestionDescription = cQuestionDescription,
                    cQuestionFieldName = cQuestionFieldName,
                    cSystemFileName = cSystemFileName,
                    cTableName = cTableName,
                    cValueMode = cValueMode,
                    cValueOperator = cValueOperator,
                    cValues = cValues,
                    iGroupNumber = iGroupNumber,
                    iGroupOrder = iGroupOrder,
                    SegmentId = SegmentId,
                    dCreatedDate = DateTime.Now,
                    cCreatedBy = cCreatedBy
                });

            await SegmentSelectionsAppService.CreateSegmentSelectionDetails(
                new SegmentSelectionSaveDto
                {
                   selections = selections,
                   campaignId = campaignId,
                   DatabaseId = databaseId,
                   deletedSelections = new List<int>()
                },
                true
                );

           
            await UsingDbContextAsync(async context =>
            {                
                var createdSelection = await context.SegmentSelections.FirstOrDefaultAsync(o => o.SegmentId == SegmentId 
                && o.cQuestionFieldName == cQuestionFieldName);
                createdSelection.ShouldNotBe(null);
                createdSelection.cQuestionFieldName.ShouldBe(cQuestionFieldName);
                createdSelection.cSystemFileName.ShouldBe(cSystemFileName);
                createdSelection.cTableName.ShouldBe(cTableName);
                createdSelection.cValueMode.ShouldBe(cValueMode);
                createdSelection.cValueOperator.ShouldBe(cValueOperator);
                createdSelection.cValues.ShouldBe(cValues);
                createdSelection.cDescriptions.ShouldBe(cDescriptions);
                createdSelection.cFileName.ShouldBe(cFileName);
                createdSelection.cGrouping.ShouldBe(cGrouping);
                createdSelection.cJoinOperator.ShouldBe(cJoinOperator);
                createdSelection.cQuestionDescription.ShouldBe(cQuestionDescription);                              
                createdSelection.cCreatedBy.ShouldBe(cCreatedBy);
            });
        }

        [Fact]
        public async Task Should_Create_SegmentSelectionList()
        {
            var selections = new List<SegmentSelectionDto>();
            var temp = 1;
            for (int i = 1; i <= 20; i++)
            {
                selections.Add(
                    new SegmentSelectionDto
                    {
                        cDescriptions = "",
                        cFileName = "",
                        cGrouping = "Y",
                        cJoinOperator = "IN",
                        cQuestionDescription = "",
                        cQuestionFieldName = "ABINUMBER",
                        cSystemFileName = "",
                        cTableName = "tblMain_12705_201802",
                        cValueMode = "T",
                        cValueOperator = "AND",
                        cValues = $"UNIT_TEST{i}",
                        iGroupNumber = temp,
                        iGroupOrder = 1,
                        SegmentId = 3370687,
                        dCreatedDate = DateTime.Now,
                        cCreatedBy = "saArthakp",
                    });

                if (i%2 == 0)                
                    temp++;
                
            }
            await CreateSegmentSelectionListAndTestAsync(selections, 656167, 992);

        }

        private async Task CreateSegmentSelectionListAndTestAsync(List<SegmentSelectionDto> selections, int campaignId, int databaseId)
        {
            await SegmentSelectionsAppService.CreateSegmentSelectionDetails(
                new SegmentSelectionSaveDto
                {
                    selections = selections,
                    campaignId = campaignId,
                    DatabaseId = databaseId,
                    deletedSelections = new List<int>()
                },
                true
                );


             UsingDbContext(context =>
            {
                var createdSelection =  context.SegmentSelections.Where(o => o.SegmentId == selections[0].SegmentId
                && o.cQuestionFieldName == selections[0].cQuestionFieldName).ToList();
                var createdSelectionOrder = createdSelection.OrderBy(x => x.iGroupNumber);
                createdSelection.ShouldNotBe(null);
                createdSelection.SequenceEqual(createdSelectionOrder);               
            });
        }
    }
}
