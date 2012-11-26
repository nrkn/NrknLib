namespace NrknLib.Color {
  public struct Rgba {
    public Rgba( byte red, byte green, byte blue, byte alpha = (byte)255 ) {
      Red = red;
      Green = green;
      Blue = blue;
      Alpha = alpha;
    }

    public byte Red;
    public byte Blue;
    public byte Green;
    public byte Alpha;

    
  }
}
