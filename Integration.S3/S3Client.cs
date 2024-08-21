namespace Integration.S3
{
    using Microsoft.Extensions.Options;
    using Minio;
    using Minio.DataModel.Args;

    internal sealed class S3Client : IS3Client
    {
        private readonly IMinioClient _minioClient;
        private readonly Dictionary<string, string> _contentTypeMap;

        public S3Client(IMinioClient minioClient, IOptions<S3Config> options)
        {
            _minioClient = minioClient;
            _contentTypeMap = options.Value.ContentTypeMap;
        }

        public async Task PutAsync(PutS3FileArgs args, CancellationToken ct)
        {
            string contentType = args.GetFileContentType(_contentTypeMap);

            bool bucketExists = await BucketExistsAsync(args.Bucket, ct)
                .ConfigureAwait(false);

            if (!bucketExists)
            {
                await CreateBucketAsync(args.Bucket, ct)
                    .ConfigureAwait(false);
            }

            var minioArgs = new PutObjectArgs()
                .WithBucket(args.Bucket)
                .WithObject(args.Name)
                .WithContentType(contentType)
                .WithStreamData(args.Data)
                .WithObjectSize(args.Data.Length);

            await _minioClient.PutObjectAsync(minioArgs, ct)
                .ConfigureAwait(false);
        }

        public async Task<S3File> GetAsync(GetS3FileArgs args, CancellationToken ct)
        {
            var minioArgs = new GetObjectArgs()
                .WithBucket(args.Bucket)
                .WithObject(args.Name)
                .WithCallbackStream(args.StreamCallback);

            var res = await _minioClient.GetObjectAsync(minioArgs, ct)
                .ConfigureAwait(false);

            return new S3File(res.ContentType, res.Size);
        }

        private Task<bool> BucketExistsAsync(string bucket, CancellationToken ct)
        {
            var args = new BucketExistsArgs()
                .WithBucket(bucket);

            return _minioClient.BucketExistsAsync(args, ct);
        }

        private Task CreateBucketAsync(string bucket, CancellationToken ct)
        {
            var args = new MakeBucketArgs()
                .WithBucket(bucket);

            return _minioClient.MakeBucketAsync(args, ct);
        }
    }
}