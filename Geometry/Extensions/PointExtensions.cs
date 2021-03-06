﻿using System;
using System.Collections.Generic;
using System.Linq;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions {
  public static class PointExtensions {
    public static IPoint Rotate( this IPoint point, double degrees ) {
      var rotatedX = point.X * Math.Cos( degrees.ToRadians() ) - point.Y * Math.Sin( degrees.ToRadians() );
      var rotatedY = point.X * Math.Sin( degrees.ToRadians() ) + point.Y * Math.Cos( degrees.ToRadians() );
      return ( Math.Abs( degrees % 90 - 0 ) < 0.0000001 ) ? 
        new Point( (int) Math.Floor( rotatedX ), (int) Math.Floor( rotatedY ) ) : 
        new Point( (int) Math.Ceiling( rotatedX ), (int) Math.Ceiling( rotatedY ) );
    }

    public static IPoint Rotate( this IPoint point, double degrees, IPoint pivot ) {
      var translate = new Point( pivot.X * -1, pivot.Y * -1 );
      var translatedPoint = point.Translate( translate );
      var rotatedPoint = translatedPoint.Rotate( degrees );
      return rotatedPoint.Translate( pivot );
    }

    public static IPoint Translate( this IPoint point, IPoint amount ) {
      return new Point( point.X + amount.X, point.Y + amount.Y );
    }

    public static IEnumerable<IPoint> Rotate( this IEnumerable<IPoint> points, double degrees ) {
      return points.Select( point => point.Rotate( degrees ) );
    }

    public static IEnumerable<IPoint> Rotate( this IEnumerable<IPoint> points, double degrees, IPoint pivot ) {
      return points.Select( point => point.Rotate( degrees, pivot ) );
    }

    public static IEnumerable<IPoint> Translate( this IEnumerable<IPoint> points, IPoint amount ) {
      return points.Select( point => point.Translate( amount ) );
    }    

    public static IEnumerable<IPoint> Neighbours( this IPoint point ) {
      return new IPoint[] {
        new Point( point.X, point.Y - 1 ),
        new Point( point.X, point.Y + 1 ),
        new Point( point.X - 1, point.Y ),
        new Point( point.X + 1, point.Y )
      };
    }

    public static IPoint Wrap( this IPoint point, IRectangle bounds ) {
      return new Point( point.X.Wrap( bounds.Width ), point.Y.Wrap( bounds.Height ) );
    }
  }
}