using System;
using System.Collections.Generic;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry {
  [Serializable]
  public struct Line : ILine {
    public Line( IPoint start, IPoint end ) : this() {
      Start = start;
      End = end; 
    }

    public Line( int x1, int y1, int x2, int y2 ) : this( new Point( x1, y1 ), new Point( x2, y2 ) ) {}

    public IPoint Start { get; set; }
    public IPoint End { get; set; }

    public IEnumerable<int> Verticals {
      get { return new[] {Start.X, End.X}; }
    }

    public IEnumerable<int> Horizontals {
      get { return new[] {Start.Y, End.Y}; }
    }

    public IEnumerable<IPoint> Points {
      get { return new[] {Start, End}; }
    }

    public int X1 {
      get { return Start.X; }
      set { Start.X = value; }
    }

    public int X2 {
      get { return End.X; }
      set { End.X = value; }
    }

    public int Y1 {
      get { return Start.Y; }
      set { Start.Y = value; }
    }

    public int Y2 {
      get { return End.Y; }
      set { End.Y = value; }
    }
  }
}