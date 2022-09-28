using System;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using CloudNative.CloudEvents;
using Google.Cloud.Storage.V1;
using Google.Events.Protobuf.Cloud.Storage.V1;
using StorageImageAnnotator;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        private const string PngContentType = "image/png";
        private const string Mpeg4ContentType = "video/mp4";
        private const string PngExtension = ".png";
        private const string Mpeg4Extension = ".mp4";

        [Test]
        public async Task Test1()
        {
            var logger = new Mock<ILogger<Function>>();
            var videoName = "test_video.mp4";
            var bucketName = "test_bucket";
            var data = new StorageObjectData
            {
                ContentEncoding = "",
                ContentDisposition = "",
                CacheControl = "",
                ContentLanguage = "",
                Metageneration = 0,
                TimeDeleted = Timestamp.FromDateTime(DateTime.UtcNow),
                ContentType = Mpeg4ContentType,
                Size = 0,
                TimeCreated = Timestamp.FromDateTime(DateTime.UtcNow),
                Crc32C = "",
                ComponentCount = 0,
                Md5Hash = "",
                Etag = "",
                Updated = Timestamp.FromDateTime(DateTime.UtcNow),
                StorageClass = "",
                KmsKeyName = "",
                TimeStorageClassUpdated = Timestamp.FromDateTime(DateTime.UtcNow),
                TemporaryHold = false,
                RetentionExpirationTime = Timestamp.FromDateTime(DateTime.UtcNow),
                EventBasedHold = false,
                Name = videoName,
                Id = "",
                Bucket = bucketName,
                Generation = 0,
                CustomerEncryption = null,
                MediaLink = "",
                SelfLink = "",
                Kind = ""
            };

            var cloudEvent = new CloudEvent()
            {
                Type = StorageObjectData.FinalizedCloudEventType,
                Source = new Uri("//storage.googleapis.com", UriKind.RelativeOrAbsolute),
                Id = Guid.NewGuid().ToString(),
                Time = DateTimeOffset.UtcNow,
                Data = data
            };

            var storageClient = StorageClient.CreateUnauthenticated();

            var f = new Function(storageClient, logger.Object);

            await f.HandleAsync(cloudEvent, data, CancellationToken.None);

            logger.Verify(x => x.LogInformation($"Ignoring file {data.Name} with content type {data.ContentType}"), Times.Never);
            var newObjectName = Path.ChangeExtension("test.mp4", PngExtension);
            logger.Verify(x => x.LogInformation($"Created object {newObjectName} with image annotations"));

            Assert.Pass();
        }
    }
}