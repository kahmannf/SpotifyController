using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI
{

    [Serializable]
    public class SpotifyLoadException : Exception
    {
        public SpotifyLoadException() { }
        public SpotifyLoadException(string message) : base(message) { }
        public SpotifyLoadException(string message, Exception inner) : base(message, inner) { }
        protected SpotifyLoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
