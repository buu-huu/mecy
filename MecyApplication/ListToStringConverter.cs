using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MecyApplication
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<double> list = (List<double>)value;
            string result = "";

            foreach(double item in list)
            {
                result += item.ToString();
                result += " ";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
