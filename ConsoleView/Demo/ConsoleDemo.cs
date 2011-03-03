using System;
using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;

namespace NrknLib.ConsoleView.Demo {
  public class Demo {
    public Demo( ConsoleWrapper console ) {
      Console = console;
      _location = new Point( 0, 0 );
      GenerateLevel();
    }

    public ConsoleWrapper Console { get; set; }

    private IGrid<double> _noise;
    private IGrid<bool> _paths;
    private IGrid<bool> _rivers;
    private IGrid<bool> _walls;
    private IGrid<double> _colors;
    private IGrid<char> _trees;

    private Size _viewportSize = new Size( 80, 24 );
    private readonly Size _gridSize = new Size( 100, 40 );
    private Point _location;
    
    private Point Center {
      get { return new Point( _viewportSize.Width / 2, _viewportSize.Height / 2 ); }
    }

    private IRectangle Viewport {
      get {
        return new Rectangle(
          _location.Y,
          _location.X + ( _viewportSize.Width - 1 ),
          _location.Y + ( _viewportSize.Height - 1 ),
          _location.X
        );
      }
    }

    public List<object> Tick( string command ) {
      ExecuteAction( command );

      Console.SetCursorPosition( 0, 0 );

      var viewportGrid = _noise.Copy( Viewport );

      var tiles = new List<CellData>();

      viewportGrid.ForEach( p => {
        var point = new Point( p.X + _location.X, p.Y + _location.Y );

        tiles.Add( 
          new CellData {
            F = GetForegroundColor( p, point ),
            B = GetBackgroundColor( point ),
            C = p.Equals( Center ) ? '@' : GetTile( point )
          } 
        );
      } );

      Console.Blit( tiles );

      return Console.Flush();
    }

    private char GetTile( Point point ) {
      return 
        !_noise.Bounds.InBounds( point ) ? ' '
        : _walls[ point ] && _paths[ point ] ? '+'
        : _walls[ point ] ? '#'
        : _paths[ point ] && _rivers[ point ] ? '='
        : _paths[ point ] ? '.'
        : _rivers[ point ] ? '~'
        : _trees[ point ];
    }

    private ConsoleColor GetForegroundColor( IPoint p, Point point ) {
      return 
        p.Equals( Center ) || !_noise.Bounds.InBounds( point ) ? ConsoleColor.White
        : _walls[ point ] ? ConsoleColor.DarkGray
        : _paths[ point ] && _rivers[ point ] ? ConsoleColor.DarkRed
        : _paths[ point ] ? ConsoleColor.DarkGreen
        : _rivers[ point ] ? ConsoleColor.Blue
        : _colors[ point ] < 0.75 ? ConsoleColor.Green
        : _colors[ point ] < 0.96 ? ConsoleColor.Yellow
        : _colors[ point ] < 0.97 ? ConsoleColor.DarkRed
        : _colors[ point ] < 0.98 ? ConsoleColor.DarkMagenta
        : ConsoleColor.DarkCyan;
    }

    private ConsoleColor GetBackgroundColor( Point point ) {
      return 
        !_noise.Bounds.InBounds( point ) ? ConsoleColor.Black
        : _walls[ point ] ? ConsoleColor.Gray
        : _paths[ point ] && _rivers[ point ] ? ConsoleColor.DarkYellow
        : _paths[ point ] ? ConsoleColor.Green
        : _rivers[ point ] ? ConsoleColor.DarkBlue
        : ConsoleColor.DarkGreen;
    }

    private void ExecuteAction( string command ) {
      switch( command ) {
        case "38":
          _location.Y--;
          break;
        case "40":
          _location.Y++;
          break;
        case "37":
          _location.X--;
          break;
        case "39":
          _location.X++;
          break;
      }
    }

    private void GenerateLevel() {
      GenerateNoise();
      GeneratePaths();
      GenerateRiver();
      GenerateWalls();
      GenerateColors();
      GenerateTrees();
    }

    private void GenerateTrees() {
      _trees = new Grid<char>( _gridSize );
      _noise.ForEach(
        ( t, p ) =>
          _trees[ p ] = DoubleToForestItem( t )
      );
    }

    private void GenerateColors() {
      _colors = new Grid<double>( _gridSize );
      _colors.SetEach( RandomHelper.Random.NextDouble );
    }

    private void GenerateWalls() {
      _walls = new Grid<bool>( _gridSize );
      var building = new Rectangle( new Size( 5, 6 ) );

      var wallPoints = new List<IPoint>();
      foreach( var line in building.Lines ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      foreach( var line in building.Lines.Translate( new Point( 10, 7 ) ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      var rotated1 = new Point( 50, 7 );
      foreach( var line in building.Lines.Translate( rotated1 ).Rotate( 45, rotated1 ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      var rotated3 = new Point( 35, 7 );
      foreach( var line in building.Lines.Translate( rotated3 ).Rotate( 90, rotated3 ) ) {
        wallPoints.AddRange( line.Bresenham() );
      }

      foreach( var point in wallPoints ) {
        _walls[ point ] = true;
      }
    }

    private void GenerateRiver() {
      _rivers = new Grid<bool>( _gridSize );
      var line = new Line( new Point( 18, 1 ), new Point( _gridSize.Width - 20, _gridSize.Height - 1 ) );
      var points = line.DrunkenWalk( 0.75, _rivers.Bounds ).Distinct();

      foreach( var point in points ) {
        _rivers[ point ] = true;
      }
    }

    private void GeneratePaths() {
      _paths = new Grid<bool>( _gridSize );

      var line1 = new Line( new Point( 1, 1 ), new Point( _gridSize.Width - 1, _gridSize.Height - 1 ) );
      var line2 = new Line( new Point( 1, _gridSize.Height - 1 ), new Point( _gridSize.Width - 1, 1 ) );
      var line3 = new Line( new Point( 1, 1 ), _location );

      var lines = new[] { line1, line2, line3 };
      var points = lines.SelectMany( line => line.DrunkenWalk( 0.5, _noise.Bounds ) ).Distinct();

      foreach( var point in points ) {
        _paths[ point ] = true;
      }
    }

    private void GenerateNoise() {
      _noise = new Grid<double>( _gridSize ).NoiseFill( 5, true );
    }

    static char DoubleToForestItem( double value ) {
      return
        RandomHelper.Random.NextDouble() > value ?
          value < 0.5 ? '♠'
          : value < 0.6 ? '♣'
          : value < 0.7 ? 'T'
          : 't'
        : '.';
    }
  }
}