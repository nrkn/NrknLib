using System;

namespace NrknLib.Utilities.Extensions {
  public static class NumberExtensions {
    public static int Delta( this int start, int end ) { 
      return Math.Abs( end - start );
    }

    public static int Step( this int start, int end ) {
      return start < end ? 1 : end < start ? -1 : 0;
    }

    public static T Clamp<T>( this T value, T minimum, T maximum ) where T : struct, IComparable<T> {
      if( value.CompareTo( minimum ) < 0 ) {
        return minimum;
      }
      return value.CompareTo( maximum ) > 0 ? maximum : value;      
    }

    public static double ToRadians( this double degrees ) {
      return degrees / 180 * Math.PI;
    }
  }
}