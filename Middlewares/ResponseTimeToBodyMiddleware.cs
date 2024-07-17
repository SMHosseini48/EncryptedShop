using System.Diagnostics;
using MultiLevelEncryptedEshop.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var modifiedText = ModifyResponse(text, responseTime);

            context.Response.Body = originalBodyStream;

            await context.Response.WriteAsync(modifiedText);
        }
    }

    private string ModifyResponse(string responseBody, string time)
    {
        // Deserialize the response body into your class
        JObject jsonObject = JObject.Parse(responseBody);

// Append the new field
        jsonObject["ResponseTime"] = time;
        return jsonObject.ToString();
        // Serialize the modified object back to JSON
    }
}