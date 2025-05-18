using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb3Moln;

public class LotrFunctions
{
    private readonly LotrServices _lotrServices;

    public LotrFunctions()
    {
        string connectionString = Environment.GetEnvironmentVariable("MongoDbConnection");
        string databaseName = "lotr";
        _lotrServices = new LotrServices(connectionString, databaseName);
    }

    [Function("GetLotr")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData request)
    {
        if (request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            var lotr = await _lotrServices.GetLotrAsync();
            await response.WriteAsJsonAsync(lotr);
            return response;
        }

        else if (request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {

            try
            {
                var requestBody = await request.ReadAsStringAsync();
                var newLotr = JsonSerializer.Deserialize<Lotr>(requestBody);

                if (newLotr == null ||
                    string.IsNullOrEmpty(newLotr.Id) ||
                    string.IsNullOrWhiteSpace(newLotr.Name) ||
                    string.IsNullOrWhiteSpace(newLotr.Type))

                {
                    var badRequest = request.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteStringAsync("Id, Name and Type are required.");
                    return badRequest;
                }

                var existinglotr = await _lotrServices.GetByIdAsync(newLotr.Id);
                if (existinglotr != null)
                {
                    var conflict = request.CreateResponse(HttpStatusCode.Conflict);
                    await conflict.WriteStringAsync("A character with this ID already exists.");
                    return conflict;
                }

                await _lotrServices.CreateLotrAsync(newLotr);

                var response = request.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(newLotr);
                return response;
            
            }

            catch (Exception ex)
            {
                var error = request.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync($"Error: {ex.Message}");
                return error;
            }
        }
        var methodNotAllowed = request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        await methodNotAllowed.WriteStringAsync("Only Get and Post methods are allowed.");
        return methodNotAllowed;
    }
}