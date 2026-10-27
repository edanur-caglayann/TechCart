#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

if [[ ! -f .env.clean ]]; then
  printf '%s\n' '.env.clean bulunamadi.' >&2
  exit 1
fi

source ./.env.clean

: "${POSTGRES_DB:?POSTGRES_DB eksik}"
: "${POSTGRES_USER:?POSTGRES_USER eksik}"
: "${POSTGRES_PASSWORD:?POSTGRES_PASSWORD eksik}"

if [[ "$POSTGRES_DB" != "techcart_clean" || "$POSTGRES_USER" != "techcart_clean" ]]; then
  printf '%s\n' 'Bu komut techcart_clean veritabani ve kullanicisi icindir.' >&2
  exit 1
fi

export ConnectionStrings__PostgreSQL="Host=localhost;Port=5434;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD"
export Elasticsearch__Url='http://localhost:9201'

docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean up -d --wait --wait-timeout 120 postgres elasticsearch

cluster_name="$(
  curl --fail --silent --show-error "$Elasticsearch__Url/" |
    python3 -c 'import json, sys; print(json.load(sys.stdin)["cluster_name"])'
)"

if [[ "$cluster_name" != "techcart-clean" ]]; then
  printf '%s\n' 'Beklenen Elasticsearch kumesi techcart-clean. Islem durduruldu.' >&2
  exit 1
fi

printf 'Kaynak: localhost:5434 / %s\n' "$POSTGRES_DB"
printf 'Hedef: %s / products\n' "$Elasticsearch__Url"

seeder_project='backend/src/Hosts/TechCart.Seeder/TechCart.Seeder.csproj'
dotnet build "$seeder_project"

dotnet run --project "$seeder_project" --no-build --no-launch-profile -- reindex-search

curl --fail --silent --show-error -X POST "$Elasticsearch__Url/products/_refresh" > /dev/null

printf '%s\n' 'Temiz arama indeksi yuklemesi tamamlandi.'