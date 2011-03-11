using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NrknLib.Utilities {
  public static class LogHelper {
    static LogHelper() {
      Log = new StringBuilder();
    }

    public static StringBuilder Log;
  }
}
