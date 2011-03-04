using System;
using System.Collections.Generic;

namespace NrknLib.ConsoleView {
  public interface IConsoleView {
    List<object> Flush();

    void Clear();
    Action OnClear { get; }
    
    void HideCursor();
    Action OnHideCursor { get; }

    void ShowCursor();
    Action OnShowCursor { get; }
    
    void SetWindowSize( int width, int height );
    Action<int, int> OnSetWindowSize { get; }

    void Write( string value );
    Action<string> OnWrite { get; }

    void WriteLine( string value );
    Action<string> OnWriteLine { get; }

    void SetCursorPosition( int left, int top );
    Action<int, int> OnSetCursorPosition { get; }

    dynamic ForegroundColor { get; set; }
    Action<dynamic> OnSetForegroundColor { get; }

    dynamic BackgroundColor { get; set; }
    Action<dynamic> OnSetBackgroundColor { get; }

    int CursorLeft { get; set; }
    int CursorTop { get; set; }
    int WindowWidth { get; set; }
    int WindowHeight { get; set; }
    
    void Blit( IEnumerable<ConsoleCell> cells );
    Action<IEnumerable<ConsoleCell>> OnBlit { get; }

    void MoveBufferArea( int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop );
    Action<int, int, int, int, int, int> OnMoveBufferArea { get; }

    ConsoleCell GetCell( int x, int y );
  }
}
