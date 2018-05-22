# ShellWrapper

Want to draw behind desktop icons, and on top of wallpaper?

#### Windows 8+, [4.5.2 .net Framework](https://www.microsoft.com/net/download/dotnet-framework-runtime/net452)

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

Note: When setting a parent form/window, keyboard/mouse events will not work and will need to use low level events as,
- WH_KEYBOARD_LL
- WH_MOUSE_LL

Useful resources for User32 | [PInvoke.net](https://www.pinvoke.net/default.aspx)
