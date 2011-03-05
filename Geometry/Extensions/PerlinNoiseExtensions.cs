using System;
using NrknLib.Geometry.Interfaces;
using NrknLib.Utilities.Extensions;

namespace NrknLib.Geometry.Extensions
{
  public static class PerlinNoiseExtensions
  {
    //TODO stolen from http://www.gutgames.com/post/Perlin-Noise.aspx add proper credit
    public static IGrid<byte> Generate(this IGrid<byte> grid, double frequency, double amplitude, double persistance, int octaves)
    {
      var noise = ( new Grid<double>( grid.Width, grid.Height ) ).GenerateNoise();
      grid.SetEach( 
        (b, point ) => {
          var value = noise.GetValue(point.X, point.Y, frequency, amplitude, persistance, octaves);
          value = (value * 0.5) + 0.5;
          value *= 255;
          return (byte)((int)value).Clamp(0, 255);
        });
      return grid;
    }

    public static double GetValue(this IGrid<double> noise, int x, int y, double frequency, double amplitude, double persistance, int octaves)
    {
      var finalValue = 0.0;
      for (var i = 0; i < octaves; ++i)
      {
        finalValue += noise.GetSmoothNoise(x * frequency, y * frequency ) * amplitude;
        frequency *= 2.0;
        amplitude *= persistance;
      }
      if (finalValue < -1.0)
      {
        finalValue = -1.0;
      }
      else if (finalValue > 1.0)
      {
        finalValue = 1.0;
      }
      return finalValue;
    }

    public static double GetSmoothNoise( this IGrid<double> noise, double x, double y )
    {
      var fractionX = x - (int)x;
      var fractionY = y - (int)y;
      var x1 = ((int)x + noise.Width) % noise.Width;
      var y1 = ((int)y + noise.Height) % noise.Height;
      var x2 = ((int)x + noise.Width - 1) % noise.Width;
      var y2 = ((int)y + noise.Height - 1) % noise.Height;

      var finalValue = 0.0;
      finalValue += fractionX * fractionY * noise[x1, y1];
      finalValue += fractionX * (1 - fractionY) * noise[x1, y2];
      finalValue += (1 - fractionX) * fractionY * noise[x2, y1];
      finalValue += (1 - fractionX) * (1 - fractionY) * noise[x2, y2];

      return finalValue;
    }

    public static IGrid<double> GenerateNoise( this IGrid<double> grid )
    {
      var random = new Random();
      grid.SetEach( () => (random.NextDouble() - 0.5)*2.0);
      return grid;
    }
  }
}
