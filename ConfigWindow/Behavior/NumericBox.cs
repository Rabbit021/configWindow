using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ConfigWindow
{
    public class NumericBox : TextBox
    {
        #region Property

        #endregion

        public NumericBox()
        {
            var filter = new TextBoxFilterBahavior();
            Interaction.GetBehaviors(this).Add(filter);
            this.TextChanged += NumericBox_TextChanged;
        }

        private void NumericBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    public class TextBoxFilterBahavior : Behavior<TextBox>
    {
        #region Format
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(TextBoxFilterBahavior), new PropertyMetadata(""));




        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)(e.OriginalSource)).Text;
            var handled = !ValidateNum(text);
            e.Handled = handled;
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var handled = ValidateChar(e.Key);
            e.Handled = !handled;
        }

        private bool ValidateNum(string text)
        {
            var res = Regex.IsMatch(text, @"^-?[1-9]\d*$");
            return res;
        }
        private bool ValidateChar(Key inputKey)
        {
            bool shiftkey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool retval;
            if (inputKey >= Key.D0 && inputKey <= Key.D9 && !shiftkey)
            {
                retval = true;
            }
            else
            {
                retval = inputKey >= Key.NumPad0 && inputKey <= Key.NumPad9;
            }
            return retval;
        }
    }
}