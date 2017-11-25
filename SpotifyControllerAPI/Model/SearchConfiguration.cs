using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class SearchConfiguration
    {
        public SearchConfiguration()
        {
            Scope = new List<string>();
        }

        public List<string> Scope { get; set; }
        public string SearchText { get; set; }
        public int Amount { get; set; }
        public int Offset { get; set; }

        public bool CompareSearch(SearchConfiguration config)
        {
            return
                   (
                      Scope.Count == config.Scope.Count 
                   && (from s1 in Scope join s2 in config.Scope on s1 equals s2 select s1).Count() == Scope.Count
                   )
                && (SearchText.ToLower() == config.SearchText.ToLower()) //this assumes that the user doesnst specify Operators in lowercase (or even at all). See https://developer.spotify.com/web-api/search-item/
                && Amount == config.Amount && Offset == config.Offset; 
        }
    }
}
