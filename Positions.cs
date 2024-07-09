using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class Positions
    {
        public static int TPerpendicularOffSet = 0;

        public static int XAlignLocation = 317955;

        public static IDictionary<int, int> YAlignLocations = new Dictionary<int, int>()
        {
                { 150, 2460 },
                { 200, 27552 },
                { 300, 77762 }
        };

        public static IDictionary<string, int> Center = new Dictionary<string, int>()
        {
                { "x", 159480 },
                { "y", 155083 },
                { "z" , 481762 },
                {"t", 0 }
        };

        public static IDictionary<string, int> PosLimit = new Dictionary<string, int>()
        {
                { "x", 318961 },
                { "y", 310167 },
                { "z" , 963524 },
                {"t", 12000 }
        };

        public static IDictionary<string, int> NegLimit = new Dictionary<string, int>()
        {
                { "x", 0 },
                { "y", 0 },
                { "z" , 0 },
                {"t", -12000 }
        };


        


        

    }
}
