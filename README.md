# ShellWrapper

```cs

ShellWrapper wrapper = new ShellWrapper();

wrapper.draw(new Action<Graphics>((d) => {
  d.FillRectangle(new SolidBrush(Color.Red), 0, 0, 500, 500);
}))


// invalidate graphics
wrapper.clearGraphics();

```
