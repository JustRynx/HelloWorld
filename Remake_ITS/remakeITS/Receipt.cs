using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remakeITS
{
    public class Receipt //- Must be public so that it is accessible to other class.
    {
        public string ProductName { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string SubTotal { get; set; }
        public int ProductID { get; set; }
    }
}
