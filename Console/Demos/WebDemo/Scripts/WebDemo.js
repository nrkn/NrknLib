$( function(){
  var container = document.getElementById( 'consoleContainer' );
  jConsole.initializeConsole( container, 78, 23 );
  document.onkeypress = tick;
  
  function tick( event ) {
    var keyData = { 'Key': keyFromEvent( event ) };
    $.post( '/Console/Action/', keyData, processData, 'json' );
  }
  
  var processData = function( data ) {
    $.each( data, function( index, value ) {
      switch( value.command ) {
        case 'clear':
          jConsole.clear();
          break;
        case 'showCursor':
          jConsole.showCursor();
          break;
        case 'hideCursor':
          jConsole.hideCursor();
          break;
        case 'setCursorPosition':
          jConsole.setCursorPosition( value.left, value.top );
          break;
        case 'setForegroundColor':
          jConsole.foregroundColor = value.color;
          break;
        case 'setBackgroundColor':
          jConsole.backgroundColor = value.color;
          break;
        case 'write':
          jConsole.write( value.value || ' ' );
          break;
        case 'writeLine':
          jConsole.writeLine( value.value );
          break;
        case 'blit':
          jConsole.blit( value.cells );
          break
      }      
    } );
  }
  
  //probably not great way to handle keyboard but seems to work in major 
  //browsers
  function keyFromEvent( e ) {
    var event = e ? e : window.event ? window.event : null;
    
    if( event ) {
      var key = event.charCode ? event.charCode : event.keyCode ? event.keyCode : event.which ? event.which : null;
      return key;
    }
    
    return null;          
  }    
  
  tick( { which: -1 } );
});