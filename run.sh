#!/bin/bash

# ✅ Установка переменных среды для GStreamer
export DYLD_LIBRARY_PATH=/opt/homebrew/lib
export GST_PLUGIN_PATH=/opt/homebrew/lib/gstreamer-1.0

# ✅ Запуск ASP.NET Core приложения
dotnet run
