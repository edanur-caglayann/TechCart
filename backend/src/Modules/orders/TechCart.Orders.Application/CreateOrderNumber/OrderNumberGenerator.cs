namespace TechCart.Orders.Application.CreateOrderNumber;

// Her siparis icin kullaniciya okunabilir rastgele bir siparis numarasi uretir.
public static class OrderNumberGenerator
{
    // kullanilabilecek karakterler 
    private const string Alphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
    
    public static string Generate()
    {
        // 8 karakterlik bir alan olusturur
        var random = Random.Shared;
        var code = new char[8];

        // Alphabet listesinden rastgele karakterler secilir
        for (var i = 0; i < code.Length; i++)
            code[i] = Alphabet[random.Next(Alphabet.Length)];

        return $"ORD-{new string(code)}";
    }
}