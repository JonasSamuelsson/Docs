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
* `<!--<docs-sample src="source.txt#name=x"/>-->`  
import sample named x from source.txt
* `<!--<docs-sample src="source.txt#lines=2-3"/>-->`  
import lines 2 to 3 from source.txt

A named sample should have the following format
```
<!--<docs-sample name="x">-->
named sample content
<!--</docs-sample>-->
```
