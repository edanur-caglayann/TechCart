namespace TechCart.SharedKernel;

// Sayfalanmış herhangi bir listenin ortak zarfı
// Product, order vs gibi sayfalama durumlarinda ortak olarak kullancagimiz icin
// SharedKernel'de, tüm modüllerin erişebileceği yerde
public record PagedResult<T>(List<T> Items, int TotalCount, int PageNumber, int PageSize);