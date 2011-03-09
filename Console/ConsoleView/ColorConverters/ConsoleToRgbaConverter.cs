using System;
using NrknLib.Color;
using NrknLib.Color.Extensions;

namespace NrknLib.ConsoleView.ColorConverters {
  public class ConsoleToRgbaConverter {
    static ConsoleToRgbaConverter() {
      FormatColor =
        color => ( (string) ConsoleToHtmlColorConverter.FormatColor( color ) ).ToRgba();

    }
    public static Func<ConsoleColor, Rgba> FormatColor { get; set; }
  }
}
