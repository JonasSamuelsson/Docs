# docs

docs is a tool for auto generating documentation

## Samples

docs.exe can import samples from other files

`c:\source.txt`
```
1
<!--<docs:sample name="x">-->
2
<!--</docs:sample>-->
```

`c:\target.md`
```
look at this sample
<!--<docs:sample src="source.txt"/>-->
<!--<docs:sample src="source.txt#name=x"/>-->
<!--<docs:sample src="source.txt#lines=3:1"/>-->
<!--<docs:sample src="source.txt#lines=3-3"/>-->
doesn't it look great
```

run `docs.exe sample c:\target.md`

`c:\target.txt`
```
look at this sample
<!--<docs:sample src="source.txt">-->
``
sample content
``
<!--<docs:sample src="source.txt">-->
doesn't it look great
```
