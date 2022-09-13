namespace Infogroup.IDMS.Campaigns
{
    public enum CampaignOESSConsts
    {
        New = 0,
        Saved = 10,
        SubmittedToCredit = 20,
        ApprovedByCredit = 30,
        RejectedByCredit = 40,
        Invoiced = 50
    }

    public static class CampaignOESSDescription
    {
        public const string New = "New";
        public const string RejectedByCredit = "Rejected by Credit";
        public const string Saved = "Saved";
        public const string SubmittedToCredit = "Submitted to Credit";
        public const string ApprovedByCredit = "Approved by Credit";
        public const string Invoiced = "Invoiced";
    }
}