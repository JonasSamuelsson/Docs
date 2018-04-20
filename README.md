# docs

docs.exe is a tool that can help writing documentation in markdown (.md) files.

## Samples

docs.exe can import samples from other files

```
syntax: docs.exe sample <target>

args:
  target: file or folder to import samples into
```

The target file should countain a docs sample tag

* `<!--<docs-sample src="source.txt"/>-->`  
import everything from source.txt
* `<!--<docs-sample src="source.txt#id=x"/>-->`  
import sample named x from source.txt
* `<!--<docs-sample src="source.txt#lines=2"/>-->`  
import line 2 from source.txt
* `<!--<docs-sample src="source.txt#lines=3-5"/>-->`  
import lines 3 to 5 from source.txt

A named sample should have the following format
```
<!--<docs-sample id="x">-->
named sample content
<!--</docs-sample>-->
```
