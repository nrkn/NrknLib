using System;
using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions {
  public static class LineExtensions {
    public static ILine Normalize( this ILine line ){
      return new Line(
        new Point( line.Verticals.Min(), line.Horizontals.Min() ),
        new Point( line.Verticals.Max(), line.Horizontals.Max() )
      );
    }

    

    public static IEnumerable<ILine> Rotate( this IEnumerable<ILine> lines, double degrees ) {
      return lines.Select( line => new Line( line.Start.Rotate( degrees ), line.End.Rotate( degrees ) ) ).Cast<ILine>();
    }

    public static IEnumerable<ILine> Rotate( this IEnumerable<ILine> lines, double degrees, IPoint pivot ) {
      return lines.Select( line => new Line( line.Start.Rotate( degrees, pivot ), line.End.Rotate( degrees, pivot ) ) ).Cast<ILine>();
    }

    public static IEnumerable<ILine> Translate( this IEnumerable<ILine> lines, IPoint amount ) {
      return lines.Select( line => new Line( line.Start.Translate( amount ), line.End.Translate( amount ) ) ).Cast<ILine>();
    }
  }
}