using System.Threading.Tasks;
using Shouldly;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.Segments;

namespace Infogroup.IDMS.Tests.SegmentSelections
{
    // ReSharper disable once InconsistentNaming
    public class BatchEditSegments_Tests : SegmentSelectionAppServiceTestBase
    {

        [Theory]
        [InlineData(12705, 992, false, false)]
        [InlineData(12400, 66, false, true)]
        [InlineData(12210, 1267, true, false)]
        [InlineData(12346, 65, true, true)]
        public void Should_Fetch_Correct_State(int buildId, int databaseId, bool hasDefaultRules, bool calculateDistanceSet)
        {
            var state = this.SegmentSelectionsAppService.GetInitialStateForBatchEdit(buildId, databaseId);
            state.HasDefaultRules.ShouldBe(hasDefaultRules);
            state.IsCalculateDistanceSet.ShouldBe(calculateDistanceSet);
            state.MaxPers.Count().ShouldBe(11);
            for (var index = 0; index <= 9; index++)
            {
                state.MaxPers[index].Value.ShouldBe($"0{index}");
            }
            state.MaxPers[10].Value.ShouldBe("10");
        }
        [Fact]
        public void Should_Fetch_Correct_Records()
        {
            var input = new GetSegmentListInput
            {
                OrderId = 656173,
                Filter = "1-2,4",
                Sorting = null,
                MaxResultCount = 1,
                SkipCount = 1
            };
            var segments = this.SegmentSelectionsAppService.GetSegmentsForInlineEdit(input);
            segments.ShouldNotBe(null);
            segments.Count().ShouldBe(3);
            var prevSegmentNo = -1;
            var expectedSegmentDescriptions = new List<string> { "Segment 1", "Segment 2", "Segment 4" };
            for (var index = 0; index < segments.Count; index++)
            {
                segments[index].iDedupeOrderSpecified.ShouldBeGreaterThan(prevSegmentNo);
                segments[index].iRequiredQty.ShouldBeGreaterThanOrEqualTo(0);
                segments[index].cDescription.ShouldBe(expectedSegmentDescriptions[index]);
                segments[index].cMaxPerGroup.ShouldNotBeNullOrWhiteSpace();
                segments[index].cMaxPerGroup.ShouldBe("00");
                segments[index].cCreatedBy.ShouldNotBeNullOrWhiteSpace();
                segments[index].dCreatedDate.ShouldNotBeNull();
                segments[index].Dirty.ShouldBeFalse();
                segments[index].Index.ShouldBe(index);
                prevSegmentNo = segments[index].iDedupeOrderSpecified;
            }

        }
        [Theory]
        [MemberData(nameof(SaveBatchSegmentTestData))]
        public async Task Should_Save_Segments(SaveBatchSegmentDto input, int expectedStatus)
        {
            var campaignId = 0;
            if (input.ModifiedSegments.Count > 0)
            {
                campaignId = input.ModifiedSegments[0].OrderId;
            }
            await SegmentSelectionsAppService.SaveBatchSegments(input);
            var segmentNumbers = input.ModifiedSegments.Select(segment => segment.iDedupeOrderSpecified).ToList();
            UsingDbContext(context =>
                   {
                       var targetSegment = context.Segments.Where(segment => segment.OrderId == campaignId
                       && segmentNumbers.Contains(segment.iDedupeOrderSpecified)
                       && !segment.iIsOrderLevel
                       ).OrderBy(segment => segment.iDedupeOrderSpecified)
                       .ToList();
                       for (int index = 0; index < input.ModifiedSegments.Count; index++)
                           CompareSegmentFields(targetSegment[index], input.ModifiedSegments[index]);

                       var actualStatus = context.OrderStatuses.FirstOrDefault(o => o.OrderID == campaignId && o.iIsCurrent)?.iStatus ?? 0;
                       actualStatus.ShouldBe(expectedStatus);
                   });
        }

        void CompareSegmentFields(Segment actualSegment, CreateOrEditSegmentDto expectedSegment)
        {
            actualSegment.cDescription.ShouldBe(expectedSegment.cDescription);
            actualSegment.iRequiredQty.ShouldBe(expectedSegment.iRequiredQty);
            actualSegment.cKeyCode1.ShouldBe(expectedSegment.cKeyCode1);
            actualSegment.cKeyCode2.ShouldBe(expectedSegment.cKeyCode2);
            actualSegment.cMaxPerGroup.ShouldBe(expectedSegment.cMaxPerGroup);
            actualSegment.iGroup.ShouldBe(expectedSegment.iGroup);
            actualSegment.iIsRandomRadiusNth.ShouldBe(expectedSegment.iIsRandomRadiusNth);
            actualSegment.iUseAutosuppress.ShouldBe(expectedSegment.iUseAutosuppress);
            actualSegment.iUseAutosuppress.ShouldBe(expectedSegment.iUseAutosuppress);
            actualSegment.dModifiedDate.ShouldNotBe(expectedSegment.dModifiedDate);
        }

