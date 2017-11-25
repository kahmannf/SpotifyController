using Microsoft.Win32;
using Newtonsoft.Json;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI
{
    public static class ApiHelper
    {

        public static async Task<List<Track>> GetTracks(SpotifyBaseObject spi, User user)
        {
            DataLoader dataLoader = DataLoader.GetInstance();

            IEnumerable<Track> result;

            switch (spi)
            {
                case Playlist pl:
                    result = (await dataLoader.GetAllItemsFromPagingWrapper(pl.Tracks)).Select(x => x.Track);
                    break;
                case Artist artist:
                    List<Album> albums = await dataLoader.GetArtitstAllAlbums(artist.Id, user.Country);

                    result =
                        (
                            await
                            albums
                            .Select(async (x) => await GetTracks(x, user))                //returns IEnumerable<Task<IEnumerable<Track>>>
                            .Aggregate(async (y, z) => new List<Track>((await y).Union(await z)))    //returns Task<IEnumerable<Track>>
                        )                                                           //returns IEnumerable<Track>
                        .Distinct(new SpotifyObjectEqualityComparere())             //filters track (propably useless)
                        .Select(x => (Track)x);                                     //selects actual tracks (Filter retuerns SpotifyBaseObjects)
                    break;
                case Album album:
                    result = (await dataLoader.GetAblumAllTracks(album.Id));
                    break;
                default:
                    throw new ArgumentException("The parameter provided has not a valid Type", "spi");
            }

            return result.Where(x => x?.Available_Markets != null && x.Available_Markets.Contains(user.Country)).ToList();
        }

        public static void MergeSort<T>(T[] input, int low, int high, Func<T, T, int> compare)
        {
            if (low < high)
            {
                int middle = (low / 2) + (high / 2);
                MergeSort(input, low, middle, compare);
                MergeSort(input, middle + 1, high, compare);
                Merge(input, low, middle, high, compare);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="compare"></param>
        public static void MergeSort<T>(T[] input, Func<T,T,int> compare)
        {
            MergeSort(input, 0, input.Length - 1, compare);
        }

        private static void Merge<T>(T[] input, int low, int middle, int high, Func<T, T, int> compare)
        {

            int left = low;
            int right = middle + 1;
            T[] tmp = new T[(high - low) + 1];
            int tmpIndex = 0;

            while ((left <= middle) && (right <= high))
            {
                if (compare(input[left],  input[right]) < 0)
                {
                    tmp[tmpIndex] = input[left];
                    left = left + 1;
                }
                else
                {
                    tmp[tmpIndex] = input[right];
                    right = right + 1;
                }
                tmpIndex = tmpIndex + 1;
            }

            if (left <= middle)
            {
                while (left <= middle)
                {
                    tmp[tmpIndex] = input[left];
                    left = left + 1;
                    tmpIndex = tmpIndex + 1;
                }
            }

            if (right <= high)
            {
                while (right <= high)
                {
                    tmp[tmpIndex] = input[right];
                    right = right + 1;
                    tmpIndex = tmpIndex + 1;
                }
            }

            for (int i = 0; i < tmp.Length; i++)
            {
                input[low + i] = tmp[i];
            }

        }
    }
}
