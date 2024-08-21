namespace MinoriaBackend.Api.Services.ImageStoringService;

/// <summary>
/// Сервис хранения изображений
/// </summary>
public interface IImageStoringService
{
    /// <summary>
    /// Сохранить изображение
    /// </summary>
    /// <param name="file">файл изображения</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="imageSet">название бакета (папка для хранения)</param>
    /// <param name="cancellationToken">токен отмены операции</param>
    /// <returns>путь к изображению (null если не удалось сохранить)</returns>
    public Task<string?> PutImageAsync(IFormFile file, Guid userId, string imageSet, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить изображение
    /// </summary>
    /// <param name="path">путь к изображению (bucket/object)</param>
    /// <param name="cancellationToken">токен отмены операции</param>
    /// <returns></returns>
    public Task DeleteImageAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить ссылку на изображение
    /// </summary>
    /// <param name="path">путь к изображению (bucket/object)</param>
    /// <returns>ссылку на изображение (null если не удалось получить)</returns>
    public string? GetImageUrl(string path);
}