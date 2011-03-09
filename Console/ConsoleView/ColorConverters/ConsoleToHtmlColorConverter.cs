using System;
using System.Collections.Generic;

namespace NrknLib.ConsoleView.ColorConverters {
  public static class ConsoleToHtmlColorConverter {
    static ConsoleToHtmlColorConverter() {
      Conversions = new Dictionary<ConsoleColor, string> {
        {ConsoleColor.Black, "#000"}, 
        {ConsoleColor.DarkBlue, "#008"}, 
        {ConsoleColor.DarkGreen, "#080"}, 
        {ConsoleColor.DarkCyan, "#088"}, 
        {ConsoleColor.DarkRed, "#800"}, 
        {ConsoleColor.DarkMagenta, "#808"}, 
        {ConsoleColor.DarkYellow, "#880"}, 
        {ConsoleColor.Gray, "#bbb"}, 
        {ConsoleColor.DarkGray, "#888"}, 
        {ConsoleColor.Blue, "#00f"}, 
        {ConsoleColor.Green, "#0f0"}, 
        {ConsoleColor.Cyan, "#0ff"}, 
        {ConsoleColor.Red, "#f00"}, 
        {ConsoleColor.Magenta, "#f0f"}, 
        {ConsoleColor.Yellow, "#ff0"}, 
        {ConsoleColor.White, "#fff"}
      };

      FormatColor = color => Conversions[ color ];
    }

    public static IDictionary<ConsoleColor, string> Conversions { get; set; }
    public static Func<dynamic, dynamic> FormatColor { get; set; }
  }
}