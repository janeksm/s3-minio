namespace Integration.S3
{
    public interface IS3Client
    {
        Task<S3File> GetAsync(GetS3FileArgs args, CancellationToken ct);

        Task PutAsync(PutS3FileArgs args, CancellationToken ct);
    }

    public record PutS3FileArgs(string Bucket, string Name, Stream Data)
    {
        public string GetFileContentType(IDictionary<string, string> contentTypeMap)
        {
            var fi = new FileInfo(Name);
            string extension = fi.Extension[1..].ToLower();
            Guard.Against.NullOrEmpty(extension);

            if (!contentTypeMap.TryGetValue(extension, out string? contentType))
            {
                throw new ArgumentException("File.Name", $"File extension '{extension}' is not defined in S3 configuration");
            }

            return Guard.Against.NullOrEmpty(contentType);
        }
    }

    public record GetS3FileArgs(string Bucket, string Name, Func<Stream, CancellationToken, Task> StreamCallback);

    public record S3File(string ContentType, long Size);
}