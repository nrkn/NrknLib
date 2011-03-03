using System.Collections.Generic;
using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry {
  public class PointCollection : IPointCollection {
    public PointCollection() {
      Points = new List<IPoint>();
    }

    public IEnumerable<IPoint> Points { get;set; }
  }
}
