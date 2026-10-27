import { apiFetch } from "./apiClient";

export type SearchSuggestion = { id: string; name: string };
type SuggestionsResponse = { suggestions: SearchSuggestion[] };

export function getSearchSuggestionsRequest(term: string): Promise<SearchSuggestion[]> {
  return apiFetch<SuggestionsResponse>(
    `/api/search/suggestions?q=${encodeURIComponent(term)}`,
    { method: "GET" }
  ).then((response) => response.suggestions);
}