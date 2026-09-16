using Microsoft.AspNetCore.Diagnostics;
using TechCart.Api.Common.ResponseDtos;
using TechCart.SharedKernel;

namespace TechCart.Api.Common;

public class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Exception buraya duser. AppException ise response'u (code, message) formatinda yazip true doner
        if (exception is not AppException appException)
            return false;

        httpContext.Response.StatusCode = appException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new AppExceptionResponse(appException.Code, appException.Message),
            cancellationToken);

        return true;
    }
}
