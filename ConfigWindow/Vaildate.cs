using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace ConfigWindow
{
    public class DataColumn2Header : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var column = value as DataGridColumn;
            if (column == null) return null;
            return column.Header;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var column = value as DataGridColumn;
            if (column == null) return null;
            return column.Header;
        }
    }
}
