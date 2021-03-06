using System;
using System.IO;
using System.Text;
using Spectre.Console.Tests.Tools;

namespace Spectre.Console.Tests
{
    public sealed class TestableAnsiConsole : IDisposable, IAnsiConsole
    {
        private readonly StringWriter _writer;
        private readonly IAnsiConsole _console;

        public string Output => _writer.ToString();

        public Capabilities Capabilities => _console.Capabilities;
        public Encoding Encoding => _console.Encoding;
        public int Width { get; }
        public int Height => _console.Height;

        public TestableAnsiConsole(ColorSystem system, AnsiSupport ansi = AnsiSupport.Yes, int width = 80)
        {
            _writer = new StringWriter();
            _console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = ansi,
                ColorSystem = (ColorSystemSupport)system,
                Out = _writer,
                LinkIdentityGenerator = new TestLinkIdentityGenerator(),
            });

            Width = width;
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }

        public void Write(string text, Style style)
        {
            _console.Write(text, style);
        }
    }
}
