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

export ConnectionStrings__PostgreSQL="Host=localhost;Port=5434;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD"

printf 'Hedef: localhost:5434 / %s\n' "$POSTGRES_DB"

docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean up -d --wait --wait-timeout 60 postgres

dotnet tool restore

api_project='backend/src/Hosts/TechCart.Api/TechCart.Api.csproj'
dotnet build "$api_project"

migrate_module() {
  local module_dir="$1"
  local module_name="$2"
  local project_name="TechCart.${module_name}.Infrastructure"
  local context_name="${module_name}DbContext"

  printf '%s migrationlari uygulanıyor...\n' "$context_name"

  dotnet ef database update \
    --project "backend/src/Modules/${module_dir}/${project_name}/${project_name}.csproj" \
    --startup-project "$api_project" \
    --context "$context_name" \
    --no-build \
    -- --environment Development
}

migrate_module users Users
migrate_module addresses Addresses
migrate_module categories Categories
migrate_module brands Brands
migrate_module products Products
migrate_module product_images ProductImages
migrate_module inventory Inventory
migrate_module cart_items CartItems
migrate_module orders Orders
migrate_module order_items OrderItems
migrate_module payments Payments

printf '%s\n' 'Tum migrationlar tamamlandi.'