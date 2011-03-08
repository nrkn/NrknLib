using System;
using System.Collections.Generic;
using System.Text;

namespace NrknLib.ConsoleView {
  public class SystemConsoleView : BufferedConsoleView {
    public SystemConsoleView() {
      OnBlit = SystemConsoleBlit;
      OnClear = Console.Clear;
      OnHideCursor = () => Console.CursorVisible = false;
      OnShowCursor = () => Console.CursorVisible = true;
      OnMoveBufferArea = Console.MoveBufferArea;
      OnSetBackgroundColor = color => Console.BackgroundColor = color;
      OnSetCursorPosition = Console.SetCursorPosition;
      OnSetForegroundColor = color => Console.ForegroundColor = color;
      OnSetWindowSize = Console.SetWindowSize;
      OnWrite = Console.Write;
      OnWriteLine = Console.WriteLine;
    }

    private static void SystemConsoleBlit( IEnumerable<ConsoleCell> cells ) {
      foreach( var cell in cells ) {
        if( cell.Equals( default( ConsoleCell ) ) ) {
          SystemConsoleIncrementCursor();
          continue;
        }
        
        if( Console.ForegroundColor != cell.ForegroundColor ) Console.ForegroundColor = cell.ForegroundColor;
        if( Console.BackgroundColor != cell.BackgroundColor ) Console.BackgroundColor = cell.BackgroundColor;
        Console.Write( cell.Char );
      }
    }

    private static void SystemConsoleIncrementCursor() {
      if( Console.CursorLeft == Console.WindowWidth - 1 ) {
        SystemConsoleNewLine();
      }
      else {
        Console.CursorLeft++;
      }      
    }

    private static void SystemConsoleNewLine() {
      if( Console.CursorTop >= ( Console.WindowHeight - 1 ) ) return;

      Console.CursorLeft = 0;
      Console.CursorTop++;      
    }
  }
}
