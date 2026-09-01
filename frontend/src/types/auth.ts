/* Kullanıcının sistemde sahip olabileceği roller. */
export type UserRole = "Customer" | "Admin";

/*
  Oturum açan kullanıcı icin frontend tarafında tutulacak temel bilgiler.
  Parola bu nesne içerisinde tutulmaz.
*/
export type AuthUser = {
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
};

//Kayıt işleminin AuthContext'e göndereceği bilgiler.
export type RegisterCredentials = {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
};

/* Giriş için gereken bilgiler. */
export type LoginCredentials = {
  email: string;
  password: string;
};

// Profilde ad, soyad ve e-posta güncellenirken AuthContext'e gönderilecek bilgiler.
export type UpdateProfileData = {
  firstName: string;
  lastName: string;
  email: string;
};