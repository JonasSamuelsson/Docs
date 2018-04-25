# docs

docs.exe is a tool that can help writing documentation in markdown (.md) files.

## Samples

docs.exe can import samples from other files

```
syntax: docs.exe samples <target>

args:
  target: file or folder to import samples into
```

### Source path

The target file should countain a docs sample tag

```
<!--<docs-sample src="path" />-->
```

The source tag supports realtive, absolute & rooted paths.

Rooted paths are paths that starts with `$/` (ie `$/dir/file.ext`.)  
The root folder is retrived from setting `samples.dir` in the [settings file](#settings-file) closest to the target file.

### Parameters

Source tag containing a `lines` parameter `src="sample.txt#lines=3-5"` would only import lines 3 to 5.

Source tags containing a `id` parameter `src="sample.txt#id=foo"` would import a sample named `foo` from `sample.txt`.

sample.txt:
```
not part of sample
<docs-sample id="foo">
sample content
</docs-sample>
not part of sample
```

### Sample language

Sample language are supported at 3 levels; inline, settings file, defaults.

The source tag supports a `language` parameter.

The [settings file](#settings-file) supports file extension to language mappings `samples.languages.cs:csharp`.

There are some build in file extension to language mappings.

## Table of content

docs.exe can generate tables of content from headers in a file.

```
syntax: docs.exe toc <target>

args:
  target: file or folder to generate table of content in
```

Table of content will be generated for files containing this tag `<!--<docs-toc />-->`.

## Settings file

Settings files are files named `.docs`, resolved from the current/parent folders of the file being processes.  
Given the following file structure `file1.md` & `file2.md` would both resolve `x:\.docs` and `file3.md` would resolve `x:\foo\bar\.docs`.
```
x:\.docs
x:\file1.md
x:\foo\file2.md
x:\foo\bar\.docs
x:\foo\bar\file3.md
```

Each line in the settings file is treated as a key value pair formatted as `key:value`.  
Lines starting with `#` are treated as comments.