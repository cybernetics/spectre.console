using System;
using Shouldly;
using Xunit;

namespace Spectre.Console.Tests.Unit
{
    public sealed class GridTests
    {
        public sealed class TheAddColumnMethod
        {
            [Fact]
            public void Should_Throw_If_Rows_Are_Not_Empty()
            {
                // Given
                var grid = new Grid();
                grid.AddColumn();
                grid.AddRow("Hello World!");

                // When
                var result = Record.Exception(() => grid.AddColumn());

                // Then
                result.ShouldBeOfType<InvalidOperationException>()
                    .Message.ShouldBe("Cannot add new columns to grid with existing rows.");
            }
        }

        public sealed class TheAddRowMethod
        {
            [Fact]
            public void Should_Throw_If_Rows_Are_Null()
            {
                // Given
                var grid = new Grid();

                // When
                var result = Record.Exception(() => grid.AddRow(null));

                // Then
                result.ShouldBeOfType<ArgumentNullException>()
                    .ParamName.ShouldBe("columns");
            }

            [Fact]
            public void Should_Add_Empty_Items_If_User_Provides_Less_Row_Items_Than_Columns()
            {
                // Given
                var grid = new Grid();
                grid.AddColumn();
                grid.AddColumn();

                // When
                grid.AddRow("Foo");

                // Then
                grid.RowCount.ShouldBe(1);
            }

            [Fact]
            public void Should_Throw_If_Row_Columns_Are_Greater_Than_Number_Of_Columns()
            {
                // Given
                var grid = new Grid();
                grid.AddColumn();

                // When
                var result = Record.Exception(() => grid.AddRow("Foo", "Bar"));

                // Then
                result.ShouldBeOfType<InvalidOperationException>();
                result.Message.ShouldBe("The number of row columns are greater than the number of grid columns.");
            }
        }

        public sealed class TheAddEmptyRowMethod
        {
            [Fact]
            public void Should_Add_Empty_Row()
            {
                // Given
                var console = new PlainConsole(width: 80);
                var grid = new Grid();
                grid.AddColumns(2);
                grid.AddRow("Foo", "Bar");
                grid.AddEmptyRow();
                grid.AddRow("Qux", "Corgi");

                // When
                console.Render(grid);

                // Then
                console.Lines.Count.ShouldBe(3);
                console.Lines[0].ShouldBe("Foo  Bar  ");
                console.Lines[1].ShouldBe("          ");
                console.Lines[2].ShouldBe("Qux  Corgi");
            }

            [Fact]
            public void Should_Add_Empty_Row_At_The_End()
            {
                // Given
                var console = new PlainConsole(width: 80);
                var grid = new Grid();
                grid.AddColumns(2);
                grid.AddRow("Foo", "Bar");
                grid.AddEmptyRow();
                grid.AddRow("Qux", "Corgi");
                grid.AddEmptyRow();

                // When
                console.Render(grid);

                // Then
                console.Lines.Count.ShouldBe(4);
                console.Lines[0].ShouldBe("Foo  Bar  ");
                console.Lines[1].ShouldBe("          ");
                console.Lines[2].ShouldBe("Qux  Corgi");
                console.Lines[3].ShouldBe("          ");
            }
        }

        [Fact]
        public void Should_Render_Grid_Correctly()
        {
            // Given
            var console = new PlainConsole(width: 80);
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("Qux", "Corgi", "Waldo");
            grid.AddRow("Grault", "Garply", "Fred");

            // When
            console.Render(grid);

            // Then
            console.Lines.Count.ShouldBe(2);
            console.Lines[0].ShouldBe("Qux     Corgi   Waldo");
            console.Lines[1].ShouldBe("Grault  Garply  Fred ");
        }

        [Fact]
        public void Should_Render_Grid_Column_Alignment_Correctly()
        {
            // Given
            var console = new PlainConsole(width: 80);
            var grid = new Grid();
            grid.AddColumn(new GridColumn { Alignment = Justify.Right });
            grid.AddColumn(new GridColumn { Alignment = Justify.Center });
            grid.AddColumn(new GridColumn { Alignment = Justify.Left });
            grid.AddRow("Foo", "Bar", "Baz");
            grid.AddRow("Qux", "Corgi", "Waldo");
            grid.AddRow("Grault", "Garply", "Fred");

            // When
            console.Render(grid);

            // Then
            console.Lines.Count.ShouldBe(3);
            console.Lines[0].ShouldBe("   Foo   Bar    Baz  ");
            console.Lines[1].ShouldBe("   Qux  Corgi   Waldo");
            console.Lines[2].ShouldBe("Grault  Garply  Fred ");
        }

        [Fact]
        public void Should_Use_Default_Padding()
        {
            // Given
            var console = new PlainConsole(width: 80);
            var grid = new Grid();
            grid.AddColumns(3);
            grid.AddRow("Foo", "Bar", "Baz");
            grid.AddRow("Qux", "Corgi", "Waldo");
            grid.AddRow("Grault", "Garply", "Fred");

            // When
            console.Render(grid);

            // Then
            console.Lines.Count.ShouldBe(3);
            console.Lines[0].ShouldBe("Foo     Bar     Baz  ");
            console.Lines[1].ShouldBe("Qux     Corgi   Waldo");
            console.Lines[2].ShouldBe("Grault  Garply  Fred ");
        }

        [Fact]
        public void Should_Render_Explicit_Grid_Column_Padding_Correctly()
        {
            // Given
            var console = new PlainConsole(width: 80);
            var grid = new Grid();
            grid.AddColumn(new GridColumn { Padding = new Padding(3, 0) });
            grid.AddColumn(new GridColumn { Padding = new Padding(0, 0) });
            grid.AddColumn(new GridColumn { Padding = new Padding(0, 3) });
            grid.AddRow("Foo", "Bar", "Baz");
            grid.AddRow("Qux", "Corgi", "Waldo");
            grid.AddRow("Grault", "Garply", "Fred");

            // When
            console.Render(grid);

            // Then
            console.Lines.Count.ShouldBe(3);
            console.Lines[0].ShouldBe("   Foo   Bar   Baz     ");
            console.Lines[1].ShouldBe("   Qux   Corgi Waldo   ");
            console.Lines[2].ShouldBe("   GraultGarplyFred    ");
        }

        [Fact]
        public void Should_Render_Grid()
        {
            var console = new PlainConsole(width: 80);
            var grid = new Grid();
            grid.AddColumn(new GridColumn { NoWrap = true });
            grid.AddColumn(new GridColumn { Padding = new Padding(2, 0) });
            grid.AddRow("[bold]Options[/]", string.Empty);
            grid.AddRow("  [blue]-h[/], [blue]--help[/]", "Show command line help.");
            grid.AddRow("  [blue]-c[/], [blue]--configuration[/]", "The configuration to run for.\nThe default for most projects is [green]Debug[/].");

            // When
            console.Render(grid);

            // Then
            console.Lines.Count.ShouldBe(4);
            console.Lines[0].ShouldBe("Options                                                         ");
            console.Lines[1].ShouldBe("  -h, --help             Show command line help.                ");
            console.Lines[2].ShouldBe("  -c, --configuration    The configuration to run for.          ");
            console.Lines[3].ShouldBe("                         The default for most projects is Debug.");
        }
    }
}
