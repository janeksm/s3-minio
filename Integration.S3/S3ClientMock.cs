namespace Integration.S3
{
    using Microsoft.Extensions.Options;

    internal sealed class S3ClientMock : IS3Client
    {
        private static readonly Dictionary<string, (S3File File, byte[] Data)> _data = [];
        private readonly Dictionary<string, string> _contentTypeMap;

        public S3ClientMock(IOptions<S3Config> options)
        {
            _contentTypeMap = options.Value.ContentTypeMap;
        }

        public async Task PutAsync(PutS3FileArgs args, CancellationToken ct)
        {
            string contentType = args.GetFileContentType(_contentTypeMap);
            string key = $"{args.Bucket}:{args.Name}";

            var file = new S3File(contentType, args.Data.Length);

            using var ms = new MemoryStream();
            await args.Data.CopyToAsync(ms, ct)
                .ConfigureAwait(false);
            byte[] data = ms.ToArray();

            _data.TryAdd(key, (file, data));
        }

        public async Task<S3File> GetAsync(GetS3FileArgs args, CancellationToken ct)
        {
            string key = $"{args.Bucket}:{args.Name}";

            var found = _data[key];

            using var ms = new MemoryStream(found.Data);
            await args.StreamCallback(ms, ct)
                .ConfigureAwait(false);

            return found.File;
        }
    }
}