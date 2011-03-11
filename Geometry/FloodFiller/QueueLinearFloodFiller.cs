using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry.FloodFiller {
  public class QueueLinearFloodFiller {
    public QueueLinearFloodFiller( IGrid<bool> blocks ) {
      Blocks = blocks;
      
      _blocks = new bool[ blocks.Width * blocks.Height ];
      _flooded = new bool[ blocks.Width * blocks.Height ];
      _width = Blocks.Width;
      _height = Blocks.Height;

      var cells = Blocks.Cells.ToList();

      for( var i = 0; i < Blocks.Cells.Count(); i++ ) {
        _blocks[ i ] = cells[ i ];
        _flooded[ i ] = false;
      }
    }

    public IGrid<bool> Blocks { get; set; }
    public HashSet<IPoint> Flooded { 
      get {
        var flooded = new HashSet<IPoint>();
        var i = 0;        
        for( var y = 0; y < Blocks.Height; y++ ) {
          for( var x = 0; x < Blocks.Width; x++ ) {
            var point = new Point( x, y );
            if( _flooded[ i ] && !flooded.Contains( point ) ) flooded.Add( new Point( x, y ) );
            i++;
          }
        }
        return flooded;
      }
    }

    private readonly bool[] _blocks;
    private readonly bool[] _flooded;
    private readonly int _width;
    private readonly int _height;
    private FloodFillRangeQueue _ranges;

    public void FloodFill( IPoint point ) {
      _ranges = new FloodFillRangeQueue( _width * _height );
      
      LinearFill( point.X, point.Y );

      while( _ranges.Count > 0 ) {
        var range = _ranges.Dequeue();

        var upY = range.Y - 1;
        var downY = range.Y + 1;

        for( var i = range.StartX; i <= range.EndX; i++ ) {       
          if( CheckPoint( i, upY ) ) LinearFill( i, upY );
          if( CheckPoint( i, downY ) ) LinearFill( i, downY );
        }
      }
    }

    void LinearFill( int x, int y ) {
      var point = new Point( x, y );
      var leftMost = Blocks.LeftmostWhere( point, b => !b );
      var rightMost = Blocks.RightmostWhere( point, b => !b );
      var count = ( rightMost - leftMost ) + 1;
      Enumerable.Range( leftMost, count ).ToList().ForEach(
        i =>
          _flooded[ y * Blocks.Height + i ] = true
      );

      var range = new FloodFillRange( leftMost, rightMost, y );
      _ranges.Enqueue( ref range );
    }

    protected bool CheckPoint( int x, int y ) {
      var i = y * Blocks.Height + x;
      return Blocks.Bounds.InBounds( new Point( x, y ) ) && !_blocks[ i ] && !_flooded[ i ];
    }
  }
}