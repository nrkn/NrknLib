using System;
using System.Collections.Generic;
using NrknLib.Geometry;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.ConsoleView {
  public enum Command {
    Clear, ShowCursor, HideCursor, SetCursorPosition, SetForegroundColor, SetBackgroundColor, Write, WriteLine, Blit, SetWindowSize, MoveBufferArea
  }

  public class BufferedConsoleView : IConsoleView {
    protected int _windowWidth;
    protected int _windowHeight;
    protected readonly IGrid<ConsoleCell> Cells;

    public BufferedConsoleView() {
      _windowWidth = 80;
      _windowHeight = 25;

      Commands = new List<object>();
      Cells = new Grid<ConsoleCell>( _windowWidth, _windowHeight );

      ClearCells();

      FormatCommand = command => {
        return command.ToString();
      };

      FormatColor = color => color;
    }

    public List<object> Flush() {
      var commands = new List<object>( Commands );
      
      Commands.Clear();
      
      return commands;
    }

    public List<object> Commands;

    protected void ClearCells() {
      Cells.SetEach( point => new ConsoleCell { X = point.X, Y = point.Y } );
    }

    public void Clear() {
      Commands.Add( new {
        command = FormatCommand( Command.Clear )
      } );

      ClearCells();
      
      if( OnClear != null ) OnClear();
    }

    public Action OnClear { get; protected set; }

    public void ShowCursor() {
      Commands.Add( new {
        command = FormatCommand( Command.ShowCursor )
      } );

      if( OnShowCursor != null ) OnShowCursor();
    }

    public Action OnShowCursor { get; protected set; }

    public void SetWindowSize( int width, int height ) {
      _windowWidth = width;
      _windowHeight = height;
      
      Commands.Add( new {
        command = FormatCommand( Command.SetWindowSize ),
        width,
        height
      } );

      if( OnSetWindowSize != null ) OnSetWindowSize( width, height );
    }

    public Action<int, int> OnSetWindowSize { get; protected set; }

    public void Write( dynamic value ) {
      Write( value.ToString() );
    }

    public void WriteLine( dynamic value ) {
      WriteLine( value.ToString() );
    }

    public void HideCursor() {
      Commands.Add( new {
        command = FormatCommand( Command.HideCursor )
      } );

      if( OnHideCursor != null ) OnHideCursor();
    }

    public Action OnHideCursor { get; protected set; }

    public void SetCursorPosition( int left, int top ) {
      _cursorLeft = left;
      _cursorTop = top;
      Commands.Add( new {
        command = FormatCommand( Command.SetCursorPosition ),
        left,
        top
      } );

      if( OnSetCursorPosition != null ) OnSetCursorPosition( left, top );
    }

    public Action<int, int> OnSetCursorPosition { get; protected set; }

    protected int _cursorLeft;

    public int CursorLeft {
      get { return _cursorLeft; }
      set {
        SetCursorPosition( value, _cursorTop );
      }
    }

    protected int _cursorTop;
    public int CursorTop {
      get { return _cursorTop; }
      set {
        SetCursorPosition( _cursorLeft, value );
      }
    }

    public int WindowWidth {
      get { return _windowWidth; }
      set {
        SetWindowSize( value, _windowHeight );
      }
    }

    public int WindowHeight {
      get { return _windowHeight; }
      set {
        SetWindowSize( _windowWidth, value );
      }
    }

    protected string _foregroundColor;
    public dynamic ForegroundColor {
      get {
        return _foregroundColor;
      }
      set {
        _foregroundColor = value;

        Commands.Add( new {
          command = FormatCommand( Command.SetForegroundColor ),
          color = FormatColor( _foregroundColor )
        } );

        if( OnSetForegroundColor != null ) OnSetForegroundColor( value );
      }
    }

    public Action<dynamic> OnSetForegroundColor { get; protected set; }

    protected string _backgroundColor;
    public dynamic BackgroundColor {
      get {
        return _backgroundColor;
      }
      set {
        _backgroundColor = value;

        Commands.Add( new {
          command = FormatCommand( Command.SetBackgroundColor ),
          color = FormatColor( _backgroundColor )
        } );

        if( OnSetBackgroundColor != null ) OnSetBackgroundColor( value );
      }
    }

    public Action<dynamic> OnSetBackgroundColor { get; protected set; }

    public void Write( string value ) {
      Commands.Add( new {
        command = FormatCommand( Command.Write ),
        value
      } );

      foreach( var c in value ) {
        SetCell( new ConsoleCell{ BackgroundColor = _backgroundColor, ForegroundColor = _foregroundColor, Char = c, X = _cursorLeft, Y = _cursorTop }  );
        IncrementCursor();
      }

      if( OnWrite != null ) OnWrite( value );
    }

    public Action<string> OnWrite { get; protected set; }

    public void WriteLine( string value = null ) {
      if( value == null ) value = String.Empty;
      
      Commands.Add( new {
        command = FormatCommand( Command.WriteLine ),
        value
      } );

      foreach( var c in value ) {
        SetCell( new ConsoleCell { BackgroundColor = BackgroundColor, ForegroundColor = ForegroundColor, Char = c, X = CursorLeft, Y = CursorTop } );
        IncrementCursor();
      }
      NewLine();

      if( OnWriteLine != null ) OnWriteLine( value );
    }

    public Action<string> OnWriteLine { get; protected set; }

    protected void IncrementCursor() {
      if( _cursorLeft == _windowWidth - 1 ) {
        NewLine();
      }
      else {
        _cursorLeft++;
      }
    }

    protected void NewLine() {
      if( _cursorTop >= ( _windowHeight - 1 ) ) return;
      
      _cursorLeft = 0;
      _cursorTop++;
    }

    public void Blit( IEnumerable<ConsoleCell> cells ) {
      Commands.Add( new {
        command = FormatCommand( Command.Blit ),
        cells
      } );

      foreach( var cell in cells ) {
        if( cell.Equals( default( ConsoleCell ) ) ) {
          IncrementCursor();
        }
        else {
          SetCell( cell );
        }
      }

      if( OnBlit != null ) OnBlit( cells );
    }

    public Action<IEnumerable<ConsoleCell>> OnBlit { get; protected set; }

    public void MoveBufferArea( int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop ) {
      Commands.Add( new {
        command = FormatCommand( Command.MoveBufferArea ),
        sourceLeft,
        sourceTop,
        sourceWidth,
        sourceHeight,
        targetLeft,
        targetTop
      } );

      for( var y = sourceTop; y < sourceTop + sourceHeight; y++ ) {
        for( var x = sourceLeft; x < sourceLeft + sourceWidth; x++ ) {
          var sourceCell = GetCell( x, y );
          var targetCell = new ConsoleCell {
            BackgroundColor = sourceCell.BackgroundColor,
            Char = sourceCell.Char,
            ForegroundColor = sourceCell.ForegroundColor,
            X = targetLeft + ( x - sourceCell.X ),
            Y = targetTop + ( y - sourceCell.Y )
          };
          sourceCell.Char = ' ';
          SetCell( sourceCell );
          SetCell( targetCell );
        }
      }

      if( OnMoveBufferArea != null ) OnMoveBufferArea( sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop );
    }

    public Action<int, int, int, int, int, int> OnMoveBufferArea { get; protected set; }

    public ConsoleCell GetCell( int x, int y ) {
      return Cells[ x, y ];
    }

    protected void SetCell( ConsoleCell cell ) {
      Cells[ cell.X, cell.Y ] = cell;
    }

    public Func<Command, dynamic> FormatCommand { get; set; }
    public Func<dynamic, dynamic> FormatColor { get; set; }
  }
}