using System;
using System.Collections.Generic;
using System.Linq;
using NrknLib.Color;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions {
  public static class DrawingExtensions {
    public static void PasteOver( this IGrid<Rgba> grid, IGrid<Rgba> pixels, IPoint location ) {
      pixels.ForEach( ( t, p ) => {
        var newPoint = new Point( location.X + p.X, location.Y + p.Y );
        var inBounds = grid.Bounds.InBounds( newPoint );
        if( inBounds && !t.Equals( new Rgba())){
          grid[ newPoint ] = t;
        }
      });
    }

    public static IEnumerable<IPoint> Bresenham( this ILine line ) {
      var deltaX = line.End.X.Delta( line.Start.X );
      var deltaY = line.End.Y.Delta( line.Start.Y );
      var stepX = line.Start.X.Step( line.End.X );
      var stepY = line.Start.Y.Step( line.End.Y );
      var error = deltaX - deltaY;
      var current = line.Start;
      var points = new List<IPoint>();

      while( true ) {
        points.Add( new Point( current.X, current.Y ) );
        if( current.Equals( line.End ) ) break;

        var error2 = 2 * error;

        if( error2 > -deltaY ) {
          error -= deltaY;
          current.X += stepX;
        }

        if( error2 >= deltaX ) continue;

        error += deltaX;
        current.Y += stepY;
      }
      return points;
    }

    public static IEnumerable<IPoint> Circle( this IPoint center, int radius ) {
      var points = new List<IPoint>();
      var x = -radius;
      var y = 0;
      var error = 2 - 2 * radius;
      do {
        points.Add( new Point( center.X - x, center.Y + y ) );
        points.Add( new Point( center.X - y, center.Y - x ) );
        points.Add( new Point( center.X + x, center.Y - y ) );
        points.Add( new Point( center.X + y, center.Y + x ) );
        radius = error;
        if( radius <= y ) {
          error += ++y * 2 + 1;
        }
        if( radius > x || error > y ) {
          error += ++x * 2 + 1;
        }
      } while( x < 0 );
      return points;
    }

    public static IEnumerable<IPoint> Arc( this ILine line ) {
      var drawQ1 = false;
      var drawQ2 = false;
      var drawQ3 = false;
      var drawQ4 = false;
      var bounds = new Rectangle();
      var deltaX = line.Start.X.Delta( line.End.X );
      var deltaY = line.Start.Y.Delta( line.End.Y );
      

      //q1
      if( line.End.X < line.Start.X && line.End.Y > line.Start.Y ) {
        drawQ1 = true;
        bounds = new Rectangle( line.Start.Y - deltaY, line.Start.X, line.End.Y, line.End.X - deltaX );
      }

      //q2
      if( line.End.X < line.Start.X && line.End.Y < line.Start.Y ) {
        drawQ2 = true;
        bounds = new Rectangle( line.End.Y - deltaY, line.Start.X + deltaX, line.Start.Y, line.End.X );
      }

      //q3
      if( line.End.X > line.Start.X && line.End.Y < line.Start.Y ) {
        drawQ3 = true;
        bounds = new Rectangle( line.End.Y, line.End.X + deltaX, line.Start.Y + deltaY, line.Start.X );
      }

      //q4 
      if( line.End.X > line.Start.X && line.End.Y > line.Start.Y ) {
        drawQ4 = true;
        bounds = new Rectangle( line.Start.Y, line.End.X, line.End.Y + deltaY, line.Start.X - deltaX );
      }

      return bounds.Ellipse( drawQ1, drawQ2, drawQ3, drawQ4 );
    }

    public static IEnumerable<IPoint> Ellipse( this IRectangle bounds, bool drawQ1 = true, bool drawQ2 = true, bool drawQ3 = true, bool drawQ4 = true ) {
      var x0 = bounds.Left;
      var y0 = bounds.Top;
      var x1 = bounds.Right;
      var y1 = bounds.Bottom;
      var points = new List<IPoint>();
      var a = Math.Abs( x1 - x0 );
      var b = Math.Abs( y1 - y0 );
      var b1 = b & 1; /* values of diameter */
      long dx = 4 * ( 1 - a ) * b * b;
      long dy = 4 * ( b1 + 1 ) * a * a; /* error increment */
      var err = dx + dy + b1 * a * a; /* error of 1.step */

      if( x0 > x1 ) {
        x0 = x1; x1 += a;
      } /* if called with swapped points */

      if( y0 > y1 ) {
        y0 = y1; /* .. exchange them */
      }

      y0 += ( b + 1 ) / 2; y1 = y0 - b1;   /* starting pixel */
      a *= 8 * a; b1 = 8 * b * b;

      do {
        if( drawQ1 ) points.Add( new Point( x1, y0 ) ); /*   I. Quadrant */
        if( drawQ2 ) points.Add( new Point( x0, y0 ) ); /*  II. Quadrant */
        if( drawQ3 ) points.Add( new Point( x0, y1 ) ); /* III. Quadrant */
        if( drawQ4 ) points.Add( new Point( x1, y1 ) ); /*  IV. Quadrant */
        var e2 = 2 * err; /* error of 1.step */

        if( e2 <= dy ) {
          y0++; y1--; err += dy += a;
        }  /* y step */

        if( e2 < dx && 2 * err <= dy ) {
          continue;
        }

        x0++; x1--; err += dx += b1;
      } while( x0 <= x1 );
/*
      while( y0 - y1 < b ) {  // too early stop of flat ellipses a=1
        points.Add( new Point( x0 - 1, y0 ) ); // -> finish tip of ellipse
        points.Add( new Point( x1 + 1, y0++ ) );
        points.Add( new Point( x0 - 1, y1 ) );
        points.Add( new Point( x1 + 1, y1-- ) );
      }
*/
      return points.Distinct();
    }

    public static IEnumerable<IPoint> Bezier( this ILine between, IPoint control ) {
      var x0 = between.Start.X;
      var y0 = between.Start.Y;
      var x1 = control.X;
      var y1 = control.Y;
      var x2 = between.End.X;
      var y2 = between.End.Y;
      var sx = x2 - x1; 
      var sy = y2 - y1;
      long xx = x0 - x1;
      long yy = y0 - y1;
      double cur = xx * sy - yy * sx;                    /* curvature */

      /* sign of gradient must not change */
      if( !( xx * sx <= 0 && yy * sy <= 0 )) {
        throw new Exception( "Sign of gradient must not change" );
      }

      var points = new List<IPoint>();
      if( sx * (long) sx + sy * (long) sy > xx * xx + yy * yy ) { /* begin with longer part */
        x2 = x0; 
        x0 = sx + x1; 
        y2 = y0; 
        y0 = sy + y1; 
        cur = -cur;  /* swap P0 P2 */
      }
      if( Math.Abs( cur - 0 ) > 0.0000001 ) {                                    /* no straight line */
        xx += sx; 
        xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
        yy += sy; 
        yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
        var xy = 2 * xx * yy;         /* relative values for checks */ 
        xx *= xx; 
        yy *= yy;          /* differences 2nd degree */
        if( cur * sx * sy < 0 ) {                           /* negated curvature? */
          xx = -xx; 
          yy = -yy; 
          xy = -xy; 
          cur = -cur;
        }
        var dx = 4.0 * sy * cur * ( x1 - x0 ) + xx - xy;
        var dy = 4.0 * sx * cur * ( y0 - y1 ) + yy - xy;
        xx += xx; 
        yy += yy; 
        var err = dx + dy + xy;
        do {          
          /* plot curve */
          points.Add( new Point( x0, y0 )  );
          if( x0 == x2 && y0 == y2 ) return points;  /* last pixel -> curve finished */
          y1 = 2 * ( err < dx ? 1 : 0 );                  /* save value for test of y step */
          if( 2 * err > dy ) {
            x0 += sx; dx -= xy; err += dy += yy;
          } /* x step */
          if( y1 == 0 ) continue;
          y0 += sy; 
          dy -= xy; 
          err += dx += xx;
        } while( dy < 0 && dx > 0 );   /* gradient negates -> algorithm fails */
      }
      
      return points;
    }

    public static IEnumerable<IPoint> DrunkenWalk( this ILine line, double drunkenness, IRectangle bounds = null ) {
      var current = new Point( line.Start.X, line.Start.Y );
      var points = new List<IPoint> { current };
      while( !current.Equals( line.End ) ) {
        var oldLocation = new Point( current.X, current.Y );

        //are we drunk? go in a random direction.
        if( RandomHelper.Random.NextDouble() < drunkenness ) {
          if( RandomHelper.Random.NextDouble() > 0.5 ) {
            current.X += RandomHelper.Random.Next( 2 ) == 1 ? 1 : -1;
          }
          else {
            current.Y += RandomHelper.Random.Next( 2 ) == 1 ? 1 : -1;
          }
        }
        //no? we still stagger randomly but only in a direction we need to go in anyway
        else {
          var deltaX = current.X.Delta( line.End.X );
          var deltaY = current.Y.Delta( line.End.Y );

          if( RandomHelper.Random.NextDouble() < ( 1.0 / ( deltaX + deltaY ) ) * deltaX ) {
            current.X = current.X + current.X.Step( line.End.X );
          }
          else {
            current.Y = current.Y + current.Y.Step( line.End.Y );
          }
        }

        if( bounds != null ) {
          if( current.X < bounds.Left || current.X > bounds.Right || current.Y < bounds.Top || current.Y > bounds.Bottom ) {
            current = oldLocation;
            continue;
          }
        }

        points.Add( current );
      }

      return points;
    }
  }
}