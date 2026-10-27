using Microsoft.AspNetCore.Mvc;
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Application.ProcessCallback;

namespace TechCart.Api.Controllers.Payments;

[ApiController]
[Route("api/payments/iyzico")]
public class PaymentsController(
    ProcessPaymentCallbackHandler processPaymentCallbackHandler,
    IConfiguration configuration) : ControllerBase
{
    // [Authorize] yok — bu, iyzico'nun kullanıcının tarayıcısını
    // yönlendirdiği bir adres, bizim JWT token'ımızı taşımıyor. Güvenlik, iyzico'nun ürettiği
    // token değerinin kendisinin doğrulanmasından geliyor
    [HttpPost("callback")]
    public async Task<IActionResult> Callback([FromQuery] Guid orderId, [FromForm] string token, CancellationToken ct)
    {
        var frontendBaseUri = new Uri(
            configuration["PublicUrls:FrontendBaseUrl"]!,
            UriKind.Absolute);

        var isSuccessful = await processPaymentCallbackHandler.Handle(
            new ProcessPaymentCallbackCommand(token, orderId), ct);

        var resultParam = isSuccessful ? "success" : "failed";

        // Tarayıcıyı, sonucu göreceği frontend sayfasına yönlendiriyoruz —
        var resultUrl = new Uri(
            frontendBaseUri,
            $"/checkout/3d-secure?status={resultParam}");

        return Redirect(resultUrl.AbsoluteUri);
    }
}