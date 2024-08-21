namespace Integration.S3.Tests
{
    using System.Net.Mime;

    [Trait("Scope", "Integration.S3")]
    public class S3ClientTest
    {
        private readonly IS3Client _sut;

        public S3ClientTest(IS3Client sut)
        {
            _sut = sut;
        }

        [Fact]
        public async Task PutAndGetAsync()
        {
            // ARRANGE
            const string bucket = "test-bucket";
            const string fileName = "test-obj.jpg";
            const string contentType = MediaTypeNames.Image.Jpeg;
            using var data = GetFileStream();
            long size = data.Length;
            using var downloadStream = new MemoryStream();
            var putArgs = new PutS3FileArgs(bucket, fileName, data);
            var getArgs = new GetS3FileArgs(bucket, fileName, async (sc, ct) =>
            {
                await sc.CopyToAsync(downloadStream, ct);
            });
            var expectedS3File = new S3File(contentType, size);

            // ACT
            var putException = await Record.ExceptionAsync(() => _sut.PutAsync(putArgs, default));
            var s3file = await _sut.GetAsync(getArgs, default);

            // ASSERT
            putException.Should().BeNull(putException?.Message);
            s3file.Should().BeEquivalentTo(expectedS3File);
            downloadStream.Length.Should().Be(s3file.Size);
        }

        private static Stream GetFileStream()
        {
            var assembly = typeof(S3ClientTest).Assembly;
            Guard.Against.Null(assembly);

            var stream = assembly.GetManifestResourceStream("Integration.S3.Tests.radio.jpg");

            return Guard.Against.Null(stream);
        }
    }
}