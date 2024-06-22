using System.Diagnostics;
using MultiLevelEncryptedEshop.Dtos;
using Newtonsoft.Json;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace MultiLevelEncryptedEshop.Middlewares;

public class ResponseTimeToBodyMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseTimeToBodyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;
            
            
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

           


            stopwatch.Stop();
            var responseTimeForCompleteRequest = stopwatch.ElapsedMilliseconds;
            var responseTime = responseTimeForCompleteRequest.ToString();

            var modifiedText = ModifyResponse(text,responseTime);

            context.Response.Body = originalBodyStream;

            await context.Response.WriteAsync(modifiedText);
        }
    }

    private string ModifyResponse(string responseBody, string time)
    {
        // Deserialize the response body into your class
        var responseObject = JsonConvert.DeserializeObject<SignInInfoDto>(responseBody);

        if (responseObject != null)
        {
            // Modify the fields as necessary
            responseObject.ResponseTime = time; // Example modification
        }

        // Serialize the modified object back to JSON
        return JsonConvert.SerializeObject(responseObject);
    }
}