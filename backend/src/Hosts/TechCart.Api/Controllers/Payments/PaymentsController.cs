using Microsoft.AspNetCore.Mvc;
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Application.ProcessCallback;

namespace TechCart.Api.Controllers.Payments;

[ApiController]
[Route("api/payments/iyzico")]
public class PaymentsController(ProcessPaymentCallbackHandler processPaymentCallbackHandler) : ControllerBase
{
    // [Authorize] yok — bu, iyzico'nun kullanıcının tarayıcısını
    // yönlendirdiği bir adres, bizim JWT token'ımızı taşımıyor. Güvenlik, iyzico'nun ürettiği
    // token değerinin kendisinin doğrulanmasından geliyor
    [HttpPost("callback")]
    public async Task<IActionResult> Callback([FromQuery] Guid orderId, [FromForm] string token, CancellationToken ct)
    {
        var isSuccessful = await processPaymentCallbackHandler.Handle(
            new ProcessPaymentCallbackCommand(token, orderId), ct);

        var resultParam = isSuccessful ? "success" : "failed";

        // Tarayıcıyı, sonucu göreceği frontend sayfasına yönlendiriyoruz —
        return Redirect($"http://localhost:3000/checkout/3d-secure?status={resultParam}");
    }
}