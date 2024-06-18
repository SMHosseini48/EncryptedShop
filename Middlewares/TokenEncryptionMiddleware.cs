using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MultiLevelEncryptedEshop.Dtos;
using MultiLevelEncryptedEshop.Models;

namespace MultiLevelEncryptedEshop.Middlewares;

public class TokenEncryptionMiddleware
{
    private readonly RequestDelegate _next;

    public TokenEncryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private readonly static List<string[]> CharacterConversions = new List<string[]>
    {
        new string[] {"a", "lkk"},
        new string[] {"b", "qet"},
        new string[] {"c", "qic"},
        new string[] {"d", "las"},
        new string[] {"e", "eac"},
        new string[] {"f", "lac"},
        new string[] {"g", "imi"},
        new string[] {"h", "qdm"},
        new string[] {"i", "uyd"},
        new string[] {"j", "fot"},
        new string[] {"k", "wcp"},
        new string[] {"l", "pdh"},
        new string[] {"m", "nsk"},
        new string[] {"n", "wnl"},
        new string[] {"o", "atk"},
        new string[] {"p", "sjp"},
        new string[] {"q", "pij"},
        new string[] {"r", "wjd"},
        new string[] {"s", "fwx"},
        new string[] {"t", "fsv"},
        new string[] {"u", "zzt"},
        new string[] {"v", "rqe"},
        new string[] {"w", "jnk"},
        new string[] {"x", "krm"},
        new string[] {"y", "cvb"},
        new string[] {"z", "gvq"},
        new string[] {"A", "fhw"},
        new string[] {"B", "mma"},
        new string[] {"C", "xfd"},
        new string[] {"D", "tzh"},
        new string[] {"E", "ucr"},
        new string[] {"F", "vpm"},
        new string[] {"G", "zlx"},
        new string[] {"H", "pqv"},
        new string[] {"I", "pbk"},
        new string[] {"J", "esg"},
        new string[] {"K", "uft"},
        new string[] {"L", "tie"},
        new string[] {"M", "arl"},
        new string[] {"N", "mou"},
        new string[] {"O", "aff"},
        new string[] {"P", "fdx"},
        new string[] {"Q", "zly"},
        new string[] {"R", "ekq"},
        new string[] {"S", "lec"},
        new string[] {"T", "kim"},
        new string[] {"U", "ygl"},
        new string[] {"V", "khp"},
        new string[] {"W", "tim"},
        new string[] {"X", "idm"},
        new string[] {"Y", "yrx"},
        new string[] {"Z", "wiy"},
        new string[] {"-", "prc"},
        new string[] {"_", "gqi"},
        new string[] {".", "xeh"},
        new string[] {"0", "123"},
        new string[] {"1", "456"},
        new string[] {"2", "234"},
        new string[] {"3", "345"},
        new string[] {"4", "567"},
        new string[] {"5", "678"},
        new string[] {"6", "789"},
        new string[] {"7", "987"},
        new string[] {"8", "876"},
        new string[] {"9", "654"}
    };


    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.Authorization.ToString().IsNullOrEmpty())
        {
            var token = context.Request.Headers.Authorization.ToString();
            context.Request.Headers.Authorization = token.Split()[0] +" "+ Decrypt(token.Split()[1]);
        }

        //     // Store the original body stream for restoring the response body back to its original stream
        //     var originalBodyStream = context.Response.Body;
        //
        //     // Create new memory stream for reading the response; Response body streams are write-only, therefore memory stream is needed here to read
        //     await using var memoryStream = new MemoryStream();
        //     context.Response.Body = memoryStream;
        //
        //     // Call the next middleware
        //     
        //
        //     // Set stream pointer position to 0 before reading
        //     memoryStream.Seek(0, SeekOrigin.Begin);
        //
        //     // Read the body from the stream
        //     var responseBodyText = await new StreamReader(memoryStream).ReadToEndAsync();
        //
        //     // Reset the position to 0 after reading
        //     memoryStream.Seek(0, SeekOrigin.Begin);
        //
        //     // Do this last, that way you can ensure that the end results end up in the response.
        //     // (This resulting response may come either from the redirected route or other special routes if you have any redirection/re-execution involved in the middleware.)
        //     // This is very necessary. ASP.NET doesn't seem to like presenting the contents from the memory stream.
        //     // Therefore, the original stream provided by the ASP.NET Core engine needs to be swapped back.
        //     // Then write back from the previous memory stream to this original stream.
        //     // (The content is written in the memory stream at this point; it's just that the ASP.NET engine refuses to present the contents from the memory stream.)
        //     context.Response.Body = originalBodyStream;
        //     await originalBodyStream.CopyToAsync(memoryStream);
        //
        //     // Per @Necip Sunmaz's recommendation this also works:
        //     // Just make sure that the memoryStrream's pointer position is set back to 0 again.
        //     // await memoryStream.CopyToAsync(originalBodyStream);
        //     // context.Response.Body = originalBodyStream;
        //
        //
        //     // await using (Stream originalResponseStreamClone = context.Response.Body)
        //     // {
        //     //     context.Response.Body = new MemoryStream();
        //     //     await originalResponseStreamClone.CopyToAsync(memoryStream);
        //     //     var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        //     //     var responseObject = JsonSerializer.Deserialize<SignInInfoDto>(responseBody);
        //     //     if (responseObject.GetType() == typeof(SignInInfoDto))
        //     //     {
        //     //         //cipher the token with  the table 
        //     //
        //     //         var encryptedToken = Encrypt(responseObject.AccessToken);
        //     //         var newResponseBody = JsonSerializer.Serialize(new SignInInfoDto
        //     //         {
        //     //             AccessToken = encryptedToken,
        //     //             RefreshToken = responseObject.RefreshToken
        //     //         });
        //     //
        //     //         var stream = new MemoryStream();
        //     //         var streamWriter = new StreamWriter(stream);
        //     //         await streamWriter.WriteAsync(newResponseBody);
        //     //         await streamWriter.FlushAsync();
        //     //         stream.Position = 0;
        //     //         context.Response.Body = stream;
        //     //     }
        //     // }
        //     //
        //     //
        //     // Console.WriteLine($"{context.Response.Body.CanRead}\n{context.Response.Body.CanSeek}");
        //
        //
        // }

        await _next(context);
    }

    public static string Encrypt(string token)
    {
        var stringList = token.ToCharArray()
            .Select(s => CharacterConversions.FirstOrDefault(x => x[0] == s.ToString())[1]);
        return String.Join("", stringList.ToArray());
    }

    public static string Decrypt(string encryptedToken)
    {
        var subStrings = Enumerable.Range(0, (encryptedToken.Length + 2) / 3)
            .Select(i => encryptedToken.Substring(i * 3, Math.Min(3, encryptedToken.Length - i * 3)))
            .ToList().Select(x => CharacterConversions.FirstOrDefault(s => s[1] == x)[0]);

        return String.Join("", subStrings.ToArray());
    }
}