using System;
using System.Diagnostics.Eventing.Reader;
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Storage.V1;
using Google.Events.Protobuf.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace StorageImageAnnotator
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services.AddSingleton(StorageClient.Create());
    }

    [FunctionsStartup(typeof(Startup))]
    public class Function : ICloudEventFunction<StorageObjectData>
    {
        private const string PngContentType = "image/png";
        private const string Mpeg4ContentType = "video/mp4";
        private const string PngExtension = ".png";
        private const string Mpeg4Extension = ".mp4";

        private readonly StorageClient _storageClient;
        private readonly ILogger _logger;

        public Function(StorageClient storageClient, ILogger<Function> logger)
        {
            (_storageClient, _logger) = (storageClient, logger);
        }

        public async Task HandleAsync(CloudEvent cloudEvent, StorageObjectData data,
            CancellationToken cancellationToken)
        {
            if (data.ContentType != Mpeg4ContentType || Path.GetExtension(data.Name) != Mpeg4Extension)
            {
                _logger.LogInformation($"Ignoring file {data.Name} with content type {data.ContentType}");
                return;
            }

            var newObjectName = await ReUnfold(data, cancellationToken);
            _logger.LogInformation($"Created object {newObjectName} with image annotations");
        }

        private async Task<string> ReUnfold(StorageObjectData payload, CancellationToken token)
        {
            using var capture = new VideoCapture($"gs://{payload.Bucket}/{payload.Name}");

            ////　テスト用
            //using var capture = new VideoCapture(@"Resources/test_video.mp4");

            // 処理
            using var mat = new Mat();
            capture.PosFrames = 1;
            capture.Read(mat);

            // ストレージにアップロード
            using var stream = new MemoryStream();
            using var bitmap = mat.ToBitmap();
            bitmap.Save(stream, ImageFormat.Png);

            var name = Path.ChangeExtension(payload.Name, PngExtension);
            await _storageClient.UploadObjectAsync(payload.Bucket, name, PngContentType, stream,
                cancellationToken: token);

            return name;
        }
    }
}