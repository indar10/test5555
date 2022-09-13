using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ModelStatuss
{
    public enum ModelStatus
    {
        ModelCreated = 10,
        SampleSubmitted = 20,
        SampleRunning = 30,
        SampleCompleted = 40,
        SampleFailed = 50,
        DatabaseSubmitted = 60,
        DatabaseRunning = 70,
        DatabaseCompleted = 80,
        DatabaseFailed = 90
    }

    public static class ModelStatusNames
    {
        public const string ModelCreated = "10: New Model";
        public const string SampleSubmitted = "20: Sample Submitted";
        public const string SampleRunning = "30: Sample Running";
        public const string SampleCompleted = "40: Sample Scored";
        public const string SampleFailed = "50: Sample Scoring Failed";
        public const string DatabaseSubmitted = "60: Submitted";
        public const string DatabaseRunning = "70: Running";
        public const string DatabaseCompleted = "80: Database Scored";
        public const string DatabaseFailed = "90: Scoring Failed";
    }
}
