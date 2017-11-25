using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.Controls
{
    public class ViewModelViewPlayablePagingWrapper<T> : VMPagingBase, INotifyPropertyChanged
        where T : new()
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override void NextPage()
        {
            if (CurrentPage.Next != null)
            {
                if (_loadedPages.FirstOrDefault(x => x.Href == CurrentPage.Next) is PagingWrapper<T> page)
                {
                    CurrentPage = page;
                }
                else
                {
                    LoadNewCurrentPage(CurrentPage.Next);
                }
            }
        }

        public override void PreviousPage()
        {
            if (CurrentPage.Previous != null)
            {
                if (_loadedPages.FirstOrDefault(x => x.Href == CurrentPage.Previous) is PagingWrapper<T> page)
                {
                    CurrentPage = page;
                }
                else
                {
                    LoadNewCurrentPage(CurrentPage.Previous);
                }
            }
        }

        private async void LoadNewCurrentPage(string href)
        {
            SearchResult nextResult = await DataLoader.GetInstance().GetItemFromHref<SearchResult>(href);

            PagingWrapper<T> newPage = (PagingWrapper<T>)nextResult.GetPagingWrapper<T>();

            _loadedPages.Add(newPage);
            CurrentPage = newPage;
        }

        private PagingWrapper<T> _currentPage;

        public PagingWrapper<T> CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
                NotifyPropertyChanged("PageInfo");
                if (_currentPage != null && _currentPage.Items != null && _currentPage.Items.Length > 0)
                {
                    PageItems = new ObservableCollection<T>(_currentPage.Items);
                }
            }
        }

        private ObservableCollection<T> _pageItems;

        public ObservableCollection<T> PageItems
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

        public string PageInfo
        {
            get
            {
                if (_currentPage == null)
                    return string.Empty;

                return $"Page {_pageNumber} of {_maxPages}. Total items: {_currentPage.Total}";
            }
        }

        private int _pageNumber => _currentPage == null || _currentPage.Limit == 0 ? -1 : (_currentPage.Offset / _currentPage.Limit) + 1;

        private int _maxPages => _currentPage == null || _currentPage.Limit == 0 ? -1 : (_currentPage.Total / _currentPage.Limit) + (_currentPage.Total % _currentPage.Limit > 0 ? 1 : 0);

        public override int CurrentPageNumber => _currentPage != null ? (_currentPage.Offset / _currentPage.Limit) + 1 : -1;

        private List<PagingWrapper<T>> _loadedPages;


        public ViewModelViewPlayablePagingWrapper(PagingWrapper<T> page)
        {
            _loadedPages = new List<PagingWrapper<T>> { page };
            CurrentPage = page;
        }
    }
}
