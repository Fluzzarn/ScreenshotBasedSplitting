using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScreenShotSplitter
{
    public class Split
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


        private float _threshold;

        public float Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
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


    public class Splits
    {
        private List<Split> _splits;
        private int _currentSplit = 0;

        public delegate void OnAddedSplitEventHandler(object sender, SplitsEventArgs e);
        public event OnAddedSplitEventHandler AddedSplit;

        public Splits()
        {
            _splits = new List<Split>();
        }

        public List<Split> CurrentSplits
        {   
            get { return _splits; }
            private set { _splits = value; }
        }


        public void OnAdded(SplitsEventArgs e)
        {
            if (AddedSplit != null)
                AddedSplit(this, e);
        }

        public void AddSplit(Split s)
        {
            _splits.Add(s);

            SplitsEventArgs e = new SplitsEventArgs(s);
            
            OnAdded(e);
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

        public Split GetCurrentSplit()
        {
            if(_currentSplit < _splits.Count)
            return _splits[_currentSplit];

            return null;
        }

        public void ResetSplits()
        {
            _currentSplit = 0;
        }

    }

    public class SplitsEventArgs : EventArgs
    {
        private Split _split;

        public Split Split
        {
            get { return _split; }
            private set { _split = value; }
        }

        public SplitsEventArgs(Split s)
        {
            _split = s;
        }
    }
}
