using System.Collections.Generic;

namespace NrknLib.Geometry.Interfaces {
  public interface IPointCollection {
    IEnumerable<IPoint> Points { get; }
  }
}