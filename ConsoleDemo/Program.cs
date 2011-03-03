using System;
using NrknLib.ConsoleView;
using NrknLib.ConsoleView.Demo;

namespace ConsoleDemo {
  class Program {
    static void Main( string[] args ) {
      var console = new ConsoleWrapper();
      var demo = new Demo( console );
      var command = String.Empty;
      do {
        demo.Tick( command );
        command = Console.ReadKey().Key.ToCommand();
      } while( command != ConsoleKey.Escape.ToCommand() );
    }
  }

  public static class Extensions {
    public static string ToCommand( this ConsoleKey info ) {
      return ( (int) info ).ToString();
    }
  }
}
