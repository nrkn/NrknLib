using System.Collections.Generic;
using System.Linq;

namespace NrknLib.ConsoleView {
  public class ConsoleWrapper {
    private readonly dynamic _console;
    public ConsoleWrapper( dynamic console = null ) {
      _console = console;
    }

    public void Clear() {
      if( _console == null ) {
        System.Console.Clear();
        return;
      }
      _console.Clear();
    }

    public void HideCursor() {
      if( _console == null ) {
        System.Console.CursorVisible = false;
        return;
      } 

      _console.HideCursor();
    }

    public void ShowCursor() {
      if( _console == null ) {
        System.Console.CursorVisible = true;
        return;
      } 

      _console.ShowCursor();
    }

    public void SetWindowSize( int width, int height ) {
      if( _console == null ) {
        System.Console.SetWindowSize( width, height );
        return;
      }
      
      _console.SetWindowSize( width, height );
    }

    public void Write( dynamic value ) {
      if( _console == null ) {
        System.Console.Write( value );
        return;
      }  
   
      _console.Write( value );
    }

    public void WriteLine( dynamic value ) {
      if( _console == null ) {
        System.Console.WriteLine( value );
        return;
      }  
      
      _console.WriteLine( value );
    }

    public void SetCursorPosition( int left, int top ) {
      if( _console == null ) {
        System.Console.SetCursorPosition( left, top );
        return;
      }  

      _console.SetCursorPosition( left, top );
    }

    public dynamic ForegroundColor {
      get {
        return _console == null ? System.Console.ForegroundColor : _console.ForegroundColor;
      }
      set {
        if( _console == null ) {
          System.Console.ForegroundColor = value;
          return;
        } 
        _console.ForegroundColor = value;
      }
    }

    public dynamic BackgroundColor {
      get {
        return _console == null ? System.Console.BackgroundColor : _console.BackgroundColor;
      }
      set {
        if( _console == null ) {
          System.Console.BackgroundColor = value;
          return;
        } 
        _console.BackgroundColor = value;
      }
    }

    public List<object> Flush() {
      return _console != null && _console.HasMethod( "Flush" ) ? _console.Flush() : new List<object>();
    }

    public void Blit( IEnumerable<CellData> cells ) {
      if( _console != null && _console.HasMethod( "Blit" ) ) {
        _console.Blit( cells );
        return;
      }

      foreach( var cell in cells ) {
        ForegroundColor = cell.F;
        BackgroundColor = cell.B;
        Write( cell.C );
      }
    }
  }

  public static class Extensions {
    public static bool HasMethod( this object obj, string methodName ) {
      return obj.GetType().GetMethods().Any( x => x.Name == methodName );
    }
  }
}