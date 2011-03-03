namespace NrknLib.Geometry.Interfaces {
  public interface IRectangle : ILineCollection {
    int Top { get; set; }
    int Bottom { get; set; }
    int Left { get; set; }
    int Right { get; set; }
    int Width { get; }
    int Height { get; }
    ISize Size { get; }
    IPoint TopLeft { get; }
    IPoint TopRight { get; }
    IPoint BottomRight { get; }
    IPoint BottomLeft { get; }
    bool IsEmpty { get; }
    bool InBounds( IPoint point );
  }
}