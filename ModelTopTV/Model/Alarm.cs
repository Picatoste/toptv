using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopTV.Model
{
    public class AlarmModel
    {
        public string Nombre
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public int MinutesBefore
        {
            get;
            set;
        }
    }

}
