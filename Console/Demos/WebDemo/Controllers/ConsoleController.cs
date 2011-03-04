using System.Text;
using System.Web.Mvc;
using NrknLib.ConsoleView;
using NrknLib.ConsoleView.ColorConverters;
using NrknLib.ConsoleView.Demo;

namespace WebDemo.Controllers {
  public class ConsoleController : Controller {
    public ContentResult Index() {
      Response.StatusCode = 418;
      return new ContentResult {
        Content = "<h1>I'm a teapot</h1><p>Well, what were you expecting to find here, a coffeepot?</p><hr /><em>HTCPCP Status Code 418 - I'm a little teapot, short and stout. <a href='http://en.wikipedia.org/wiki/Hyper_Text_Coffee_Pot_Control_Protocol'>http://wc3.org/</a></em>",
        ContentEncoding = Encoding.UTF8,
        ContentType = "text/html"
      };
    }

    public JsonResult Action( string key ) {
      var commands = Demo.Tick( key );
      return Json( commands );
    }

    private static string FormatJsCommand( Command command ) {
      var first = command.ToString().ToLower()[ 0 ];
      var remainder = command.ToString().Substring( 1 );
      return first + remainder;        
    }

    private ConsoleViewDemo Demo {
      get {
        if( Session[ "demo" ] == null ) {
          Session[ "demo" ] = new ConsoleViewDemo(
            new BufferedConsoleView {
              FormatCommand = FormatJsCommand,
              FormatColor = ConsoleToHtmlColorConverter.FormatColor
            }
          );
        }

        return (ConsoleViewDemo) Session[ "demo" ];          
      }
    }
  }
}