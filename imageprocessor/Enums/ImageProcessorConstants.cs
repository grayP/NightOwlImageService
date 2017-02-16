using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Enums
{   public enum Set_Program_Property : uint
        {
            RepititionPlayTimes = 1,
            ProgramPlayTime = 2,
            CodeConversion
        }


        public enum RenderMode : int
        {
            Center = 0,
            Zoom_to_fit_the_window,
            Stretch_to_fit_the_window,
            Tile,
            LeftTop,
            Vertical_multipage,
            Horizontal_multi_page_Align_left,
            Horizontal_multi_page_Align_right

        }
    public class ImageProcessorConstants
    {
     
    }
}
