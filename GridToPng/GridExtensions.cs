using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NrknLib.Color;
using NrknLib.Geometry;
using NrknLib.Geometry.Extensions;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities.Extensions;

namespace GridToPng {
  public static class GridExtensions {
    public static IGrid<Rgba> ToRgba( this IGrid<bool> grid, bool hasAlpha = false ) {
      return grid.Map( b => b.ToRgba( hasAlpha ) );
    }

    public static Rgba ToRgba( this bool value, bool hasAlpha = false ) {
      var alpha = hasAlpha ? value.ToAlpha() :  Convert.ToByte( 255 );
      return new Rgba( value.ToByte(), value.ToByte(), value.ToByte(), alpha );
    }

    //black for true, white for false
    public static byte ToByte( this bool value ) {
      return Convert.ToByte( value ? 0 : 255 );
    }

    //transparent if false
    public static byte ToAlpha( this bool value ) {
      return Convert.ToByte( value ? 255 : 0 );
    }

    public static IGrid<Rgba> ToRgba( this IGrid<double> grid ) {
      return grid.Map( d => d.ToRgba() );
    }

    public static Rgba ToRgba( this double value ) {
      return new Rgba( value.ToByte(), value.ToByte(), value.ToByte() );
    }

    public static byte ToByte( this double value ) {
      return Convert.ToByte( Math.Floor( value * 255 ).Clamp( 0, 255 ) );
    }

    public static IGrid<Rgba> ToRgba( this IGrid<byte> grid ) {
      return grid.Map( b => b.ToRgba() );
    }

    public static Rgba ToRgba( this byte value ) {
      return new Rgba( value, value, value );
    }

    public static Color ToColor( this Rgba rgba ) {
      return Color.FromArgb( rgba.Alpha, rgba.Red, rgba.Green, rgba.Blue );
    }

    public static Image ToImage( this IGrid<bool> grid ) {
      return grid.ToRgba().ToImage();
    }

    public static Image ToImage( this IGrid<byte> grid ) {
      return grid.ToRgba().ToImage();
    }

    public static Image ToImage( this IGrid<double> grid ) {
      return grid.ToRgba().ToImage();
    }

    public static void SetRgba( this BitmapData bitmapData, Rgba rgba, int x, int y ) {
      //32bpp
      var offset = y * bitmapData.Stride + ( 4 * x );
      Marshal.WriteByte( bitmapData.Scan0, offset + 2, rgba.Red );
      Marshal.WriteByte( bitmapData.Scan0, offset + 1, rgba.Green );
      Marshal.WriteByte( bitmapData.Scan0, offset, rgba.Blue );
      Marshal.WriteByte( bitmapData.Scan0, offset + 3, rgba.Alpha );
    }

    public static Image ToImage( this IGrid<Rgba> grid ) {
      var bitmap = new Bitmap( grid.Size.Width, grid.Size.Height );
      var bitmapData = bitmap.LockBits(
        new System.Drawing.Rectangle( 0, 0, grid.Width, grid.Height ),
        ImageLockMode.ReadWrite,
        bitmap.PixelFormat
      );
      grid.ForEach( bitmapData.SetRgba );
      bitmap.UnlockBits( bitmapData );
      return bitmap;
    }

    public static void Save( this IGrid<Rgba> grid, string filename ) {
      grid.ToImage().Save( filename );
    }

    public static void Save( this IGrid<bool> grid, string filename ) {
      grid.ToImage().Save( filename );
    }

    public static void Save( this IGrid<byte> grid, string filename ) {
      grid.ToImage().Save( filename );
    }

    public static void Save( this IGrid<double> grid, string filename ) {
      grid.ToImage().Save( filename );
    }

    public static Rgba ToRgba( this Color color ) {
      return new Rgba( color.R, color.G, color.B, color.A );
    }

    public static Rgba GetPixel( this BitmapData bitmapData, int x, int y ) {
      //assumes 32bpp
      var offset = y * bitmapData.Stride + ( 4 * x );
      return new Rgba( 
        Marshal.ReadByte(bitmapData.Scan0, offset + 2 ),
        Marshal.ReadByte(bitmapData.Scan0, offset + 1 ),
        Marshal.ReadByte(bitmapData.Scan0, offset ),
        Marshal.ReadByte(bitmapData.Scan0, offset + 3 )
      );
    }

    public static IGrid<Rgba> ToGrid( this Bitmap bitmap ) {
      var grid = new Grid<Rgba>( bitmap.Width, bitmap.Height );

      using( var clone = new Bitmap( bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb ) ) {
        using( var graphics = Graphics.FromImage( clone ) ) {
          graphics.DrawImage( bitmap, new System.Drawing.Rectangle( 0, 0, clone.Width, clone.Height ) );
        }
        var bitmapData = clone.LockBits(
          new System.Drawing.Rectangle( 0, 0, grid.Width, grid.Height ),
          ImageLockMode.ReadWrite,
          clone.PixelFormat
        );
        grid.SetEach( bitmapData.GetPixel );
        clone.UnlockBits( bitmapData );
      }
      return grid;
    }
  }
}