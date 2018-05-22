using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellWrapper
{
    class Example
    {

        private ShellWrapper wrapper;

        public Example()
        {
            wrapper = new ShellWrapper();

            wrapper.draw(new Action<Graphics>((d) =>
            {
                d.FillRectangle(new SolidBrush(Color.Red), 0, 0, 500, 500);
            }));

            System.Threading.Thread.Sleep(5000);
            wrapper.clearGraphics();
        }


    }
}
