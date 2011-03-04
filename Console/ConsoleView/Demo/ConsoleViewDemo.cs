using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;

namespace NrknLib.ConsoleView.Demo {
  public enum Direction {
    None, Left, Right, Up, Down
  }

  public class ConsoleViewDemo {
    public ConsoleViewDemo( IConsoleView console ) {
      Console = console;
      _location = new Point( 0, 0 );
      _log = new StringBuilder();
      GenerateLevel();
      UseBuffer = true;
    }

    public IConsoleView Console { get; set; }
    public bool UseBuffer { get; set; }
    private readonly StringBuilder _log;
    public string Log {
      get { return _log.ToString(); }
    }

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
      var direction = ExecuteAction( command );

      var source = new Rectangle( _viewportSize );
      var target = new Point( 0, 0 );
      var oldCenter = new Point( Center.X, Center.Y );
      if( UseBuffer ) {
        switch( direction ) {
          case Direction.Left:
            source.Right--;
            target.X++;
            oldCenter.X++;
            break;
          case Direction.Right:
            source.Left++;
            oldCenter.X--;
            break;
          case Direction.Up:
            source.Bottom--;
            target.Y++;
            oldCenter.Y++;
            break;
          case Direction.Down:
            source.Top++;
            oldCenter.Y--;
            break;
        }
      }

      Console.SetCursorPosition( 0, 0 );

      var viewportGrid = _noise.Copy( Viewport );

      var tiles = new List<ConsoleCell>();

      viewportGrid.ForEach( p => {
        var point = new Point( p.X + _location.X, p.Y + _location.Y );

        var draw =
          !UseBuffer
          || direction == Direction.None
          || p.Equals( Center )
          || p.Equals( oldCenter )
          || ( direction == Direction.Left && p.X == 0 )
          || ( direction == Direction.Right && p.X == viewportGrid.Width - 1 )
          || ( direction == Direction.Up && p.Y == 0 )
          || ( direction == Direction.Down && p.Y == viewportGrid.Height - 1 );

        tiles.Add(   
          draw? new ConsoleCell {
            ForegroundColor = GetForegroundColor( p, point ),
            BackgroundColor = GetBackgroundColor( point ),
            Char = p.Equals( Center ) ? '@' : GetTile( point )
          } 
          : default( ConsoleCell )
        );
      } );

      if( UseBuffer ) Console.MoveBufferArea( source.Left, source.Top, source.Width, source.Height, target.X, target.Y );
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

    private Direction ExecuteAction( string command ) {
      switch( command ) {
        case "38":
          _location.Y--;
          return Direction.Up;
        case "40":
          _location.Y++;
          return Direction.Down;
        case "37":
          _location.X--;
          return Direction.Left;
        case "39":
          _location.X++;
          return Direction.Right;
      }
      return Direction.None;
    }

    private void GenerateLevel() {
      var noiseStart = DateTime.Now;
      GenerateNoise();

      var pathsStart = DateTime.Now;
      GeneratePaths();

      var riversStart = DateTime.Now;
      GenerateRivers();

      var wallsStart = DateTime.Now;
      GenerateWalls();

      var colorsStart = DateTime.Now;
      GenerateColors();

      var treesStart = DateTime.Now;
      GenerateTrees();

      var end = DateTime.Now;

      _log.AppendLine( "Generation time" );
      _log.AppendLine( "  Noise   " + ( pathsStart - noiseStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  Paths   " + ( riversStart - pathsStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  Rivers  " + ( wallsStart - riversStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  Walls   " + ( colorsStart - wallsStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  Colors  " + ( treesStart - colorsStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  Trees   " + ( end - treesStart ).TotalMilliseconds + "ms" );
      _log.AppendLine( "  " + new String( '-', 20 ) );
      _log.AppendLine( "  TOTAL   " + ( end - noiseStart ).TotalMilliseconds + "ms" );
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

    private void GenerateRivers() {
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