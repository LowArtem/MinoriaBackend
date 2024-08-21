using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Api.Extensions.Api;
using MinoriaBackend.Api.Services.ImageStoringService;
using Swashbuckle.AspNetCore.Annotations;

namespace MinoriaBackend.Api.Api.InternalApi.v1.TestImage;

/// <summary>
/// [Internal] Контроллер для тестирования работы с Minio
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute]
[Authorize]
public class TestMinioController : ControllerBase
{
    private readonly IImageStoringService _imageStoringService;

    public TestMinioController(IImageStoringService imageStoringService)
    {
        _imageStoringService = imageStoringService;
    }
    
    /// <summary>
    /// Загрузить изображение
    /// </summary>
    /// <param name="file">файл изображения (jpg,png,jpeg,svg)</param>
    /// <param name="token">токен отмены</param>
    /// <returns>Ссылка на сохраненное изображение</returns>
    [HttpPost("put-image")]
    [SwaggerResponse(200, "Ссылка на сохраненное изображение", typeof(string))]
    [SwaggerResponse(400, "Неверный формат файла")]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Нет доступа")]
    public async Task<IActionResult> PutImageAsync(IFormFile file, CancellationToken token)
    {
        // Проверка расширения
        var validExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".svg" };
        var currentFileExtension = Path.GetExtension(file.FileName);
        if (!validExtensions.Contains(currentFileExtension))
        {
            return BadRequest("Неверный формат файла");
        }
        
        var userId = User.GetUserId();
        
        var url = await _imageStoringService.PutImageAsync(file, userId, "test", token);
        return Ok(url);
    }
    
    /// <summary>
    /// Получить ссылку на изображение
    /// </summary>
    /// <param name="path">путь к изображению</param>
    /// <returns>Ссылка на сохраненное изображение</returns>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerResponse(200, "Ссылка на сохраненное изображение (jpg,png,jpeg,svg)", typeof(string))]
    [SwaggerResponse(404, "Изображение не найдено")]
    public IActionResult GetImageUrl(string path)
    {
        var url = _imageStoringService.GetImageUrl(path);
        if (url == null)
            return NotFound();
        
        return Ok(url);
    }
    
    /// <summary>
    /// Удалить изображение
    /// </summary>
    /// <param name="path">путь к изображению</param>
    /// <param name="token">токен отмены</param>
    /// <returns></returns>
    [HttpDelete]
    [AllowAnonymous]
    [SwaggerResponse(200, "Изображение успешно удалено")]
    [SwaggerResponse(404, "Изображение не найдено")]
    public async Task<IActionResult> DeleteImageAsync(string path, CancellationToken token)
    {
        await _imageStoringService.DeleteImageAsync(path, token);
        return Ok();
    }
}