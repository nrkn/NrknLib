using System.Collections.Generic;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry {
  public class LineCollection : ILineCollection {
    public LineCollection() {
      Points = new List<IPoint>();
      Lines = new List<ILine>();
    }
    public IEnumerable<IPoint> Points { get; private set; }
    public IEnumerable<ILine> Lines { get; set; }
  }
}
