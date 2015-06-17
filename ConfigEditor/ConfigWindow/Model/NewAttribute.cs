using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ConfigWindow.Model
{
    public class NewAttribute : DependencyObject
    {
        #region AttrName
        public string AttrName
        {
            get { return (string)GetValue(AttrNameProperty); }
            set { SetValue(AttrNameProperty, value); }
        }
        public static readonly DependencyProperty AttrNameProperty =
            DependencyProperty.Register("AttrName", typeof(string), typeof(NewAttribute), new PropertyMetadata("", (sender, e) => { }));
        #endregion

        #region AttrValue
        public string AttrValue
        {
            get { return (string)GetValue(AttrValueProperty); }
            set { SetValue(AttrValueProperty, value); }
        }
        public static readonly DependencyProperty AttrValueProperty =
            DependencyProperty.Register("AttrValue", typeof(string), typeof(NewAttribute), new PropertyMetadata("", (sender, e) => { }));
        #endregion
    }
}
