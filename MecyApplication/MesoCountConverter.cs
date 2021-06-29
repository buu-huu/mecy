using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MecyApplication
{
    /// <summary>
    /// Converter for MainWindow
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class MesoCountConverter : IValueConverter
    {
        /// <summary>
        /// Converts a number of mesocyclones to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
