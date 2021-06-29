using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MecyApplication
{
    /// <summary>
    /// Converter for MainWindow
    /// </summary>
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a list to a string. The elements are separated with a space.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture info</param>
        /// <returns>Converted element</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<double> list = (List<double>)value;
            string result = "";

            for (int i = 0; i < (list.Count - 1); i++)
            {
                result += list[i];
                result += "° | ";
            }
            result += list[list.Count - 1];
            result += "°";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
