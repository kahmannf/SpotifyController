using SpotifyControllerAPI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.Controls
{
    public class ViewModelAggregationSearchResult : VMPagingBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ViewModelAggregationSearchResult(PlaylistAggregationSearchResult result)
        {
            _result = result;
            PageSize = 20;
            _currentPageNumber = 0;
            LoadPage();
        }

        private PlaylistAggregationSearchResult _result;

        private int _currentPageNumber;

        public override int CurrentPageNumber => _currentPageNumber;

        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                _currentPageNumber = 0;
                NotifyPropertyChanged("PageSize");
                NotifyPropertyChanged("PageInfo");
                LoadPage();
            }
        }

        private ObservableCollection<AggregationSearchTrackItem> _pageItems;

        public ObservableCollection<AggregationSearchTrackItem> PageItems
        {
            get
            {
                return _pageItems;
            }
            set
            {
                _pageItems = value;
                NotifyPropertyChanged("PageItems");
            }
        }

        private int _maxPages => _result.Tracks.Count / PageSize;

        public string PageInfo
        {
            get
            {
                if (_result == null)
                    return string.Empty;

                return $"Page {_currentPageNumber} of {_maxPages}. Total items: {_result.Tracks.Count}";
            }
        }


        public override void NextPage()
        {
            if (_currentPageNumber == _maxPages)
            {
                return;
            }

            _currentPageNumber++;

            LoadPage();
        }

        public override void PreviousPage()
        {
            if (CurrentPageNumber == 0)
            {
                return;
            }

            _currentPageNumber--;

            LoadPage();
        }

        private void LoadPage()
        {
            PageItems = new ObservableCollection<AggregationSearchTrackItem>(_result.GetPage(_currentPageNumber, PageSize).Where(x => x != null));


            NotifyPropertyChanged("CurrentPageNumber");
            NotifyPropertyChanged("PageInfo");
        }

    }
}
