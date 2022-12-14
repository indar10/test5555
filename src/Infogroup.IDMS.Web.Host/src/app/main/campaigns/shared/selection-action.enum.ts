export enum SelectionAction {
    SaveOnly,
    Execute,
    QuickCount,
    SelectSIC,
    SelectCountyCity,
    SelectOccupation,
    SavedSelection,
    SelectMultiField,
    SearchGeoRadius,
    BulkSegmentUpload,
    AppendRules,
    DeleteRules,
    DeleteSegments,
    FindReplace,
    EditSegments,
    AddToFavorites,
    CopySegment,
    NewSegment,
    EditSegment,
    DeleteSegment,
    EditCampaign,
    Sources,
    CampaignHistory,
    EditOutputLayout,
    DataPreview,
    Print,
    Subsets,
    SegmentChange,
    ImportSegments,
    BulkCampaignHistory,
    SelectGeoMapping
}
export enum GlobalChangesAction {
    AppendRules = "APPEND_RULES",
    DeleteRules = "DELETE_RULES",
    DeleteSegments = "DELETE_SEGMENTS",
    FindReplace = "FIND_REPLACE",
    EditSegments = "EDIT_SEGMENTS",
    CampaignHistory = "CAMPAIGN_HISTORY",
    ADD_TO_FAVORITES = "ADD_TO_FAVORITES"
}
