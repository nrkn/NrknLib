using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry {
  /// <summary>
  /// A 2D grid of T
  /// </summary>
  /// <typeparam name="T">Anything</typeparam>
  [Serializable]
  public class Grid<T> : IGrid<T> {
    /// <summary>
    /// Grid constructor
    /// </summary>
    /// <param name="size">The size of the grid</param>
    public Grid( ISize size )
      : this( size.Width, size.Height ) {

    }
    /// <summary>
    /// Grid constructor
    /// </summary>
    /// <param name="width">The width of the grid</param>
    /// <param name="height">The height of the grid</param>
    public Grid( int width = 0, int height = 0 ) {
      _grid = new List<List<T>>();
      _width = width;
      _height = height;

      if( Width == 0 || Height == 0 ) return;

      Initialize();
    }

    public IGrid<T> Copy( IRectangle rectangle = null ) {
      rectangle = rectangle ?? Bounds;
      var grid = new Grid<T>( rectangle.Size );      
      grid.SetEach( ( t, p ) => {
        var newPoint = new Point( rectangle.Left + p.X, rectangle.Top + p.Y );
        return Bounds.InBounds( newPoint ) ? this[ newPoint ] : default( T );
      } );
      return grid;
    }

    public void Paste( IGrid<T> grid, IPoint location ) {
      grid.ForEach( ( t, p ) => {
        var newPoint = new Point( location.X + p.X, location.Y + p.Y );
        if( Bounds.InBounds( newPoint ) ) this[ newPoint ] = t;
      } );
    }

    /// <summary>
    /// The grid width
    /// </summary>
    public int Width {
      get { return _width; }
    }

    /// <summary>
    /// The grid height
    /// </summary>
    public int Height {
      get { return _height; }
    }

    public ISize Size {
      get { return new Size( _width, _height ); }
    }

    /// <summary>
    /// Bounding box for the grid
    /// </summary>
    public IRectangle Bounds {
      get {
        return new Rectangle {
          Top = 0,
          Left = 0,
          Right = Width - 1,
          Bottom = Height - 1
        };
      }
    }

    private void Initialize() {
      _grid.Clear();
      for( var y = 0; y < Height; y++ ) {
        _grid.Add( new List<T>( Width ) );
        for( var x = 0; x < Width; x++ ) {
          _grid[ y ].Add( default( T ) );
        }
      }
    }

    private readonly int _width;
    private readonly int _height;
    private readonly List<List<T>> _grid;

    public override string ToString() {      
      if( typeof( T ) == typeof( double ) ) {
        return ToString( cell => {
          dynamic t = cell;
          return ( (int) ( Math.Floor( t * 10 ) ) ).Clamp( 0, 9 ).ToString();
        } );
      }

      var builder = new StringBuilder();
      foreach( var row in _grid ) {
        foreach( var cell in row ) {
          builder.Append( cell.ToString() );
        }
        builder.AppendLine();
      }
      return builder.ToString();
    }

    public string ToString( Func<T, string> converter ) {
      var builder = new StringBuilder();
      foreach( var row in _grid ) {
        foreach( var cell in row ) {
          builder.Append( converter( cell ) );
        }
        builder.AppendLine();
      }
      return builder.ToString();
    }

    public T this[ IPoint point ] {
      get { return this[ point.X, point.Y ]; }
      set { this[ point.X, point.Y ] = value; }
    }

    public void ForEach( Action<IPoint> action ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          action( new Point( x, y ) );
        }
      }
    }

    public void SetEach( T value ) {
      SetEach( () => value  );
    }

    /// <summary>
    /// Set each cell in the grid
    /// </summary>
    /// <param name="func">T Func()</param>
    public void SetEach( Func<T> func ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          _grid[ y ][ x ] = func();
        }
      }
    }

    /// <summary>
    /// Set each cell in the grid
    /// </summary>
    /// <param name="func">T Func( T currentValue )</param>
    public void SetEach( Func<T, T> func ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          _grid[ y ][ x ] = func( _grid[ y ][ x ] );
        }
      }
    }

    /// <summary>
    /// Set each cell of the grid
    /// </summary>
    /// <param name="func">T Func( T currentValue, int x, int y )</param>
    public void SetEach( Func<T, int, int, T> func ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          _grid[ y ][ x ] = func( _grid[ y ][ x ], x, y );
        }
      }
    }

    /// <summary>
    /// Set each cell of the grid
    /// </summary>
    /// <param name="func">T Func( T currentValue, int x, int y )</param>
    public void SetEach( Func<T, IPoint, T> func ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          _grid[ y ][ x ] = func( _grid[ y ][ x ], new Point( x, y ) );
        }
      }
    }

    /// <summary>
    /// Perform an action with each cell of the grid
    /// </summary>
    /// <param name="action">void Action( T currentValue )</param>
    public void ForEach( Action<T> action ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          action( _grid[ y ][ x ] );
        }
      }
    }

    /// <summary>
    /// Perform an action with each cell of the grid
    /// </summary>
    /// <param name="action">void Action( T currentValue, int x, int y )</param>
    public void ForEach( Action<T, int, int> action ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          action( _grid[ y ][ x ], x, y );
        }
      }
    }

    public void ForEach( Action<int, int> action ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          action( x, y );
        }
      }
    }

    public void ForEach( Action<T, IPoint> action ) {
      for( var y = 0; y < Height; y++ ) {
        for( var x = 0; x < Width; x++ ) {
          action( _grid[ y ][ x ], new Point( x, y ) );
        }
      }
    }

    /// <summary>
    /// The grid cells. If you pass too many cells it will ignore the extra ones. If you pass too few it will fill the grid out with default( T )
    /// </summary>
    public IEnumerable<T> Cells {
      get {
        var cells = new List<T>();
        ForEach( cells.Add );
        return cells;
      }
      set {
        var stack = new Stack<T>( value.Reverse() );
        SetEach( c => stack.Count > 0 ? stack.Pop() : default( T ) );
      }
    }

    public IEnumerable<IEnumerable<T>> Rows {
      get {
        return Cells.Partitions( Width );
      }
    }

    public IEnumerable<IEnumerable<T>> Columns {
      get {
        var columns = new List<List<T>>();
        for( var x = 0; x < Width; x++ ) {
          var column = new List<T>();
          for( var y = 0; y < Height; y++ ) {
            column.Add( this[ x, y ] );
          }
          columns.Add( column );
        }
        return columns;
      }
    }

    /// <summary>
    /// Gets or sets a cell
    /// </summary>
    /// <param name="x">The x location of the cell</param>
    /// <param name="y">The y location of the cell</param>
    /// <returns>The cell at [ x, y ]</returns>
    public T this[ int x, int y ] {
      get {
        return _grid[ y ][ x ];
      }
      set {
        _grid[ y ][ x ] = value;
      }
    }
  }
}