using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
    class fileInfoParser
    {



        string debugString;

        //Used for printing useful info stored in "debugString", can be called in another class by calling this constructor
        public override string ToString()
        {
            return "Debugging info:\n\n" + debugString + "\n";
        }
    }
}
