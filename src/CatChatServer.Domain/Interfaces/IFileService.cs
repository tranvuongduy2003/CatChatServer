namespace CatChatServer.Domain.Interfaces;

public interface IFileService
{
    Task UploadFileAsync(string bucketName, string objectName, Stream data, string contentType);
    
    Task<Stream> GetFileAsync(string bucketName, string objectName);
    
    Task DeleteFileAsync(string bucketName, string objectName);
    
    Task<bool> BucketExistsAsync(string bucketName);
    
    Task CreateBucketAsync(string bucketName);
}
