using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Index
{
    static public class Globals
    {
        public static PictureMode PictureMode { get; set; } = PictureMode.Small;
    }

    public enum PictureMode
    {
        None,
        Small,
        Big
    }
}
