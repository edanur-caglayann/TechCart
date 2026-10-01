"use client";

import { Search } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";

import { getSearchSuggestionsRequest, type SearchSuggestion } from "../../services/searchService";
import styles from "./SearchBar.module.css";

export default function SearchBar() {
  const [query, setQuery] = useState("");
  const [suggestions, setSuggestions] = useState<SearchSuggestion[]>([]);
  const [isOpen, setIsOpen] = useState(false);

  const containerRef = useRef<HTMLDivElement | null>(null);
  const router = useRouter();

  useEffect(() => {
    const trimmed = query.trim();

    if (trimmed.length < 2) {
      setSuggestions([]);
      return;
    }

    const timeoutId = setTimeout(() => {
      getSearchSuggestionsRequest(trimmed)
        .then((result) => {
          setSuggestions(result);
          setIsOpen(true);
        })
        .catch(() => setSuggestions([]));
    }, 250);

    return () => clearTimeout(timeoutId);
  }, [query]);

  useEffect(() => {
    function closeOnOutsideClick(event: MouseEvent) {
      const clickedElement = event.target as Node;
      if (containerRef.current && !containerRef.current.contains(clickedElement)) {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", closeOnOutsideClick);
    return () => document.removeEventListener("mousedown", closeOnOutsideClick);
  }, []);

  // Enter'a basmak ya da büyüteçe tıklamak: KATALOĞU filtreler (yazılan metin
  // tam bir ürün adı olmayabilir — "bilgisayar" gibi genel bir terim de olabilir).
  function searchCatalog() {
    const trimmed = query.trim();
    if (trimmed === "") return;

    setIsOpen(false);
    router.push(`/?q=${encodeURIComponent(trimmed)}#products`);
    document.getElementById("products")?.scrollIntoView({ behavior: "smooth" });
  }

  // Bir öneriye tıklamak: o BELİRLİ ürünü doğrudan açar.
  function openProduct(productId: string) {
    setIsOpen(false);
    router.push(`/products/${productId}`);
  }

  function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    searchCatalog();
  }

  return (
    <div className={styles.searchBar} ref={containerRef}>
      <form onSubmit={handleSubmit} className={styles.form}>
        <input
          className={styles.input}
          type="text"
          placeholder="Ürün, marka veya kategori ara..."
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          onFocus={() => {
            if (suggestions.length > 0) setIsOpen(true);
          }}
          onKeyDown={(event) => {
            if (event.key === "Escape") setIsOpen(false);
          }}
        />

        <button className={styles.searchButton} type="submit" aria-label="Ürün ara">
          <Search size={21} strokeWidth={2} />
        </button>
      </form>

      {isOpen && suggestions.length > 0 && (
        <ul className={styles.suggestionList} role="listbox">
          {suggestions.map((suggestion) => (
            <li key={suggestion.id}>
              <button
                type="button"
                className={styles.suggestionItem}
                onClick={() => openProduct(suggestion.id)}
                title={suggestion.name}
              >
                {suggestion.name}
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}