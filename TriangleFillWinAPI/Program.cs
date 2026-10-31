using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern SafeFileHandle GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool WriteConsoleOutputW(
        SafeFileHandle hConsoleOutput,
        CharInfo[] lpBuffer,
        Coord dwBufferSize,
        Coord dwBufferCoord,
        ref SmallRect lpWriteRegion);

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;
        public Coord(short x, short y) { X = x; Y = y; }
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(2)] public ushort Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    const int STD_OUTPUT_HANDLE = -11;

    static void Main()
    {
        Console.Clear();
        Console.Title = "Вертикальная заливка треугольника снизу вверх";

        SafeFileHandle hOutput = GetStdHandle(STD_OUTPUT_HANDLE);

        const short width = 20;
        const short height = 10;

        CharInfo[] buffer = new CharInfo[width * height];

        // Цвета (формат WinAPI: старший байт — фон, младший — текст)
        ushort bgBlue = 0x0010; // синий фон
        ushort fillColor = 0x000F; // ярко-белый текст
        ushort emptyColor = 0x0008; // тёмно-серый на чёрном

        // 1) Изначально заполняем всё точками (контур будущей фигуры)
        for (short y = 0; y < height; y++)
        {
            for (short x = 0; x < width; x++)
            {
                int index = y * width + x;
                buffer[index].UnicodeChar = '.';
                buffer[index].Attributes = emptyColor;
            }
        }

        // Область вывода в консоли
        SmallRect screenRect = new SmallRect
        {
            Left = 5,
            Top = 2,
            Right = (short)(5 + width - 1),
            Bottom = (short)(2 + height - 1)
        };

        // Первичный "пустой" кадр, чтобы пользователь увидел стартовое состояние
        WriteConsoleOutputW(hOutput, buffer,
            new Coord(width, height), new Coord(0, 0), ref screenRect);
        Thread.Sleep(300);

        // 2) Анимация: закрашиваем строки снизу вверх
        for (short y = (short)(height - 1); y >= 0; y--)
        {
            // Ширина полосы в текущей строке: 2*y + 2 (y=9 → 20, y=0 → 2)
            short rowWidth = (short)(2 + 2 * y);
            short leftOffset = (short)((width - rowWidth) / 2);

            for (short x = leftOffset; x < leftOffset + rowWidth; x++)
            {
                int index = y * width + x;
                buffer[index].UnicodeChar = '█';
                buffer[index].Attributes = (ushort)(bgBlue | fillColor);
            }

            // Обновляем кадр целиком, без Console.Clear
            WriteConsoleOutputW(hOutput, buffer,
                new Coord(width, height), new Coord(0, 0), ref screenRect);

            Thread.Sleep(150); // задержка между строками
        }

        Console.SetCursorPosition(0, 14);
        Console.WriteLine("Треугольник залит снизу вверх. Нажмите любую клавишу...");
        Console.ReadKey();
    }
}