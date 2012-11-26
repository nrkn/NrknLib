using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NrknLib.Color;
using NrknLib.Geometry.FloodFiller;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions {
  public static class GridExtensions {
    public static IGrid<T> Interpolate<T>( this IGrid<T> grid, ISize size ) {
      var xRatio = grid.Width / (double) size.Width;
      var yRatio = grid.Height / (double) size.Height;
      var newGrid = new Grid<T>( size.Width, size.Height );

      newGrid.SetEach( ( c, x, y ) => {
        var pointX = Math.Floor( x * xRatio );
        var pointY = Math.Floor( y * yRatio );
        return grid.Cells.ToList()[ (int) ( ( pointY * grid.Width ) + pointX ) ];
      } );

      return newGrid;
    }

    public static IGrid<byte> Interpolate( this IGrid<byte> grid, ISize size, bool wrap = true ) {
      var xRatio = grid.Width / size.Width;
      var yRatio = grid.Height / size.Height;
      var newGrid = new Grid<byte>( size.Width, size.Height );

      newGrid.SetEach( ( c, x, y ) => {
        var pointX = x * xRatio;
        var pointY = y * yRatio;

        var ceilingX = pointX + 1;
        if( ceilingX >= grid.Width ) ceilingX = wrap ? 0 : pointX;
        var ceilingY = pointY + 1;
        if( ceilingY >= grid.Height ) ceilingY = wrap ? 0 : pointY;

        var fractionX = c * xRatio - pointX;
        var fractionY = y * yRatio - pointY;

        var oneLessX = 255 - fractionX;
        var oneLessY = 255 - fractionY;

        var c1 = grid[ pointX, pointY ];
        var c2 = grid[ ceilingX, pointY ];
        var c3 = grid[ pointX, ceilingY ];
        var c4 = grid[ ceilingX, ceilingY ];

        var b1 = oneLessX * c1 + fractionX * c2;
        var b2 = oneLessX * c3 + fractionX * c4;


        return (byte) ( oneLessY * b1 + fractionY * b2 ).Clamp( 0, 255 );
      } );


      return newGrid;
    }

    public static IGrid<double> Interpolate( this IGrid<double> grid, ISize size, bool wrap = true ) {
      var xRatio = grid.Width / (double) size.Width;
      var yRatio = grid.Height / (double) size.Height;
      var newGrid = new Grid<double>( size.Width, size.Height );

      newGrid.SetEach( ( c, x, y ) => {
        var pointX = (int) Math.Floor( x * xRatio );
        var pointY = (int) Math.Floor( y * yRatio );

        var ceilingX = pointX + 1;
        if( ceilingX >= grid.Width ) ceilingX = wrap ? 0 : pointX;
        var ceilingY = pointY + 1;
        if( ceilingY >= grid.Height ) ceilingY = wrap ? 0 : pointY;

        var fractionX = c * xRatio - pointX;
        var fractionY = y * yRatio - pointY;

        var oneLessX = 1.0 - fractionX;
        var oneLessY = 1.0 - fractionY;

        var c1 = grid[ pointX, pointY ];
        var c2 = grid[ ceilingX, pointY ];
        var c3 = grid[ pointX, ceilingY ];
        var c4 = grid[ ceilingX, ceilingY ];

        var b1 = oneLessX * c1 + fractionX * c2;
        var b2 = oneLessX * c3 + fractionX * c4;

        return oneLessY * b1 + fractionY * b2;
      } );


      return newGrid;
    }

    public static IGrid<int> Interpolate( this IGrid<int> grid, ISize size, bool wrap = true ) {
      var doubleGrid = new Grid<double>( grid.Width, grid.Height ) {
        Cells = grid.Cells.Select( g => (double) g )
      };

      var newGrid = doubleGrid.Interpolate( size );

      return new Grid<int>( newGrid.Width, newGrid.Height ) {
        Cells = newGrid.Cells.Select( g => (int) g )
      };
    }

    public static IGrid<double> Average( this IEnumerable<IGrid<double>> grids ) {
      var gridsCopy = grids.ToArray();

      if( gridsCopy.Any( grid => !grid.Size.Equals( gridsCopy.First().Size ) ) )
        throw new ArgumentException( "grids must all be the same size", "grids" );

      var gridList = gridsCopy.ToList();
      var first = gridsCopy.First();
      var averageGrid = new Grid<double>( first.Width, first.Height ) { Cells = first.Cells };
      for( var i = 1; i < gridList.Count; i++ ) {
        var cells = averageGrid.Cells.ToList();
        var newCells = gridList[ i ].Cells.ToList();
        var newCellValues = new List<double>();
        for( var c = 0; c < newCells.Count(); c++ ) {
          newCellValues.Add( ( cells[ c ] + newCells[ c ] ) / 2 );
        }
        averageGrid.Cells = newCellValues;
      }
      return averageGrid;
    }

    public static IGrid<byte> Average( this IEnumerable<IGrid<byte>> grids ) {
      var gridList = grids.ToList();
      if( gridList.Any( grid => !grid.Size.Equals( gridList.First().Size ) ) )
        throw new ArgumentException( "grids must all be the same size", "grids" );

      var first = gridList.First();
      var averageGrid = new Grid<byte>( first.Width, first.Height ) {
        Cells = first.Cells
      };
      for( var i = 1; i < gridList.Count; i++ ) {
        var cells = averageGrid.Cells.ToList();
        var newCells = gridList[ i ].Cells.ToList();
        var newCellValues = new List<byte>();
        for( var c = 0; c < newCells.Count(); c++ ) {
          newCellValues.Add( (byte) ( ( cells[ c ] + newCells[ c ] ) / 2 ).Clamp( 0, 255 ) );
        }
        averageGrid.Cells = newCellValues;
      }
      return averageGrid;
    }

    public static void Draw( this IGrid<Rgba> grid, IEnumerable<IPoint> points, Rgba color ) {
      points.Where( point => grid.Bounds.InBounds( point ) ).ToList().ForEach( point => grid[ point ] = color );
    }

    public static string ToPgm( this IGrid<double> grid ) {
      var builder = new StringBuilder();
      builder.AppendLine( "P2" );
      builder.AppendLine( String.Format( "{0} {1}", grid.Width, grid.Height ) );
      builder.AppendLine( "255" );

      grid.ForEach( cell => {
        builder.Append( Math.Floor( grid[ cell.X, cell.Y ] * 255 ) );
        if( cell.X == grid.Width - 1 ) {
          builder.AppendLine();
        }
        else {
          builder.Append( " " );
        }
      } );

      return builder.ToString();
    }

    public static string ToPpm( this IGrid<Rgba> grid ) {
      var builder = new StringBuilder();
      builder.AppendLine( "P3" );
      builder.AppendLine( String.Format( "{0} {1}", grid.Width, grid.Height ) );
      builder.AppendLine( "255" );

      grid.ForEach( cell => {
        builder.Append( grid[ cell.X, cell.Y ].Red + " " );
        builder.Append( grid[ cell.X, cell.Y ].Green + " " );
        builder.Append( grid[ cell.X, cell.Y ].Blue );
        if( cell.X == grid.Width - 1 ) {
          builder.AppendLine();
        }
        else builder.Append( " " );
      } );

      return builder.ToString();
    }

    public static IGrid<Rgba> FromPpm( this string[] ppm ) {
      ppm = ppm.Where( l => !l.StartsWith( "#" ) ).ToArray();
      var dim = ppm[ 1 ].Split( ' ' );
      var width = int.Parse( dim[ 0 ] );
      var height = int.Parse( dim[ 1 ] );

      IGrid<Rgba> grid = new Grid<Rgba>( width, height );
      var valueBuilder = new StringBuilder();
      for( var i = 3; i < ppm.Count(); i++ ) {
        valueBuilder.Append( ppm[ i ] + " " );
      }

      var values = valueBuilder.ToString().Split( ' ' ).Where(
        s =>
          !String.IsNullOrEmpty( s )
      ).ToArray();

      if( values.Count() != ( width * height * 3 ) ) {
        throw new Exception();
      }

      var rgbas = values.Select( Byte.Parse ).Partitions( 3 ).Select(
        rgb => rgb.ToList()
      ).Select(
        rgbI => new Rgba( rgbI[ 0 ], rgbI[ 1 ], rgbI[ 2 ] )
      ).ToList();

      grid.Cells = rgbas;

      return grid;
    }

    public static string ToPgm( this IGrid<byte> grid ) {
      var builder = new StringBuilder();
      builder.AppendLine( "P2" );
      builder.AppendLine( String.Format( "{0} {1}", grid.Width, grid.Height ) );
      builder.AppendLine( "255" );

      grid.ForEach( cell => {
        builder.Append( grid[ cell.X, cell.Y ] );
        if( cell.X == grid.Width - 1 ) {
          builder.AppendLine();
        }
        else {
          builder.Append( " " );
        }
      } );

      return builder.ToString();
    }

    public static string ToPbm( this IGrid<bool> grid ) {
      var builder = new StringBuilder();
      builder.AppendLine( "P1" );
      builder.AppendLine( String.Format( "{0} {1}", grid.Width, grid.Height ) );

      grid.ForEach( cell => {
        builder.Append( grid[ cell.X, cell.Y ] ? "1" : "0" );
        if( cell.X == grid.Width - 1 ) {
          builder.AppendLine();
        }
        else {
          builder.Append( " " );
        }
      } );

      return builder.ToString();
    }

    public static IGrid<bool> FromPbm( this string[] pbm ) {
      pbm = pbm.Where( l => !l.StartsWith( "#" ) ).ToArray();
      var dim = pbm[ 1 ].Split( ' ' );
      var width = int.Parse( dim[ 0 ] );
      var height = int.Parse( dim[ 1 ] );

      IGrid<bool> grid = new Grid<bool>( width, height );
      var valueBuilder = new StringBuilder();
      for( var i = 2; i < pbm.Count(); i++ ) {
        valueBuilder.Append( pbm[ i ] + " " );
      }

      var values = valueBuilder.ToString().Split( ' ' ).Where( s => !String.IsNullOrEmpty( s ) ).ToArray();
      if( values.Count() != width * height ) throw new Exception();
      var bools = values.Select( v => v == "1" );

      grid.Cells = bools;

      return grid;
    }

    public static IGrid<double> NoiseFill( this IGrid<double> grid, int levels, bool normalize = false ) {
      var grids = new List<Grid<double>>();

      var currentWidth = grid.Width;
      var currentHeight = grid.Height;
      var currentSize = new Size( grid.Width, grid.Height );
      for( var i = 0; i < levels; i++ ) {
        var newGrid = new Grid<double>( currentWidth, currentHeight );
        newGrid.SetEach( () => RandomHelper.Random.NextDouble() );
        if( !grid.Size.Equals( currentSize ) ) newGrid = (Grid<double>) newGrid.Interpolate( new Size( grid.Width, grid.Height ) );
        grids.Add( newGrid );
        currentWidth /= 2;
        currentHeight /= 2;
        currentWidth = currentWidth < 1 ? 1 : currentWidth;
        currentHeight = currentHeight < 1 ? 1 : currentHeight;
        currentSize = new Size( currentWidth, currentHeight );
      }

      var noiseFilled = grids.Average();

      if( normalize ) {
        noiseFilled.Cells = noiseFilled.Cells.Normalize();
      }

      return noiseFilled;
    }

    public static IGrid<byte> NoiseFill( this IGrid<byte> grid, int levels, bool normalize = false ) {
      var grids = new List<Grid<byte>>();

      var currentWidth = grid.Width;
      var currentHeight = grid.Height;
      var currentSize = new Size( grid.Width, grid.Height );
      for( var i = 0; i < levels; i++ ) {
        var newGrid = new Grid<byte>( currentWidth, currentHeight );
        var bytes = new byte[] { 0 };
        newGrid.SetEach( () => {
          RandomHelper.Random.NextBytes( bytes );
          return bytes[ 0 ];
        } );
        if( !grid.Size.Equals( currentSize ) ) newGrid = (Grid<byte>) newGrid.Interpolate( new Size( grid.Width, grid.Height ) );
        grids.Add( newGrid );
        currentWidth /= 2;
        currentHeight /= 2;
        currentWidth = currentWidth < 1 ? 1 : currentWidth;
        currentHeight = currentHeight < 1 ? 1 : currentHeight;
        currentSize = new Size( currentWidth, currentHeight );
      }

      var noiseFilled = grids.Average();

      if( normalize ) {
        noiseFilled.Cells = noiseFilled.Cells.Normalize();
      }

      return noiseFilled;
    }

    public static HashSet<IPoint> FloodFill( this IGrid<bool> blocks, IPoint start ) {
      var queueLinearFloodFiller = new QueueLinearFloodFiller( blocks );
      queueLinearFloodFiller.FloodFill( start );
      return queueLinearFloodFiller.Flooded;
    }

    //only works if grid width and height are even multiples of size, need to add check
    public static IGrid<IGrid<T>> Split<T>( this IGrid<T> grid, int size ) {
      var width = grid.Width / size;
      var height = grid.Height / size;
      var split = new Grid<IGrid<T>>( width, height );
      for( var y = 0; y < height; y++ ) {
        for( var x = 0; x < width; x++ ) {
          var rect = new Rectangle( y * size, ( ( x * size ) + size ) - 1, ( ( y * size ) + size ) - 1, x * size );
          var tile = grid.Copy( rect );
          split[ x, y ] = tile;
        }
      }
      return split;
    }

    //only works if grid width and height are even multiples of size, need to add check
    public static IGrid<T> Unsplit<T>( this IGrid<IGrid<T>> grid, int size ) {
      var width = grid.Width * size;
      var height = grid.Height * size;
      var unsplit = new Grid<T>( width, height );
      for( var y = 0; y < grid.Height; y++ ) {
        for( var x = 0; x < grid.Width; x++ ) {
          var tile = grid[ x, y ];
          unsplit.Paste( tile, new Point( x * size, y * size ) );
        }
      }
      return unsplit;
    }

    public static int Difference<T>( this IGrid<T> grid, IGrid<T> other ) {
      var difference = 0;
      grid.ForEach( ( t, p ) => difference += other[ p ].Equals( t ) ? 0 : 1 );
      return difference;
    }

    public static IGrid<bool> Fov( this IGrid<bool> blocks, int radius, IPoint from ) {
      var fov = new Grid<bool>( blocks.Size );
      for( var i = 0; i < 360; i++ ) {
        var x = Math.Cos( i * 0.01745f );
        var y = Math.Sin( i * 0.01745f );
        Fov( fov, blocks, radius, from, x, y );
      }
      return fov;
    }

    private static void Fov( IGrid<bool> fov, IGrid<bool> blocks, int radius, IPoint<int> from, double x, double y ) {
      var ox = from.X + 0.5;
      var oy = from.Y + 0.5;
      for( var i = 0; i < radius; i++ ) {
        var point = new Point( (int) ox, (int) oy );
        if( fov.Bounds.InBounds( point ) ) {
          fov[ point ] = true;
        }
        if( blocks.Bounds.InBounds( point ) && blocks[ (int) ox, (int) oy ] ) {
          return;
        }
        ox += x;
        oy += y;
      }
    }

    public static IGrid<TDestination> Map<TSource, TDestination>( this IGrid<TSource> grid, Func<TSource, TDestination> map ) {
      var result = new Grid<TDestination>( grid.Size );
      result.SetEach( ( destination, point ) => map( grid[ point ] ) );
      return result;
    }
  }
}