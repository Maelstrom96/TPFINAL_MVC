using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrDragon.Models
{
    public class Modal
    {
        public String Title { get; set; }
        public String Message { get; set; }

        public Modal(String title, String message)
        {
            Title = title;
            Message = message;
        }

        public Modal()
        {

        }
    }
}
