using System;
using System.Linq;
using System.Text;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Color.Extensions {
  public static class ColorExtensions {
    //TODO currently it discards alpha
    public static string ToHtml( this Rgba rgba ) {
      var htmlColor = "#" + rgba.Red.ToString( "X2" ) + rgba.Green.ToString( "X2" ) + rgba.Blue.ToString( "X2" );

      var r = htmlColor.Substring( 1, 2 );
      var g = htmlColor.Substring( 3, 2 );
      var b = htmlColor.Substring( 5, 2 );

      if( ( r[ 0 ] == r[ 1 ] ) && ( g[ 0 ] == g[ 1 ] ) && ( b[ 0 ] == b[ 1 ] ) ) {
        htmlColor = String.Format( "#{0}{1}{2}", r[ 0 ], g[ 0 ], b[ 0 ] );
      }

      return htmlColor;
    }

    public static Rgba ToRgba( this string htmlColor ) {
      htmlColor = htmlColor.NormalizeHexColor();
      
      var r = Convert.ToByte( htmlColor.Substring( 1, 2 ), 16 );
      var g = Convert.ToByte( htmlColor.Substring( 3, 2 ), 16 );
      var b = Convert.ToByte( htmlColor.Substring( 5, 2 ), 16 );

      return new Rgba( r, g, b );
    }

    public static string NormalizeHexColor( this string hexColor ) {
      var leadingHash = hexColor.StartsWith( "#" );
      var newColor = hexColor;

      if( leadingHash ) newColor = newColor.TrimStart( '#' );

      if( newColor.Length == 3 ) {
        var colorBuilder = new StringBuilder();
        foreach( var hex in newColor ) {
          colorBuilder.Append( hex );
          colorBuilder.Append( hex );
        }
        newColor = colorBuilder.ToString();
      }
      else {
        return hexColor;
      }

      if( leadingHash ) newColor = "#" + newColor;

      return newColor;
    }

    public static Rgba SetBrightness( this Rgba rgba, double brightness ) {
      var hsla = rgba.ToHsla();
      hsla.Lightness = brightness;
      return hsla.ToRgba();
    }

    public static Rgba ModifyBrightness( this Rgba rgba, double brightness ) {
      var hsla = rgba.ToHsla();
      hsla.Lightness *= brightness;
      return hsla.ToRgba();
    }

    public static Rgba SetSaturation( this Rgba rgba, double saturation ) {
      var hsla = rgba.ToHsla();
      hsla.Saturation = saturation;
      return hsla.ToRgba();
    }

    public static Rgba ModifySaturation( this Rgba rgba, double saturation ) {
      var hsla = rgba.ToHsla();
      hsla.Saturation *= saturation;
      return hsla.ToRgba();
    }

    public static Rgba SetHue( this Rgba rgba, double hue ) {
      var hsla = rgba.ToHsla();
      hsla.Hue = hue;
      return hsla.ToRgba();
    }

    public static Rgba ModifyHue( this Rgba rgba, double hue ) {
      var hsla = rgba.ToHsla();
      hsla.Hue *= hue;
      return hsla.ToRgba();
    }

    public static Rgba ToRgba( this Hsla hsla ) {
      var h = hsla.Hue;
      var s = hsla.Saturation;
      var l = hsla.Lightness;

      var r = l;
      var g = l;
      var b = l;
      var v = ( l <= 0.5 ) ? ( l * ( 1.0 + s ) ) : ( l + s - l * s );
      if( v > 0 ) {
        var m = l + l - v;
        var sv = ( v - m ) / v;
        h *= 6.0;
        var sextant = (int) h;
        var fract = h - sextant;
        var vsf = v * sv * fract;
        var mid1 = m + vsf;
        var mid2 = v - vsf;
        switch( sextant ) {
          case 0:
            r = v;
            g = mid1;
            b = m;
            break;
          case 1:
            r = mid2;
            g = v;
            b = m;
            break;
          case 2:
            r = m;
            g = v;
            b = mid1;
            break;
          case 3:
            r = m;
            g = mid2;
            b = v;
            break;
          case 4:
            r = mid1;
            g = m;
            b = v;
            break;
          case 5:
            r = v;
            g = m;
            b = mid2;
            break;
        }
      }
      Rgba rgba;
      rgba.Red = Convert.ToByte( r * 255.0f );
      rgba.Green = Convert.ToByte( g * 255.0f );
      rgba.Blue = Convert.ToByte( b * 255.0f );
      rgba.Alpha = Convert.ToByte( hsla.Alpha * 255 );
      return rgba;
    }

    public static Rgba Average( this Rgba rgba, Rgba rgba2, double weight = 1 ) {
      var r = ( ( rgba.Red * weight ) + rgba2.Red ) / ( 1 + weight );
      var g = ( ( rgba.Green * weight ) + rgba2.Green ) / ( 1 + weight );
      var b = ( ( rgba.Blue * weight ) + rgba2.Blue ) / ( 1 + weight );
      var a = ( ( rgba.Alpha * weight ) + rgba2.Alpha ) / ( 1 + weight );

      return new Rgba( (byte) r.Clamp( 0, 255 ), (byte) g.Clamp( 0, 255 ), (byte) b.Clamp( 0, 255 ), (byte) a.Clamp( 0, 255 ) );
    }

    public static Hsla ToHsla( this Rgba color ) {
      var r = color.Red / 255.0;
      var g = color.Green / 255.0;
      var b = color.Blue / 255.0;
      var a = color.Alpha / 255.0;

      var rgb = new[] {r, g, b};
      var min = rgb.Min();
      var max = rgb.Max();
      var maxLessMin = max - min;
      var hsla = new Hsla {Hue = 0, Saturation = 0, Lightness = 0, Alpha = a};

      var l = ( max + min ) / 2.0;

      if( l <= 0 ) return hsla;

      var s = maxLessMin;
      if( s > 0 ) {
        s /= l <= 0.5 ? max + min : 2.0 - maxLessMin;
      } else {
        return hsla;
      }

      var r2 = ( max - r ) / maxLessMin;
      var g2 = ( max - g ) / maxLessMin;
      var b2 = ( max - b ) / maxLessMin;

      double h;
      if( r == max ) {
        h = ( g == min ? 5.0 + b2 : 1.0 - g2 );
      } else if( g == max ) {
        h = ( b == min ? 1.0 + r2 : 3.0 - b2 );
      } else {
        h = ( r == min ? 3.0 + g2 : 5.0 - r2 );
      }
      h /= 6.0;

      return new Hsla {Hue = h, Saturation = s, Lightness = l, Alpha = a};
    }
  }
}