using System;
using System.Collections.Generic;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry {
  [Serializable]
  public struct Rectangle : IRectangle {
    public Rectangle( int top, int right, int bottom, int left ) : this() {
      Top = top;
      Left = left;
      Right = right;
      Bottom = bottom;
    }
    public Rectangle( ISize size ) : this( 0, size.Width - 1, size.Height - 1, 0 ) { }
    public Rectangle( int width, int height ) : this( new Size( width, height ) ) { }

    public int Top { get; set; }

    public int Bottom { get; set; }

    public int Left { get; set; }

    public int Right { get; set; }

    public int Width { get { return ( Right - Left ) + 1; } }

    public int Height { get { return ( Bottom - Top ) + 1; } }

    public ISize Size {
      get { return new Size( Width, Height ); }
    }

    public IEnumerable<IPoint> Points {
      get { return new[] {TopLeft, TopRight, BottomRight, BottomLeft}; }
    }

    public IEnumerable<ILine> Lines {
      get {
        return new ILine[] {
          new Line( TopLeft, TopRight ),
          new Line( TopRight, BottomRight ),
          new Line( BottomRight, BottomLeft ),
          new Line( BottomLeft, TopLeft )
        };
      }
    }

    public bool IsEmpty {
      get {
        return TopLeft.Equals( new Point() ) && TopLeft.Equals( BottomRight );
      }
    }

    public bool InBounds( IPoint point ) {
      return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
    }

    public IPoint TopLeft {
      get { return new Point( Left, Top ); }
    }

    public IPoint TopRight {
      get { return new Point( Right, Top );}
    }

    public IPoint BottomRight {
      get { return new Point( Right, Bottom ); }
    }

    public IPoint BottomLeft {
      get {return new Point( Left, Bottom );}
    }
  }
}