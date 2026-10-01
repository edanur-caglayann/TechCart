namespace TechCart.Search.Contracts.Dtos.ResponseDtos;

// sadece urun adi. kullanici yazarken liste gostermek icin.
// Bu aamada resim,fiyar bilgileri gerksiz yuk olur
public record SearchSuggestionsResponse(List<string> Suggestions);