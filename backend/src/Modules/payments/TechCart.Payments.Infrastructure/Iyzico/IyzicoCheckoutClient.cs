using System.Globalization;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Options;
using TechCart.Payments.Application.Abstractions;
using TechCart.Payments.Application.Dtos.ResponseDtos;

namespace TechCart.Payments.Infrastructure.Iyzico;

public class IyzicoCheckoutClient : IPaymentGateway
{
    // _iyzipayOptions -> iyzico'ya gonderilecek her istekte kimlik dogrulama ve Api adresi icin kullanilir
    private readonly Iyzipay.Options _iyzipayOptions;
    
    // TechCart ile iyzico arasinda baglanti kurarak kullanici icin iyzico odeme sayfasi olusturur
    public IyzicoCheckoutClient(IOptions<IyzicoOptions> options)
    {
        var settings = options.Value;

        // Iyzipay paketinin KENDİ "Options" sınıfı — bizim IyzicoOptions'tan
        // (appsettings'ten okuduğumuz ham değerler) farklı, paketin
        // beklediği asıl nesne. Her istekte bunu kullanacağız.
        _iyzipayOptions = new Iyzipay.Options()
        {
            ApiKey = settings.ApiKey,
            SecretKey = settings.SecretKey,
            BaseUrl = settings.BaseUrl
        };
    }
    
    // callbackUrl -> iyzico odeme bittiginde kullaniciyi nereye gondersin
    // Bu adres yalnızca kullanıcıyı yönlendirmek için değil, iyzico’nun gönderdiği
    // token üzerinden ödeme sonucunu doğrulamak için de kullanılır.
    // Yani callback’e gelinmesi tek başına ödemenin başarılı olduğu anlamına gelmez;
    // callback’te alınan token ile iyzico’dan sonuç ayrıca sorgulanmalıdır.
    
    // bu fonksiyon alici biglileri ve bir callbackUrl parametre olarak alir.
    // kullanici icin bir bir iyzico checkout form odeme oturumu baslatir
    public async Task<Contracts.ResponseDtos.CheckoutInitResponse> InitializeCheckoutAsync(
        string conversationId, decimal price, string buyerName, string buyerSurname,
        string buyerEmail, string buyerPhone, string buyerAddress, string buyerCity,
        string callbackUrl, List<(string Name, decimal Price)> basketItems, CancellationToken ct)
    {
        // odemesi sayfasi olusturulurken olusan nesne
        var request = new CreateCheckoutFormInitializeRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = conversationId, // techcart'daki odemeyle iyzico islemini iliskilendiren takip numarasidir
            Price = price.ToString("F2", CultureInfo.InvariantCulture),
            PaidPrice = price.ToString("F2", CultureInfo.InvariantCulture),            Currency = Currency.TRY.ToString(),
            BasketId = conversationId,
            PaymentGroup = PaymentGroup.PRODUCT.ToString(),
            CallbackUrl = callbackUrl,

            Buyer = new Buyer
            {
                Id = conversationId,
                Name = buyerName,
                Surname = buyerSurname,
                Email = buyerEmail,
                GsmNumber = buyerPhone,
                RegistrationAddress = buyerAddress,
                City = buyerCity,
                Country = "Turkey",
                Ip = "127.0.0.1" ,// sandbox'ta gerçek IP zorunlu değil
                IdentityNumber = "11111111111"
            },

            ShippingAddress = new Address
            {
                ContactName = $"{buyerName} {buyerSurname}",
                City = buyerCity,
                Country = "Turkey",
                Description = buyerAddress
            },

            BillingAddress = new Address
            {
                ContactName = $"{buyerName} {buyerSurname}",
                City = buyerCity,
                Country = "Turkey",
                Description = buyerAddress
            },

            BasketItems = basketItems.Select(item => new BasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = item.Name,
                Category1 = "Teknoloji",
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = item.Price.ToString("F2", CultureInfo.InvariantCulture)
                
            }).ToList()
        };

        var result = await CheckoutFormInitialize.Create(request, _iyzipayOptions);
        if (result.Status != "success")
        {
            throw new InvalidOperationException(
                $"iyzico ödeme başlatma başarısız: {result.ErrorMessage} (ErrorCode: {result.ErrorCode})");
        }

        return new Contracts.ResponseDtos.CheckoutInitResponse(result.PaymentPageUrl, result.Token);    }

    // iyzico odeme ekrani tamamlandiktan sorna callback adresine gonderilen token degeriyle
    // odemenin gercek sonucunu iyzico'dan sorgular
    public async Task<PaymentResultDto> RetrieveCheckoutResultAsync(string token, CancellationToken ct)
    {
        var request = new RetrieveCheckoutFormRequest { Token = token };

        var result = await CheckoutForm.Retrieve(request, _iyzipayOptions);

        var isSuccessful = result.Status == "success" && result.PaymentStatus == "SUCCESS";

        return new PaymentResultDto(
            IsSuccessful: isSuccessful,
            ConversationId: result.ConversationId,
            ProviderPaymentId: result.PaymentId,
            PaidPrice: decimal.Parse(result.PaidPrice, System.Globalization.CultureInfo.InvariantCulture),
            MaskedCardNumber: result.LastFourDigits,
            FailureReason: isSuccessful ? null : result.ErrorMessage);
    }
}