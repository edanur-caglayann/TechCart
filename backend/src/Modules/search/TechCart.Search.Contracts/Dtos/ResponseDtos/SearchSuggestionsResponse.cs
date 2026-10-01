namespace TechCart.Search.Contracts.Dtos.ResponseDtos;
public record SearchSuggestionItemResponse(Guid Id, string Name);
public record SearchSuggestionsResponse(List<SearchSuggestionItemResponse> Suggestions);