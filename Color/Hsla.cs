using NrknLib.Utilities.Extensions;

namespace NrknLib.Color {
  /// <summary>
  /// Represents an HSLA (hue, saturation, lightness, aplha) color.
  /// </summary>
  public struct Hsla {
    public Hsla( double hue, double saturation, double lightness, double alpha = 1.0 ) {
      _hue = hue;
      _saturation = saturation;
      _lightness = lightness;
      _alpha = alpha;
    }
    private double _hue;
    private double _saturation;
    private double _lightness;
    private double _alpha;

    /// <summary>
    /// Gets the Hue component of an HSL color (in the range 0 - 1.0)
    /// </summary>
    /// <remarks>
    /// Numbers outside the range 0 - 1.0 will be normalized
    /// </remarks>
    public double Hue {
      get { return _hue; }
      set {
        _hue = value.Clamp( 0, 1 );
      }
    }

    /// <summary>
    /// Gets the Saturation component of an HSL color (in the range 0 - 1.0)
    /// </summary>
    /// <remarks>
    /// Numbers outside the range 0 - 1.0 will be normalized
    /// </remarks>
    public double Saturation {
      get { return _saturation; }
      set {
        _saturation = value.Clamp( 0, 1 );
      }
    }

    /// <summary>
    /// Gets the Lightness component of an HSL color (in the range 0 - 1.0)
    /// </summary>
    /// <remarks>
    /// Numbers outside the range 0 - 1.0 will be normalized
    /// </remarks>
    public double Lightness {
      get { return _lightness; }
      set {
        _lightness = value.Clamp( 0, 1 );
      }
    }

    /// <summary>
    /// Gets the Alpha component of an HSL color (in the range 0 - 1.0)
    /// </summary>
    /// <remarks>
    /// Numbers outside the range 0 - 1.0 will be normalized
    /// </remarks>
    public double Alpha {
      get { return _alpha; }
      set {
        _alpha = value.Clamp( 0, 1 );
      }
    }
  }
}