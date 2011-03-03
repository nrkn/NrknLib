using System;

namespace NrknLib.Utilities.Extensions {
  public static class NumberExtensions {
    public static int Delta( this int start, int end ) { 
      return Math.Abs( end - start );
    }

    public static int Step( this int start, int end ) {
      return start < end ? 1 : end < start ? -1 : 0;
    }

    public static double Clamp( this double x, double min, double max ) {
      return x < min ? min : x > max ? max : x;
    }

    public static double Clamp( this int x, int min, int max ) {
      return x < min ? min : x > max ? max : x;
    }

    public static double ToRadians( this double degrees ) {
      return degrees / 180 * Math.PI;
    }
  }
}