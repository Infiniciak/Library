@echo off
SETLOCAL

:: Konfiguracja parametrów
SET PG_HOST=localhost
SET PG_PORT=5432
SET PG_USER=postgres
SET PG_PASSWORD=123  :: Hasło administratora PostgreSQL
SET NEW_DB_NAME=Library
SET NEW_DB_USER=library_user
SET NEW_DB_PASSWORD=123

:: Ścieżka do narzędzi PostgreSQL (dostosuj do swojej instalacji)
SET PG_BIN_PATH="C:\Program Files\PostgreSQL\14\bin"

:: 1. Tworzenie nowej bazy danych
echo Tworzenie bazy danych %NEW_DB_NAME%...
%PG_BIN_PATH%\psql.exe -h %PG_HOST% -p %PG_PORT% -U %PG_USER% -c "CREATE DATABASE "%NEW_DB_NAME%";"

:: 2. Tworzenie nowego użytkownika z hasłem
echo Tworzenie użytkownika %NEW_DB_USER%...
%PG_BIN_PATH%\psql.exe -h %PG_HOST% -p %PG_PORT% -U %PG_USER% -c "CREATE USER %NEW_DB_USER% WITH PASSWORD '%NEW_DB_PASSWORD%';"

:: 3. Nadawanie uprawnień użytkownikowi
echo Nadawanie uprawnień...
%PG_BIN_PATH%\psql.exe -h %PG_HOST% -p %PG_PORT% -U %PG_USER% -c "GRANT ALL PRIVILEGES ON DATABASE "%NEW_DB_NAME%" TO %NEW_DB_USER%;"

:: 4. Tworzenie tabel w nowej bazie danych
echo Tworzenie tabel w bazie %NEW_DB_NAME%...
%PG_BIN_PATH%\psql.exe -h %PG_HOST% -p %PG_PORT% -U %PG_USER% -d %NEW_DB_NAME% -c "

-- Tabela Kategorie
CREATE TABLE categories (
  \"Id\" SERIAL PRIMARY KEY,
  \"Name\" VARCHAR(50) NOT NULL UNIQUE
);

-- Tabela Ksiazki
CREATE TABLE books (
  \"Id\" SERIAL PRIMARY KEY,
  \"Title\" VARCHAR(100) NOT NULL,
  \"Author\" VARCHAR(100) NOT NULL,
  \"ISBN\" VARCHAR(20) NOT NULL,
  \"ReleaseYear\" INT NOT NULL
);

-- Tabela łącząca Ksiazki_Kategorie (relacja wiele-do-wielu)
CREATE TABLE books_categories (
  \"BookId\" INT NOT NULL,
  \"CategoryId\" INT NOT NULL,
  PRIMARY KEY (\"BookId\", \"CategoryId\"),
  FOREIGN KEY (\"BookId\") REFERENCES books(\"Id\"),
  FOREIGN KEY (\"CategoryId\") REFERENCES categories(\"Id\")
);

-- Tabela Czlonkowie
CREATE TABLE members (
  \"Id\" SERIAL PRIMARY KEY,
  \"Name\" VARCHAR(50) NOT NULL,
  \"Surname\" VARCHAR(50) NOT NULL,
  \"CardNumber\" VARCHAR(20) NOT NULL UNIQUE,
  \"Email\" VARCHAR(100) NOT NULL UNIQUE
);

-- Tabela Wypozyczenia
CREATE TABLE borrows (
  \"Id\" SERIAL PRIMARY KEY,
  \"BookId\" INT NOT NULL,
  \"MemberId\" INT NOT NULL,
  \"BorrowDate\" TIMESTAMP NOT NULL,
  \"ReturnDate\" TIMESTAMP NULL,
  FOREIGN KEY (\"BookId\") REFERENCES books(\"Id\"),
  FOREIGN KEY (\"MemberId\") REFERENCES members(\"Id\")
);
"

echo Operacja zakończona pomyślnie!
pause