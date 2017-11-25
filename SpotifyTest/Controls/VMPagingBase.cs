using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.Controls
{
    public abstract class VMPagingBase
    {
        public abstract void NextPage();
        public abstract void PreviousPage();

        public abstract int CurrentPageNumber { get; }
    }
}
