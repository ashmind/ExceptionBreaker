### Overview

ExceptionBreaker is a VS extension that provides a way to fast-toggle breaking on all CLR exceptions.  
It is available through both `Debug` toolbar and `Debug` top level menu.

![](%23assets/screenshots/VS2012.Toolbar.png)

### Installation

The extension can be installed from [Visual Studio Gallery]().  
Supported VS versions: 2010 and 2012.

### Details

Toggling the button toggles all exceptions in `Common Language Runtime Exceptions` group to be selected (or deselected):

<p><img src="%23assets/screenshots/Exceptions.Set.png" width="567" height="279" /></p>

Notes:  

1. If using `Just My Code`, both `Thrown` and `Unhandled` columns will be affected.  
   (I do not use `Just My Code` so please let me know if deselecting `Unhandled` makes any sense).

2. Deselected button is not equal to "Do not break on all CLR exceptions".  
   Some exceptions might be selected (manually through `Debug -> Exceptions`) — just not all of them.

### Thanks

This extension would not exist without the help of [Sam Harwell](http://stackoverflow.com/users/138304/280z28) of [Tunnel Vision Laboratories](http://tunnelvisionlabs.com/).

### Future plans

I am considering looking into Exception Assistant to quickly unselect the current exception if required.  
However after enjoyable experience of two days of unmanaged VS debugging, I am not very keen on pursuing it soon.
