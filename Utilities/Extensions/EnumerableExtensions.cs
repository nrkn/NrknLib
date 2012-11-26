using System;
using System.Collections.Generic;
using System.Linq;

namespace NrknLib.Utilities.Extensions {
  public static class EnumerableExtensions {
    public static IEnumerable<Tuple<T, T>> Pairs<T>( this IEnumerable<T> values, bool wrap = false ) {
      var valuesCopy = values.ToArray();
      var pairs = new List<Tuple<T, T>>();
      for( var i = 0; i < valuesCopy.Count() - 1; i++ ) {
        pairs.Add( new Tuple<T, T>( valuesCopy.ToList()[ i ], valuesCopy.ToList()[ i + 1 ] ) );
      }
      if( wrap ) {
        pairs.Add( new Tuple<T, T>( valuesCopy.Last(), valuesCopy.First() ) );
      }
      return pairs; 
    }
    
    public static IEnumerable<T> Partition<T>( this IEnumerable<T> values, int size, int partition ) {
      if( size < 1 ) throw new ArgumentOutOfRangeException( "size" );
      var valuesCopy = values.ToArray();
      var take = size < valuesCopy.Count() ? size : valuesCopy.Count();
      return valuesCopy.Skip( size * partition ).Take( take );
    }

    public static IEnumerable<IEnumerable<T>> Partitions<T>( this IEnumerable<T> values, int size ) {
      if( size < 1 ) throw new ArgumentOutOfRangeException( "size" );
      var valuesCopy = values.ToArray();

      var partitions = new List<List<T>>();
      var count = Math.Ceiling( (double) valuesCopy.Count() / size );

      for( var page = 0; page < count; page++ ) {
        partitions.Add( valuesCopy.Partition( size, page ).ToList() );
      }

      return partitions;
    }

    public static IEnumerable<double> Normalize( this IEnumerable<double> values ) {
      var valuesCopy = values.ToArray();

      var min = valuesCopy.Min();
      var max = valuesCopy.Max();
      var range = max - min;
      var ratio = 1.0 / range;
      return valuesCopy.Select( x => ( x - min ) * ratio );
    }

    public static IEnumerable<byte> Normalize(this IEnumerable<byte> values)
    {
      var valuesCopy = values.ToArray();

      var min = valuesCopy.Min();
      var max = valuesCopy.Max();
      var range = max - min;
      var ratio = 255 / range;
      return valuesCopy.Select( x => (byte) ( ( x - min ) * ratio ).Clamp( 0, 255 ) );
    }
  }
}