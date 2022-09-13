using Infogroup.IDMS.SegmentSelections;
using System;

namespace Infogroup.IDMS.Tests.SegmentSelections
{
    public abstract class SegmentSelectionAppServiceTestBase : AppTestBase
    {
        protected readonly ISegmentSelectionsAppService SegmentSelectionsAppService;

        protected SegmentSelectionAppServiceTestBase()
        {
            SegmentSelectionsAppService = Resolve<ISegmentSelectionsAppService>();
        }

        protected void CreateTestSegmentSelections()
        {            
              UsingDbContext(
                context =>
                {
                    context.SegmentSelections.Add(CreateSegmentSelectionEntity(3370615, "", "", "N",
                        "OR","", "ABINUMBER", "", "tblMain_12705_201802", "T","IN","Unit_Test", 1, 1, "Test"));
                    context.SegmentSelections.Add(CreateSegmentSelectionEntity(3370615, "", "", "N",
                        "OR", "", "ABINUMBER", "", "tblMain_12705_201802", "T", "IN", "Unit_Test1", 1, 1, "Test"));
                    context.SegmentSelections.Add(CreateSegmentSelectionEntity(3370615, "", "", "N",
                        "OR", "", "ABINUMBER", "", "tblMain_12705_201802", "T", "IN", "Unit_Test2", 1, 1, "Test"));

                });
        }

        protected SegmentSelection CreateSegmentSelectionEntity(int SegmentId, string cDescriptions, string cFileName, string cGrouping, string cJoinOperator,
            string cQuestionDescription, string cQuestionFieldName, string cSystemFileName, string cTableName, string cValueMode,
            string cValueOperator, string cValues, int iGroupNumber, int iGroupOrder, string cCreatedBy)
        {
            var segmentSelection = new SegmentSelection
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
                dCreatedDate=DateTime.Now,
                cCreatedBy= cCreatedBy

            };           
            return segmentSelection;
        }
    }
}