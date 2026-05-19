using System.Net.Http.Headers;
using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class ArchivosClientService(HttpClient client)
{
    public async Task<List<Archivo>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Archivo>>(client, "api/archivos");
    }

    public async Task<Archivo?> GetAsync(int id)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Archivo>(client, $"api/archivos/{id}/detalle");
    }

    public async Task PostAsync(Upload Archivo)
    {
        var memoryStream = new MemoryStream();
        await Archivo.Portada.CopyToAsync(memoryStream);
        var Contenido = memoryStream.ToArray();
        memoryStream.Close();

        var fileContent = new ByteArrayContent(Contenido);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Archivo.Portada.ContentType);

        using var form = new MultipartFormDataContent
        {
            { fileContent, "file", Archivo.Portada.FileName! }
        };

        await ApiErrorMapper.PostAsync(client, "api/archivos", form);
    }

    public async Task PutAsync(Upload Archivo)
    {
        var memoryStream = new MemoryStream();
        await Archivo.Portada.CopyToAsync(memoryStream);
        var Contenido = memoryStream.ToArray();
        memoryStream.Close();

        var fileContent = new ByteArrayContent(Contenido);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Archivo.Portada.ContentType);

        using var form = new MultipartFormDataContent
        {
            { fileContent, "file", Archivo.Portada.FileName! }
        };

        await ApiErrorMapper.SendPutMultipartAsync(client, $"api/archivos/{Archivo.ArchivoId}", form);
    }

    public async Task DeleteAsync(int id)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/archivos/{id}");
    }
}
