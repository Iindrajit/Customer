using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.CustomAttributes
{
    public class CustomDateRangeAttribute : RangeAttribute
    {
        public CustomDateRangeAttribute() 
            : base(typeof(DateTime),
                  DateTime.Now.AddYears(-150).ToShortDateString(),
                  DateTime.Now.ToShortDateString())
        { }
    }
}
