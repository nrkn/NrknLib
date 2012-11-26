using System.Collections.Generic;
using NrknLib.Color;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry.FloodFiller {
  public static class Naive {
    private static bool ColorMatch( Rgba a, Rgba b ) {
      return a.Equals( b );
    }
    
    public static void FloodFill( this IGrid<Rgba> grid, Point pt, Rgba targetColor, Rgba replacementColor ) {
      var q = new Queue<Point>();
      q.Enqueue( pt );
      while( q.Count > 0 ) {
        var n = q.Dequeue();
        if( !ColorMatch( grid[ n.X, n.Y ], targetColor ) )
          continue;
        Point w = n, e = new Point( n.X + 1, n.Y );
        while( ( w.X > 0 ) && ColorMatch( grid[ w.X, w.Y ], targetColor ) ) {
          grid[ w.X, w.Y ] = replacementColor;
          if( ( w.Y > 0 ) && ColorMatch( grid[ w.X, w.Y - 1 ], targetColor ) )
            q.Enqueue( new Point( w.X, w.Y - 1 ) );
          if( ( w.Y < grid.Height - 1 ) && ColorMatch( grid[ w.X, w.Y + 1 ], targetColor ) )
            q.Enqueue( new Point( w.X, w.Y + 1 ) );
          w.X--;
        }
        while( ( e.X < grid.Width - 1 ) && ColorMatch( grid[ e.X, e.Y ], targetColor ) ) {
          grid[ e.X, e.Y ] = replacementColor;
          if( ( e.Y > 0 ) && ColorMatch( grid[ e.X, e.Y - 1 ], targetColor ) )
            q.Enqueue( new Point( e.X, e.Y - 1 ) );
          if( ( e.Y < grid.Height - 1 ) && ColorMatch( grid[ e.X, e.Y + 1 ], targetColor ) )
            q.Enqueue( new Point( e.X, e.Y + 1 ) );
          e.X++;
        }
      }
    }
  }
}
