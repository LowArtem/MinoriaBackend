using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Newtonsoft.Json;

namespace MinoriaBackend.Api.Services.ImageStoringService;

/// <summary>
/// Сервис хранения изображений с помощью Minio
/// </summary>
public class MinioService : IImageStoringService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioService> _logger;
    
    /// <summary>
    /// Сервис хранения изображений с помощью Minio
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    public MinioService(IServiceProvider serviceProvider, ILogger<MinioService> logger)
    {
        _logger = logger;
        _minioClient = serviceProvider.GetRequiredService<IMinioClient>();
    }

    /// <inheritdoc />
    public async Task<string?> PutImageAsync(IFormFile file, Guid userId, string imageSet, CancellationToken token)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(userId.ToString());
            
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, token);
            if (!bucketExists)
            {
                await CreateBucketAsync(userId, token);
            }
            
            // Загружайте файл в Minio после проверки или создания bucket
            var fileName = imageSet + "/" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(userId.ToString())
                .WithObject(fileName)
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length);
            await _minioClient.PutObjectAsync(putObjectArgs, token);
            
            _logger.LogInformation("Uploaded image {FileName} to bucket {BucketName}", fileName, imageSet);

            // Возвращаем путь загруженного изображения
            return $"{userId.ToString()}/{fileName}";
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Failed to put image to Minio");
            return null;
        }
    }
    
    /// <summary>
    /// Создать bucket в Minio
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="token"></param>
    private async Task CreateBucketAsync(Guid userId, CancellationToken token)
    {
        var createBucketArgs = new MakeBucketArgs()
            .WithBucket(userId.ToString());
        await _minioClient.MakeBucketAsync(createBucketArgs, token);

        // Установим публичный доступ к bucket
        var policy = new Dictionary<string, object>
        {
            { "Version", "2012-10-17" },
            { "Statement", new List<Dictionary<string, object>>
                {
                    new()
                    {
                        { "Effect", "Allow" },
                        { "Principal", new { AWS = "*" } },
                        { "Action", "s3:GetObject" },
                        { "Resource", $"arn:aws:s3:::{userId.ToString()}/*" }
                    }
                }
            }
        };

        var policyJson = JsonConvert.SerializeObject(policy);

        var setPolicyArgs = new SetPolicyArgs()
            .WithBucket(userId.ToString())
            .WithPolicy(policyJson);
        await _minioClient.SetPolicyAsync(setPolicyArgs, token);
        
        _logger.LogInformation("Created bucket {BucketName}", userId.ToString());
    }

    /// <inheritdoc />
    public async Task DeleteImageAsync(string path, CancellationToken token)
    {
        try
        {
            var (userId, imageSet, objectName) = ParsePath(path);

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(userId.ToString())
                .WithObject(imageSet + "/" + objectName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, token);

            _logger.LogInformation("Deleted object {ObjectName} from bucket {BucketName}", objectName, userId.ToString());
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Failed to delete image {Path} from Minio", path);
        }
    }

    /// <inheritdoc />
    public string? GetImageUrl(string path)
    {
        try
        {
            // Разделяем путь на bucket и имя объекта
            var (userId, imageSet, objectName) = ParsePath(path);

            // Формируем URL для доступа к объекту
            return $"{_minioClient.Config.Endpoint}/{userId.ToString()}/{imageSet}/{objectName}";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate image URL");
            return null;
        }
    }
    
    /// <summary>
    /// Вспомогательный метод для разбора пути на bucket и объект
    /// </summary>
    private static (Guid userId, string imageSet, string objectName) ParsePath(string path)
    {
        // Ожидаемый формат path: "bucketName/objectName"
        var parts = path.Split('/', 3);
        var isUserIdCorrect = Guid.TryParse(parts[0], out var userId);
        
        if (parts.Length != 3 || !isUserIdCorrect)
        {
            throw new ArgumentException("Invalid path format. Expected 'userId/imageSet/objectName'.");
        }

        return (userId, parts[1], parts[2]);
    }
}