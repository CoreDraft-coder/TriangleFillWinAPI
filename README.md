# TriangleFillWinAPI

Вертикальная заливка треугольника (пирамиды) снизу вверх через WinAPI `WriteConsoleOutputW`.

## Демонстрация
- Буфер `CharInfo[20 × 10]`
- Анимация: строки заполняются от `y = 9` до `y = 0`
- Ширина строки `y`: `2 + 2*y` (пирамида)
- Никакого `Console.Clear()` — только `WriteConsoleOutputW`

## Требования
- Windows
- .NET Framework 4.7.2+
- Visual Studio 2022 (или .NET SDK 8+ для сборки через `dotnet build`)

## Запуск