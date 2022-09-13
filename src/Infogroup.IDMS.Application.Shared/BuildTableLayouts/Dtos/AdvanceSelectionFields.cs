namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class AdvanceSelectionFields
    {
        public Field SICCode { get; set; }
        public Field MinorIndustryGroup { get; set; }
        public Field MajorIndustryGroup { get; set; }
        public Field FranchiseBySIC { get; set; }
        public Field IndustrySpecificBySIC { get; set; }
        public Field PrimarySICFlag { get; set; }
        public Field StateSelect { get; set; }
        public Field StateCountySelect { get; set; }
        public Field NeighborhoodSelect { get; set; }
        public Field StateCitySelect { get; set; }
        public Field GeoRadius { get; set; }
        public Field ZipRadius { get; set; }
        public Field IndustrySelection { get; set; }
        public Field OccupationSelection { get; set; }
        public Field SpecialtySelection { get; set; }
        public Field GeoMapping { get; set; }
    }

}