using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI.Models.Catalog
{
    internal class BookCopyDisplay
    {
        public string CopyID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public string Edition { get; set; }
        public string Format { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }

        public BookCopyDisplay()
        {
            
        }
    }
}
