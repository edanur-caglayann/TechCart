/*
  Backend'e giden TÜM isteklerin geçtiği ortak nokta.
  Base URL ekleme, Authorization header'ı ekleme ve
  backend'in iki farklı hata formatını
  ({code,message} ve {errors:[{field,message}]})
  tek bir Error tipine çevirme işini burada yapıyoruz.
  Böylece authService, cartService vb. her dosya bunu
  tekrar tekrar yazmak zorunda kalmıyor.
*/

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

const TOKEN_STORAGE_KEY = "techcart-token";

/* Token'ı localStorage'dan okur. */
export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
}

export function clearToken(): void {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
}

/* Backend'in validasyon hatasında döndüğü tek bir alan hatası. */
export type FieldError = { field: string; message: string };

/*
  Backend'den dönen hem AppException ({code,message}) hem de
  validasyon ({errors:[...]}) formatlarını taşıyan ortak hata tipi.
  Çağıran kod (örn. register formu) error.fieldErrors varsa
  alan bazlı, yoksa error.code'a göre genel bir mesaj gösterebilir.
*/
export class ApiError extends Error {
  status: number;
  code?: string;
  fieldErrors?: FieldError[];

  constructor(
    status: number,
    message: string,
    code?: string,
    fieldErrors?: FieldError[]
  ) {
    super(message);
    this.status = status;
    this.code = code;
    this.fieldErrors = fieldErrors;
  }
}

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  /* true ise Authorization header'ı otomatik eklenir. */
  requiresAuth?: boolean;
};

export async function apiFetch<TResponse>(
  path: string,
  options: RequestOptions = {}
): Promise<TResponse> {
  const { method = "GET", body, requiresAuth = false } = options;

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  };

  if (requiresAuth) {
    const token = getToken();
    if (token) {
      headers["Authorization"] = `Bearer ${token}`;
    }
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  /* 204 No Content: gövde yok, parse etmeye çalışma (logout, change-password burada). */
  if (response.status === 204) {
    return undefined as TResponse;
  }

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    /* Validasyon hatası: { errors: [{ field, message }] } */
    if (data?.errors) {
      throw new ApiError(
        response.status,
        "Doğrulama hatası",
        undefined,
        data.errors
      );
    }

    /* AppException hatası: { code, message } (örn. EMAIL_ALREADY_EXISTS, INVALID_CREDENTIALS) */
    throw new ApiError(
      response.status,
      data?.message ?? "Bir hata oluştu",
      data?.code
    );
  }

  return data as TResponse;
}