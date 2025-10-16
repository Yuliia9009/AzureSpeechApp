
# 🎙 AI Расшифровка и Перевод Речи (ASP.NET Core + Azure)

## 📦 Описание
Веб-приложение для:
- Распознавания речи с микрофона или аудиофайла
- Перевода текста на выбранный язык
- Скачивания результата в формате .txt

## 🛠️ Используемые технологии
- ASP.NET Core Web API (.NET 8)
- Azure Cognitive Services: Speech to Text, Translator
- HTML + JS (с Recorder.js и Bootstrap)
- GStreamer (для аудиоформатов)

---

## ⚙️ Установка

### 1. Клонируй проект и установи зависимости

```bash
dotnet restore
```

### 2. Укажи ключи Azure в `appsettings.json`:

```json
"Azure": {
  "SpeechKey": "...",
  "SpeechRegion": "...",
  "TranslatorKey": "...",
  "TranslatorEndpoint": "...",
  "TranslatorRegion": "..."
}
```

---

## 🍏 Важно для macOS

Если ты используешь **Mac**, то для работы с MP3 / WAV / сжатыми аудиоформатами через Azure SDK необходимо установить **GStreamer**:

```bash
brew install gstreamer
brew install gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly
```

⚠ Без него ты получишь ошибку:
```
SPXERR_GSTREAMER_NOT_FOUND_ERROR
```

---

## 🚀 Запуск

```bash
dotnet run
```

Открой браузер:
```
http://localhost:5000/index.html
```

---

## ✅ Поддерживаемые форматы

- 🎙 Микрофон (WAV через Recorder.js)
- 📁 Файлы: WAV, MP3 (предпочтительно)

---

## 📂 Структура

- `Controllers/SpeechController.cs` — API-методы
- `Services/SpeechService.cs` — Распознавание речи
- `Services/TranslationService.cs` — Перевод текста
- `wwwroot/index.html` — Фронтенд
- `wwwroot/script.js` — Логика кнопок, запись и отправка

---

## 📄 Лицензия

MIT
