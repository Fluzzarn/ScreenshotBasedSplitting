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

        public Split()
        {
            _splitImage = null;
            _splitName = null;
        }

        public Split(string name, ImageSource image)
        {
            _splitName = name;
            _splitImage = image;
        }
    }


    class Splits
    {
        private List<Split> _splits;
        private int _currentSplit = 0;

        public delegate void OnAddedSplitEventHandler(object sender, EventArgs e);
        public event OnAddedSplitEventHandler AddedSplit;

        public List<Split> CurrentSplits
        {   
            get { return _splits; }
            private set { _splits = value; }
        }


        public void OnAdded(EventArgs e)
        {
            if (AddedSplit != null)
                AddedSplit(this, e);
        }

        public void AddSplit(Split s)
        {
            _splits.Add(s);
            OnAdded(EventArgs.Empty);
        }

        public Split GetNextSplit()
        {
            if(_currentSplit + 1 < _splits.Count)
            {
                return _splits[_currentSplit + 1];
            }

            return null;
        }

        public void GotoNextSplit()
        {
            _currentSplit++;
        }

        public void GotoPreviousSplit()
        {
            _currentSplit--;
        }

    }
}
