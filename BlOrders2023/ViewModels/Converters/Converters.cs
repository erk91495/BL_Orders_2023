//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using BlOrders2023.Models.Enums;
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace BlOrders2023.ViewModels.Converters
{
    /// <summary>
    /// Provides static methods for use in x:Bind function binding to convert bound values to the required value.
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Returns the reverse of the provided value.
        /// </summary>
        public static bool Not(bool value) => !value;

        /// <summary>
        /// Returns true if the specified value is not null; otherwise, returns false.
        /// </summary>
        public static bool IsNotNull(object value) => value != null;

        /// <summary>
        /// Returns Visibility.Collapsed if the specified value is true; otherwise, returns Visibility.Visible.
        /// </summary>
        public static Visibility CollapsedIf(bool value) =>
            value ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Returns Visibility.Collapsed if the specified value is null; otherwise, returns Visibility.Visible.
        /// </summary>
        public static Visibility CollapsedIfNull(object value) =>
            value == null ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Returns Visibility.Collapsed if the specified string is null or empty; otherwise, returns Visibility.Visible.
        /// </summary>s
        public static Visibility CollapsedIfNullOrEmpty(string? value) =>
            string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;

    }

    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is decimal @decimal)
            {
                return decimal.ToDouble(@decimal);
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToDecimal(value);
        }
    }

    public class FloatToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is float f)
            {
                return (double)f;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToSingle(value);
        }
    }

    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(value.GetType(), value))
                {
                    throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum"/*.GetLocalized()*/);
                }

                var enumValue = Enum.Parse(value.GetType(), enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName"/*.GetLocalized()*/);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(targetType, enumString);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName"/*.GetLocalized()*/);
        }
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(value.GetType(), value))
                {
                    throw new ArgumentException("ExceptionEnumToVisibilityConverterValueMustBeAnEnum"/*.GetLocalized()*/);
                }

                var enumValue = Enum.Parse(value.GetType(), enumString);

                return enumValue.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
            }

            throw new ArgumentException("ExceptionEnumToVisibilityConverterValueMustBeAnEnum"/*.GetLocalized()*/);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(targetType, enumString);
            }

            throw new ArgumentException("ExceptionEnumToVisibilityConverterValueMustBeAnEnum"/*.GetLocalized()*/);
        }
    }

    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset offset = new DateTimeOffset((DateTime)value);
            return offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((DateTimeOffset)value).DateTime;
        }
    }

    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                return date.ToString("MM/dd/yyyy");
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                return date.ToString("hh:mm tt");
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class WeightFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((float)value).ToString("N2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
