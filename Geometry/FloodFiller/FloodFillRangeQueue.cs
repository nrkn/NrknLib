using System;

namespace NrknLib.Geometry.FloodFiller {
  /// <summary>A queue of FloodFillRanges.</summary>
  public class FloodFillRangeQueue {
    FloodFillRange[] _array;
    int _head;

    /// <summary>
    /// Returns the number of items currently in the queue.
    /// </summary>
    public int Count { get; private set; }

    public FloodFillRangeQueue()
      : this( 10000 ) {
    }

    public FloodFillRangeQueue( int initialSize ) {
      _array = new FloodFillRange[ initialSize ];
      _head = 0;
      Count = 0;
    }

    /// <summary>Gets the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
    public FloodFillRange First {
      get { return _array[ _head ]; }
    }

    /// <summary>Adds a <see cref="FloodFillRange"/> to the end of the queue.</summary>
    public void Enqueue( ref FloodFillRange r ) {
      if( Count + _head == _array.Length ) {
        var newArray = new FloodFillRange[ 2 * _array.Length ];
        Array.Copy( _array, _head, newArray, 0, Count );
        _array = newArray;
        _head = 0;
      }
      _array[ _head + ( Count++ ) ] = r;
    }

    /// <summary>Removes and returns the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
    public FloodFillRange Dequeue() {
      var range = new FloodFillRange();
      if( Count > 0 ) {
        range = _array[ _head ];
        _array[ _head ] = new FloodFillRange();
        _head++;//advance head position
        Count--;//update size to exclude dequeued item
      }
      return range;
    }
  }
}