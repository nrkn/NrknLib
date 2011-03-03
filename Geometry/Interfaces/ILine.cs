using System.Collections.Generic;

namespace NrknLib.Geometry.Interfaces {
  public interface ILine : IPointCollection {
    IPoint Start { get; set; }
    IPoint End { get; set; }
    IEnumerable<int> Verticals { get; }
    IEnumerable<int> Horizontals { get; }
    int X1 { get; set; }
    int Y1 { get; set; }
    int X2 { get; set; }
    int Y2 { get; set; }
  }
}
