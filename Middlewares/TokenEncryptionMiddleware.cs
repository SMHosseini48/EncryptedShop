using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MultiLevelEncryptedEshop.Dtos;
using MultiLevelEncryptedEshop.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            context.Request.Headers.Authorization = token.Split()[0] + " " + Decrypt(token.Split()[1]);
        }


        var originalBodyStream = context.Response.Body;

        // Create a new memory stream to intercept the response
        using (var memoryStream = new MemoryStream())
        {
            context.Response.Body = memoryStream;

            await _next(context);
            // Read the response stream
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            // Modify the response body

            var modifiedResponseBody = ModifyAccessToken(responseBody);

            // Write the modified response back to the original stream
            await context.Response.WriteAsync(modifiedResponseBody, Encoding.UTF8);

            // Reset the response body stream to the original stream
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    public bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) // For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException) // Parsing failed
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private string ModifyAccessToken(string responseBody)
    {
        // Deserialize the response body into your class
        try
        {
            var responseObject = JsonConvert.DeserializeObject<SignInInfoDto>(responseBody);
            // Modify the fields as necessary
            responseObject.AccessToken = Encrypt(responseObject.AccessToken);
            return JsonConvert.SerializeObject(responseObject);
        }
        catch (Exception e)
        {
            return responseBody;
        }
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