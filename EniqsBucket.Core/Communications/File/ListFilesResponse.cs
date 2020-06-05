 using System;
namespace EniqsBucket.Core.Communications.File
{
    public class ListFilesResponse
    {
        public string Key { get; set; }
        public string Owner { get; set; }
        public string BucketName { get; set; }
        public long Size { get; set; }
    }
}
