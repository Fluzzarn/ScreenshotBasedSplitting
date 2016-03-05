using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScreenShotSplitter
{
    class Split
    {

        private ImageSource _splitImage;

        public ImageSource SplitImage
        {
            get { return _splitImage; }
            set { _splitImage = value; }
        }

        private string _splitName;

        public string SplitName
        {
            get { return _splitName; }
            set { _splitName = value; }
        }


    }
}
