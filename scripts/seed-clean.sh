#!/usr/bin/env bash
set -eu

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

csv_path="$repo_root/backend/src/Hosts/TechCart.Seeder/data/electronics_product.csv"

if [[ ! -f "$csv_path" ]]; then
  printf '%s\n' 'Ornek urun CSV dosyasi bulunamadi.' >&2
  exit 1
fi

export ConnectionStrings__PostgreSQL="Host=localhost;Port=5434;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD"

printf 'Hedef: localhost:5434 / %s\n' "$POSTGRES_DB"

docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean up -d --wait --wait-timeout 60 postgres

seeder_project='backend/src/Hosts/TechCart.Seeder/TechCart.Seeder.csproj'
dotnet build "$seeder_project"

dotnet run --project "$seeder_project" --no-build --no-launch-profile -- "$csv_path"
dotnet run --project "$seeder_project" --no-build --no-launch-profile -- backfill-stock

printf '%s\n' 'Ornek veri yuklemesi tamamlandi.'