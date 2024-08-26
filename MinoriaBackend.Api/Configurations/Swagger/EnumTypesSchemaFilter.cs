using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinoriaBackend.Api.Configurations.Swagger;

/// <summary>
/// Swashbuckle Schema Filter
/// </summary>
public class EnumTypesSchemaFilter : ISchemaFilter
{
    private readonly XDocument? _xmlComments = null;

    /// <inheritdoc />
    public EnumTypesSchemaFilter(string xmlPath)
    {
        if (File.Exists(xmlPath))
        {
            _xmlComments = XDocument.Load(xmlPath);
        }
    }

    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (_xmlComments == null) return;

        if (schema.Enum != null && schema.Enum.Count > 0 &&
            context.Type != null && context.Type.IsEnum)
        {
            schema.Description += "<p>Members:</p><ul>";

            var fullTypeName = context.Type.FullName;

            // Создаём новый список для преобразованных значений enum
            var lowerEnumValues = new List<IOpenApiAny>();

            foreach (var enumMember in schema.Enum.OfType<OpenApiString>())
            {
                var originalEnumMemberName = enumMember.Value;
                var lowerEnumMemberName = originalEnumMemberName.ToLower(); // Приведение имени к нижнему регистру
                lowerEnumValues.Add(new OpenApiString(lowerEnumMemberName)); // Добавляем преобразованное значение в новый список

                var fullEnumMemberName = $"F:{fullTypeName}.{originalEnumMemberName}"; // Используем оригинальное имя для поиска комментария

                var enumMemberComments = _xmlComments.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")!.Value.Equals(fullEnumMemberName, StringComparison.OrdinalIgnoreCase));
                if (enumMemberComments == null) continue;

                var summary = enumMemberComments.Descendants("summary").FirstOrDefault();
                if (summary == null) continue;

                schema.Description += $"<li><i>{lowerEnumMemberName}</i> - {summary.Value.Trim()}</li>"; // Используем имя в нижнем регистре в описании
            }

            schema.Description += "</ul>";

            // Заменяем оригинальные значения enum на новые, приведённые к нижнему регистру
            schema.Enum.Clear();
            foreach (var lowerEnumValue in lowerEnumValues)
            {
                schema.Enum.Add(lowerEnumValue);
            }
        }
    }
}