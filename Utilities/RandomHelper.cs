using System;

namespace NrknLib.Utilities {
  /// <summary>
  /// Random singleton
  /// </summary>
  public static class RandomHelper {
    public static Random Random = new Random();

    private static int _seed;
    public static int Seed {
      get { return _seed; }
      set {
        _seed = value;
        Random = new Random( value );
      }
    }
  }
}