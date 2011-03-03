using System.Collections.Generic;

namespace NrknLib.Geometry.Interfaces {
  public interface ILineCollection : IPointCollection {
    IEnumerable<ILine> Lines { get; }
  }
}
