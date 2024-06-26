﻿//  ---------------------------------------------------------------------------------
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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

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
            if (value is float decimalValue)
            {
                return (double)decimalValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                return (float)doubleValue;
            }

            return value;
        }
    }

    public class DecimalToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                return (float)decimalValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is float floatValue)
            {
                return (decimal)floatValue;
            }

            return value;
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
            DateTimeOffset offset = new((DateTime)value);
            return offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset offset)
            {
                return offset.DateTime;
            }
            else
            {
                throw new ArgumentException("ExceptionDateTimeToDateTimeOffsetConverterValueCannotBeNull"/*.GetLocalized()*/);
            }
        }
    }

    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                return date.ToString("M/d/yyyy");
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
            if (value.GetType() == typeof(float))
            {
                return ((float)value).ToString("N2");
            }
            else if (value.GetType() == typeof(double))
            {
                return ((double)value).ToString("N2");
            }
            else if(value.GetType() == typeof(decimal))
            {
                return ((decimal)value).ToString("N2");
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }


    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((float)value).ToString("C");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public class FloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.GetType() != typeof(string))
            {
                throw new ArgumentException();
            }
            else
            {
                return float.Parse(value as string);
            }
        }

    }

    public class NullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool?) value == true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? (bool?)true : (bool?)null;
        }
    }
    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return double.Parse(value.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)Math.Round((double)value,0);
        }
    }

    public class NullIntToDoubleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? double.Parse(value.ToString()) : null;
        }
        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null ? (int)Math.Round((double)value, 0) : null;
        }
    }

    public class AllocatedToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "Allocated" : "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().Equals("Allocated");
        }
    }

    public class NullIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? (string)null : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int temp;
            if (string.IsNullOrEmpty((string)value) || !int.TryParse((string)value, out temp)) return null;
            else return temp;
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            if (enumObj == null)
            {
                return string.Empty;
            }
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Enum myEnum = (Enum)value;
            if (myEnum == null)
            {
                return null;
            }
            string description = GetEnumDescription(myEnum);
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }
            return myEnum.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return string.Empty;
        }
    }
}
