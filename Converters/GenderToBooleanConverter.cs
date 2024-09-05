using Chat.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Converters
{
    public class GenderToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var gender = (Gender)value;
           var targetGender=(string)parameter;
            switch (targetGender)
            {
                case "Male":
                    return gender==Gender.Male;
                case "Female":
                    return gender==Gender.Female;
                default:
                    throw new ArgumentException($"Invalid parameter '{targetGender}'");
                   
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isChecked=(bool)value;
            var targetGender = (string)parameter;
            switch (targetGender)
            {
                case "Male":
                    return isChecked?Gender.Male:Gender.Female;
                case "Female":
                    return !isChecked?Gender.Male:Gender.Female;
                default:
                    throw new ArgumentException($"Invalid parameter '{targetGender}'");
            }
            throw new NotImplementedException();
        }
    }
}
