import { apiFetch } from "./apiClient";
import type { AuthUser } from "../types/auth";

/* Backend'in login/register endpoint'lerinin döndüğü ortak şekil. */
type AuthResponse = {
  token: string;
  user: AuthUser;
};

/* GET /api/auth/session'ın döndüğü şekil. */
type SessionResponse = {
  isAuthenticated: boolean;
  user: AuthUser;
};

export function registerRequest(payload: {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}) {
  return apiFetch<AuthResponse>("/api/auth/register", {
    method: "POST",
    /*
      passwordConfirm'i burada, frontend'in kendi bildiği password
      değeriyle dolduruyoruz — Zod zaten form seviyesinde eşleştiğini
      doğruladı, bu alanı ayrıca kullanıcıdan/component'ten almaya
      gerek yok. Backend'in [Compare] kontrolü asıl işlevini, API'ye
      doğrudan (frontend'i atlayarak) istek atan biri olursa görür.
    */
    body: { ...payload, passwordConfirm: payload.password },
  });
}

export function loginRequest(payload: {
  email: string;
  password: string;
}) {
  return apiFetch<AuthResponse>("/api/auth/login", {
    method: "POST",
    body: payload,
  });
}

export function sessionRequest() {
  return apiFetch<SessionResponse>("/api/auth/session", {
    method: "GET",
    requiresAuth: true,
  });
}

export function logoutRequest() {
  return apiFetch<void>("/api/auth/logout", {
    method: "POST",
    requiresAuth: true,
  });
}

export function updateProfileRequest(payload: {
  firstName: string;
  lastName: string;
  email: string;
}) {
  return apiFetch<AuthUser>("/api/users/me", {
    method: "PUT",
    body: payload,
    requiresAuth: true,
  });
}

/*
  AuthContext'e bilerek eklemiyoruz — şifre değişimi context'teki
  "user" state'ini etkilemiyor (backend 204 dönüyor, güncel kullanıcı
  bilgisi dönmüyor). Profil/şifre formu bunu doğrudan buradan çağırabilir.
*/
export function changePasswordRequest(payload: {
  currentPassword: string;
  newPassword: string;
  newPasswordConfirm: string;
}) {
  return apiFetch<void>("/api/users/me/password", {
    method: "PUT",
    body: payload,
    requiresAuth: true,
  });
}