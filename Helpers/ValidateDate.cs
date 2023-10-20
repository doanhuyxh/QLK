using System.ComponentModel.DataAnnotations;

namespace AMS
{
    public class ValidateDate : RangeAttribute
    {
        public ValidateDate()
          : base(typeof(DateTime),
                  DateTime.Now.AddYears(-6).ToShortDateString(),
                  DateTime.Now.ToShortDateString())
        { }
    }

}
