using System;
using NrknLib.ConsoleView;
using NrknLib.ConsoleView.Demo;

namespace ConsoleDemo {
  class Program {
    static void Main( string[] args ) {
      var console = new SystemConsoleView();
      //.NET Console.MoveBufferArea is too slow for the buffering to be useful.
      //it's actually faster to just update the whole screen
      var demo = new ConsoleViewDemo( console ) {UseBuffer = false};
      var command = String.Empty;
      do {
        demo.Tick( command );
        command = Console.ReadKey().Key.ToCommand();
      } while( command != ConsoleKey.Escape.ToCommand() );

      Console.ForegroundColor = ConsoleColor.Gray;
      Console.BackgroundColor = ConsoleColor.Black;
      Console.Clear();
      Console.WriteLine( demo.Log );
      Console.ReadKey();
    }
  }

  public static class Extensions {
    public static string ToCommand( this ConsoleKey info ) {
      return ( (int) info ).ToString();
    }
  }
}
