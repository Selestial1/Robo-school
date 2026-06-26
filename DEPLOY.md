# ROBO.SCHOOL — вывод сайта в интернет 24/7

Эта инструкция настроит:

1. **Neon** — облачную базу PostgreSQL с веб-интерфейсом (просмотр таблиц в браузере)
2. **Render** — бесплатный хостинг сайта (работает без включённого компьютера)
3. **admin.html** — панель просмотра БД на вашем сайте
4. **status.html** — страница проверки работоспособности

---

## Шаг 1. База данных на Neon (просмотр БД в браузере)

1. Откройте [https://neon.tech](https://neon.tech) и зарегистрируйтесь (можно через Google/GitHub).
2. Нажмите **New Project** → имя, например `robo-school` → **Create project**.
3. На главной странице проекта скопируйте **Connection string** (формат `postgresql://...`).
4. **Важно:** выберите **Direct connection** (не Pooled). В hostname не должно быть `-pooler`.
   - ✅ `ep-xxxxx.us-east-2.aws.neon.tech`
   - ❌ `ep-xxxxx-pooler.us-east-2.aws.neon.tech`
5. Сохраните строку — она понадобится на Render как `DATABASE_URL`.

**Как смотреть БД в Neon:**

- В левом меню: **Tables** — все таблицы (`applications`, `packages`, `trainers`)
- **SQL Editor** — выполнять запросы, например:
  ```sql
  SELECT * FROM applications ORDER BY "CreatedAt" DESC;
  ```

---

## Шаг 2. Загрузка проекта на GitHub

На компьютере должен быть установлен [Git](https://git-scm.com/download/win).

```powershell
cd "c:\Users\Administrator\Desktop\142536\Новая папка"
git init
git add .
git commit -m "ROBO.SCHOOL — сайт с API и панелью БД"
```

1. Создайте репозиторий на [https://github.com/new](https://github.com/new) (имя: `robo-school`).
2. Выполните команды, которые покажет GitHub:

```powershell
git remote add origin https://github.com/ВАШ_ЛОГИН/robo-school.git
git branch -M main
git push -u origin main
```

**Без GitHub:** на Render можно задеплоить через **Public Git repository** или загрузить через Docker Hub (сложнее).

---

## Шаг 3. Хостинг на Render (сайт 24/7)

1. Откройте [https://render.com](https://render.com) и зарегистрируйтесь.
2. **New +** → **Blueprint** (или **Web Service**).
3. Подключите GitHub-репозиторий `robo-school`.
4. Render найдёт файл `render.yaml` и создаст сервис автоматически.
5. В настройках сервиса добавьте переменные окружения:

| Переменная | Значение |
|------------|----------|
| `DATABASE_URL` | Строка подключения из Neon |
| `AdminKey` | Ваш секретный ключ (например `robo-admin-2026`) |

6. Нажмите **Deploy**. Через 5–10 минут сайт будет доступен по адресу вида:
   `https://robo-school-xxxx.onrender.com`

**Важно:** на бесплатном тарифе Render сайт «засыпает» после ~15 мин без посетителей. Первый заход после сна может занять 30–60 секунд. Для постоянной работы без задержек нужен платный тариф или VPS.

---

## Шаг 4. Проверка после деплоя

| Страница | URL |
|----------|-----|
| Сайт | `https://ВАШ-САЙТ.onrender.com/` |
| Панель БД | `https://ВАШ-САЙТ.onrender.com/admin.html` |
| Статус | `https://ВАШ-САЙТ.onrender.com/status.html` |
| API health | `https://ВАШ-САЙТ.onrender.com/api/health` |
| Neon (БД) | [https://console.neon.tech](https://console.neon.tech) |

На **admin.html** введите ключ `AdminKey` — увидите заявки, пакеты, тренеров и статус БД.

На **status.html** — зелёная галочка, если сайт и база работают.

---

## Шаг 5. Почта (необязательно)

В Render → **Environment** добавьте:

| Переменная | Пример |
|------------|--------|
| `Email__Enabled` | `true` |
| `Email__Host` | `smtp.yandex.ru` |
| `Email__Port` | `587` |
| `Email__Username` | `ваш@yandex.ru` |
| `Email__Password` | пароль приложения |
| `Email__FromEmail` | `ваш@yandex.ru` |
| `Email__FromName` | `ROBO.SCHOOL` |
| `Email__UseSsl` | `true` |

---

## Локальный запуск (как раньше)

```powershell
start.bat
```

- Сайт: [http://localhost:5080](http://localhost:5080)
- Панель БД: [http://localhost:5080/admin.html](http://localhost:5080/admin.html)
- Статус: [http://localhost:5080/status.html](http://localhost:5080/status.html)

Локально используется SQLite (`data/robo.db`). В облаке — PostgreSQL на Neon.

---

## Быстрая шпаргалка

```
Neon     → база данных + просмотр таблиц в браузере
Render   → хостинг сайта 24/7 (бесплатно, с «сном»)
admin.html  → заявки и таблицы на вашем сайте
status.html → проверка «работает / не работает»
```

---

## Ошибка `relation "packages" does not exist`

**Причина:** на Render запущена старая версия кода, таблицы в Neon не созданы.

### Быстрое исправление (3 действия)

**1. Загрузите исправление на GitHub**

Дважды щёлкните файл **`upload-fix.bat`** в папке проекта  
(или в PowerShell выполните команды из него).

**2. На Render перезапустите деплой**

1. [dashboard.render.com](https://dashboard.render.com) → ваш сервис
2. **Manual Deploy** → **Deploy latest commit**
3. Подождите 5–10 минут

**3. Проверьте DATABASE_URL**

На Render → **Environment** → `DATABASE_URL` должен быть **Direct connection** из Neon (без `-pooler` в адресе).

### Альтернатива: создать таблицы вручную в Neon

1. [console.neon.tech](https://console.neon.tech) → ваш проект → **SQL Editor**
2. Вставьте и нажмите **Run**:

```sql
CREATE TABLE IF NOT EXISTS packages (
    "Id" SERIAL PRIMARY KEY,
    "Code" VARCHAR(20) NOT NULL,
    "Name" VARCHAR(50) NOT NULL,
    "Price" INTEGER NOT NULL,
    "Description" VARCHAR(500) NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_packages_Code" ON packages ("Code");

CREATE TABLE IF NOT EXISTS trainers (
    "Id" SERIAL PRIMARY KEY,
    "Slug" VARCHAR(40) NOT NULL,
    "Name" VARCHAR(120) NOT NULL,
    "Role" VARCHAR(120) NOT NULL,
    "PhotoUrl" VARCHAR(500) NOT NULL,
    "Bio" VARCHAR(2000) NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_trainers_Slug" ON trainers ("Slug");

CREATE TABLE IF NOT EXISTS applications (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(120) NOT NULL,
    "Phone" VARCHAR(30) NOT NULL,
    "Email" VARCHAR(120) NOT NULL,
    "PackageCode" VARCHAR(20),
    "CreatedAt" TIMESTAMP NOT NULL
);
CREATE INDEX IF NOT EXISTS "IX_applications_CreatedAt" ON applications ("CreatedAt");
```

3. На Render: **Manual Deploy** → **Deploy latest commit**

Если нужна помощь с конкретным шагом (Neon, GitHub или Render) — напишите, на каком этапе вы сейчас.