        public static IEnumerable<object[]> SaveBatchSegmentTestData
        {
            get
            {
                yield return new object[] {
                    new SaveBatchSegmentDto
                    {
                        ModifiedSegments = new List<CreateOrEditSegmentDto>
                        {
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saArthakp",
                                // Edited
                                cDescription = "Segment 1_2",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "1",
                                cKeyCode2 = "",
                                cMaxPerGroup = "00",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 1,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = true,
                                iOutputQty = -1,
                                iProvidedQty = 0,
                                iRequiredQty = 0,
                                iUseAutosuppress = true,
                                Id = 3370653,
                                OrderId = 656171,
                                dCreatedDate = new DateTime(2020, 8, 10, 17, 37, 00, DateTimeKind.Local),
                                dModifiedDate = DateTime.Now,
                            }
                        },
                        NextStatus = 1000
                    }
                    , 10
                };
                yield return new object[] {
                    new SaveBatchSegmentDto
                    {
                        ModifiedSegments = new List<CreateOrEditSegmentDto>
                        {
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saArthakp",
                                // Edited
                                cDescription = "Segment 1_2",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "1",
                                cKeyCode2 = "",
                                cMaxPerGroup = "00",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 1,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = true,
                                iOutputQty = -1,
                                iProvidedQty = 0,
                                iRequiredQty = 0,
                                iUseAutosuppress = true,
                                Id = 3370653,
                                OrderId = 656171,
                                dCreatedDate = new DateTime(2020, 8, 10, 17, 37, 00, DateTimeKind.Local),
                                dModifiedDate = DateTime.Now,
                            },
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saArthakp",
                                cDescription = "Copy of Segment 1_1",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "",
                                cKeyCode2 = "KC2",
                                cMaxPerGroup = "02",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                dCreatedDate = DateTime.Now,
                                dModifiedDate = DateTime.Now,
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 2,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = true,
                                iOutputQty = -1,
                                iProvidedQty = 0,
                                iRequiredQty = 100,
                                iUseAutosuppress = true,
                                Id = 3370654,
                                OrderId = 656171,
                            }
                        },
                        NextStatus = 10
                    }
                    , 10
                };
                yield return new object[] {
                    new SaveBatchSegmentDto
                    {
                        ModifiedSegments = new List<CreateOrEditSegmentDto>
                        {
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saugustine",
                                cDescription = "Copy of Segment 2_NEW",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "KC1",
                                cKeyCode2 = "",
                                cMaxPerGroup = "00",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 1,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = true,
                                iOutputQty = 5,
                                iProvidedQty = 0,
                                iRequiredQty = 0,
                                iUseAutosuppress = true,
                                Id = 3188489,
                                OrderId = 638504,
                                dCreatedDate = new DateTime(2019, 6, 3, 10, 5, 00, DateTimeKind.Local),
                                dModifiedDate = DateTime.Now,
                            },
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saugustine",
                                cDescription = "Segment 2",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "KC2",
                                cKeyCode2 = "",
                                cMaxPerGroup = "00",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 2,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = false,
                                iOutputQty = 91335,
                                iProvidedQty = 91336,
                                iRequiredQty = 0,
                                iUseAutosuppress = true,
                                Id = 3370702,
                                OrderId = 638504,
                                dCreatedDate = new DateTime(2019, 6, 3, 10, 5, 00, DateTimeKind.Local),
                                dModifiedDate = DateTime.Now,
                            },
                        },
                        NextStatus = 40
                    }
                    , 40
                };
                yield return new object[] {
                    new SaveBatchSegmentDto
                    {
                        ModifiedSegments = new List<CreateOrEditSegmentDto>
                        {                           
                            new CreateOrEditSegmentDto
                            {
                                ApplyDefaultRules = false,
                                cCreatedBy = "saugustine",
                                cDescription = "Segment 2",
                                cFieldDescription = "",
                                cFixedTitle = "",
                                cKeyCode1 = "KC2",
                                cKeyCode2 = "",
                                cMaxPerGroup = "00",
                                cModifiedBy = "saArthakp",
                                cTitleSuppression = "B",
                                iAvailableQty = 0,
                                iDedupeOrderSpecified = 2,
                                iDedupeOrderUsed = 0,
                                iDoubleMultiBuyerCount = 0,
                                iGroup = 1,
                                iIsCalculateDistance = false,
                                iIsOrderLevel = false,
                                iIsRandomRadiusNth = false,
                                iOutputQty = 91335,
                                iProvidedQty = 91336,
                                iRequiredQty = 0,
                                iUseAutosuppress = true,
                                Id = 3370702,
                                OrderId = 638504,
                                dCreatedDate = new DateTime(2019, 6, 3, 10, 5, 00, DateTimeKind.Local),
                                dModifiedDate = DateTime.Now,
                            },
                        },
                        NextStatus = 1000
                    }
                    , 40
                };
            }
        }

    }
}
