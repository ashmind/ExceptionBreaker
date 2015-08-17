### Overview

ExceptionBreaker is a VS extension that provides a way to fast-toggle breaking on all CLR exceptions.  
It is available through both `Debug` toolbar and `Debug` top level menu.

![](%23assets/screenshots/VS2012.Toolbar.png)

### Installation

The extension can be installed from [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/50091e25-9e75-40d3-9780-a05892f474de).  
Supported VS versions: 2010, 2012, 2013, 2015.

### Details

Pressing the button sets (or unsets) breaking for all exceptions in `Common Language Runtime Exceptions` group:

<p><img src="%23assets/screenshots/Exceptions.Set.png" width="567" height="279" /></p>

Notes:  

1. If using `Just My Code`, both `Thrown` and `Unhandled` columns will be affected.  
   (I do not use `Just My Code` so please let me know if deselecting `Unhandled` makes any sense).

2. Deselected button is not equal to "Do not break on all CLR exceptions".  
   Some exceptions might be selected (manually through `Debug -> Exceptions`) — just not all of them.
   
### Options

By popular demand, version 1.1 introduces support for excluding certain exceptions from being changed:

<p><img src="%23assets/screenshots/VS2013.Options.png" width="610" height="371" /></p>

### Thanks

This extension would not exist without the help of [Sam Harwell](http://stackoverflow.com/users/138304/280z28) of [Tunnel Vision Laboratories](http://tunnelvisionlabs.com/).
