using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI.Models
{
    internal class BookCopy
    {
        public string CopyID { get; set; }
        public string EditionID { get; set; }
        public string StatusID { get; set; }
        public string LocationID { get; set; }

        public BookCopy() 
        {

        }

        public BookCopy(string copyid, string editionid, string statusid, string locationid)
        {
            CopyID = copyid;
            EditionID = editionid;
            StatusID = statusid;
            LocationID = locationid;
        }
    }
}
