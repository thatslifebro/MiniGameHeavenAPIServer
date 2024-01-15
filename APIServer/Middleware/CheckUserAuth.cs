using APIServer.Model.DAO;
using APIServer.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIServer.Middleware;

public class CheckUserAuthAndLoadUserData
{
    readonly IMemoryDb _memoryDb;
    readonly RequestDelegate _next;

    public CheckUserAuthAndLoadUserData(RequestDelegate next, IMemoryDb memoryDb)
    {
        _memoryDb = memoryDb;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        //로그인, 회원가입 api는 토큰 검사를 하지 않는다.
        var formString = context.Request.Path.Value;
        if (string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0 ||
            string.Compare(formString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0)
        {
            // Call the next delegate/middleware in the pipeline
            await _next(context);

            return;
        }

        // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
        // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
        context.Request.EnableBuffering();

        var userLockKey = "";

        using (StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 4096, true))
        {
            string bodyStr = await reader.ReadToEndAsync();

            // body String에 어떤 문자열도 없을때
            if (await IsNullBodyDataThenSendError(context, bodyStr))
            {
                return;
            }

            JsonDocument document = JsonDocument.Parse(bodyStr);

            //uid와 토큰이 없을때
            if (IsInvalidJsonFormatThenSendError(context, document, out int uid, out string token))
            {
                return;
            }

            //uid를 키로 하는 데이터 없을 때
            (bool isOk, RdbAuthUserData userInfo) = await _memoryDb.GetUserAsync(uid);
            if (await IsInvalidUserAuthTokenNotFound(context, isOk))
            {
                return;
            }

            //토큰이 일치하지 않을 때
            if (await IsInvalidUserAuthTokenThenSendError(context, userInfo, token))
            {
                return;
            }
            //이번 api 호출 끝날 때까지 redis키 잠금 만약 이미 잠겨있다면 에러
            userLockKey = Services.MemoryDbKeyMaker.MakeUserLockKey(userInfo.Uid.ToString());
            if (await SetLockAndIsFailThenSendError(context, userLockKey))
            {
                return;
            }

            context.Items[nameof(RdbAuthUserData)] = userInfo;
        }
        // 읽음 위치 초기화
        context.Request.Body.Position = 0;

        // Call the next delegate/middleware in the pipeline
        await _next(context);

        // 트랜잭션 해제(Redis 동기화 해제)
        _ = await _memoryDb.DelUserReqLockAsync(userLockKey);
    }

    async Task<bool> SetLockAndIsFailThenSendError(HttpContext context, string AuthToken)
    {
        if (await _memoryDb.SetUserReqLockAsync(AuthToken))
        {
            return false;
        }
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenFailSetNx
        });
        await context.Response.WriteAsync(errorJsonResponse);
        return true;
    }

    async Task<bool> IsInvalidUserAuthTokenThenSendError(HttpContext context, RdbAuthUserData userInfo, string token)
    {
        if (string.CompareOrdinal(userInfo.Token, token) == 0)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenFailWrongAuthToken
        });
        await context.Response.WriteAsync(errorJsonResponse);

        return true;
    }

    bool IsInvalidJsonFormatThenSendError(HttpContext context, JsonDocument document, out int uid,
        out string token)
    {
        try
        {
            uid = document.RootElement.GetProperty("uid").GetInt32();
            token = document.RootElement.GetProperty("token").GetString();
            return false;
        }
        catch
        {
            uid = 0; token = "";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailWrongKeyword
            });
            context.Response.WriteAsync(errorJsonResponse);

            return true;
        }
    }

    async Task<bool> IsNullBodyDataThenSendError(HttpContext context, string bodyStr)
    {
        if (string.IsNullOrEmpty(bodyStr) == false)
        {
            return false;
        }
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.InValidRequestHttpBody
        });
        await context.Response.WriteAsync(errorJsonResponse);

        return true;
    }

    async Task<bool> IsInvalidUserAuthTokenNotFound(HttpContext context, bool isOk)
    {
        if (!isOk)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenKeyNotFound
            });
            await context.Response.WriteAsync(errorJsonResponse);
        }
        return !isOk;
    }


    class MiddlewareResponse
    {
        public ErrorCode result { get; set; }
    }
}
