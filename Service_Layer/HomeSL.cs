using Data_Layer;
using Domain_Layer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class HomeSL
    {
        static HomeDL home = new HomeDL();

        public MatchesFoundDTO FindMatchesSL(string queryString)
        {
            return home.FindMatchesDL(queryString);
        }
    }
}
