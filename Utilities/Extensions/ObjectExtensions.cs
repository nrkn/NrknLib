using System.Linq;

namespace NrknLib.Utilities.Extensions {
  public static class ObjectExtensions {
    public static bool HasMethod( this object obj, string methodName ) {
      return obj.GetType().GetMethods().Any( x => x.Name == methodName );
    }

    public static bool HasProperty( this object obj, string propertyName ) {
      return obj.GetType().GetProperties().Any( x => x.Name == propertyName );
    }
  }
}