using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MecyApplication
{
    [ValueConversion(typeof(int), typeof(string))]
    public class MesoCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mesoCount = (int)value;
            if (mesoCount == 0)
            {
                return "";
            }
            else
            {
                return mesoCount.ToString();
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
