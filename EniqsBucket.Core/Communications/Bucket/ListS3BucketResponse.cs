using System;
namespace EniqsBucket.Core.Communications.Bucket
{
    public class ListS3BucketResponse
    {
        public string BucketName { get; set; }
        public DateTime TimeStampCreated { get; set; }
    }
}
