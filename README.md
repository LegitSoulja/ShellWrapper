# ShellWrapper

Want to draw behind desktop icons, and on top of wallpaper?

```cs

ShellWrapper wrapper = new ShellWrapper();

wrapper.draw(new Action<Graphics>((d) => {
  d.FillRectangle(new SolidBrush(Color.Red), 0, 0, 500, 500);
}))


// invalidate graphics
wrapper.clearGraphics();

```

Want to set a form?

```cs

wrapper.drawForm(System.Windows.Forms.Form);
```

Want to set a window (WPF)

```cs
wrapper.drawWindow(System.Windows.Window)
```
