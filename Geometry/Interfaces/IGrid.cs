using System;
using System.Collections.Generic;

namespace NrknLib.Geometry.Interfaces {
  public interface IGrid {
    int Width { get; }
    int Height { get; }
    ISize Size { get; }
    IRectangle Bounds { get; }
    void ForEach( Action<int, int> action );
    void ForEach( Action<IPoint> action );
  }

  public interface IGrid<T> : IGrid {
    string ToString( Func<T, string> converter );
    T this[ IPoint point ] { get; set; }
    T this[ int x, int y ] { get; set; }
    void ForEach( Action<T> action );
    void ForEach( Action<T, int, int> action );
    void ForEach( Action<T, IPoint> action );
    void SetEach( T value );
    void SetEach( Func<T> func );
    void SetEach( Func<T, int, int, T> func );
    void SetEach( Func<int, int, T> func );
    void SetEach( Func<T, IPoint, T> func );
    void SetEach( Func<T, T> func );
    IGrid<T> Copy( IRectangle rectangle );
    void Paste( IGrid<T> grid, IPoint location );
    void Paste( IGrid<T> grid, IPoint location, bool wrap );
    IEnumerable<T> Cells { get; set; }
    int LeftmostWhere( IPoint start, Func<T, bool> predicate );
    int RightmostWhere( IPoint start, Func<T, bool> predicate );
  }
}