using NrknLib.Geometry.Interfaces;

namespace NrknLib.Geometry {
  public struct Size : ISize {
    public Size( int width, int height ) : this() {
      Width = width;
      Height = height;
    }

    public int Width { get; set; }
    public int Height { get; set; }
  }
}