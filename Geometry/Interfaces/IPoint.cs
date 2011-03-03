namespace NrknLib.Geometry.Interfaces {
  public interface IPoint<T> {
    T X { get; set; }
    T Y { get; set; }
  }

  public interface IPoint : IPoint<int> {
  }
}