using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions {
  public static class GridExtensions {
    public static IGrid<T> Interpolate<T>( this IGrid<T> grid, ISize size ) {
      var xRatio = grid.Width / (double) size.Width;
      var yRatio = grid.Height / (double) size.Height;
      var newGrid = new Grid<T>( size.Width, size.Height );

      newGrid.SetEach( (c, x, y) => {
        var pointX = Math.Floor( x * xRatio );
        var pointY = Math.Floor( y * yRatio );
        return grid.Cells.ToList()[ (int) ( ( pointY * grid.Width ) + pointX ) ];          
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
      if( grids.Any( grid => !grid.Size.Equals( grids.First().Size ) ) ) 
        throw new ArgumentException( "grids must all be the same size", "grids" );

      var gridList = grids.ToList();
      var first = grids.First();
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

    public static string ToPgm( this IGrid<double> grid ) {
      var builder = new StringBuilder();
      builder.AppendLine( "P2" );
      builder.AppendLine( String.Format( "{0} {1}", grid.Width, grid.Height ) );
      builder.AppendLine( "255" );
      
      grid.ForEach( cell => {
        builder.Append( Math.Floor( grid[ cell.X, cell.Y ] * 255 ) );
        if( cell.X == grid.Width - 1 ) builder.AppendLine(); 
        else builder.Append( " " );
      } );

      return builder.ToString();
    }

    public static IGrid<double> NoiseFill( this IGrid<double> grid, int levels, bool normalize = false ) {
      var grids = new List<Grid<double>>();

      var currentWidth = grid.Width;
      var currentHeight = grid.Height;
      for( var i = 0; i < levels; i++ ) {
        var newGrid = new Grid<double>( currentWidth, currentHeight );
        newGrid.SetEach( () => RandomHelper.Random.NextDouble() );
        newGrid = (Grid<double>) newGrid.Interpolate( new Size( grid.Width, grid.Height ) );
        grids.Add( newGrid );
        currentWidth /= 2;
        currentHeight /= 2;
        currentWidth = currentWidth < 1 ? 1 : currentWidth;
        currentHeight = currentHeight < 1 ? 1 : currentHeight;
      }

      var noiseFilled = grids.Average();

      if( normalize ) {
        noiseFilled.Cells = noiseFilled.Cells.Normalize();
      }

      return noiseFilled;
    }
  }
}