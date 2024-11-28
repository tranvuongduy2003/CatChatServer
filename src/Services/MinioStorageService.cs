using CatChatServer.Abstractions.Services;
using Minio;
using Minio.DataModel.Args;
using ILogger = Serilog.ILogger;

namespace CatChatServer.Services;

public class MinioStorageService : IFileService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger _logger;

    public MinioStorageService(IMinioClient minioClient, ILogger logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }
    
    public async Task UploadFileAsync(string bucketName, string objectName, Stream data, string contentType)
    {
        try
        {
            bool bucketExists = await BucketExistsAsync(bucketName);
            if (!bucketExists)
            {
                await CreateBucketAsync(bucketName);
            }

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType));

            _logger.Information("File uploaded successfully to {@BucketName}/{@ObjectName}", bucketName, objectName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error uploading file: {@Message}", ex.Message);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string bucketName, string objectName)
    {
        try
        {
            var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(async stream => await stream.CopyToAsync(memoryStream)));

            memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching file: {@Message}", ex.Message);
            throw;
        }
    }

    public async Task DeleteFileAsync(string bucketName, string objectName)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName));
            _logger.Information("File deleted successfully from {@BucketName}/{@ObjectName}", bucketName, objectName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting file: {@Message}", ex.Message);
            throw;
        }
    }

    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        try
        {
            return await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking bucket existence: {@Message}", ex.Message);
            throw;
        }
    }

    public async Task CreateBucketAsync(string bucketName)
    {
        try
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            _logger.Information("Bucket {@BucketName} created successfully", bucketName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating bucket: {@Message}", ex.Message);
            throw;
        }
    }
}
