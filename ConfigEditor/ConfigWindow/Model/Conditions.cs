using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ConfigWindow.VM;

namespace ConfigWindow
{
    public class ReplaceContion : ConditionBase
    {
        public override bool CanExec { get; set; }

        public ReplaceContion()
        {
            Container = MainViewModel.Instance;
        }

        #region oldStr
        public string oldStr
        {
            get { return (string)GetValue(oldStrProperty); }
            set { SetValue(oldStrProperty, value); }
        }

        public static readonly DependencyProperty oldStrProperty =
            DependencyProperty.Register("oldStr", typeof(string), typeof(ReplaceContion), new PropertyMetadata("", (sender, e) =>
            {
                var vm = sender as ReplaceContion;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region newStr
        public string newStr
        {
            get { return (string)GetValue(newStrProperty); }
            set { SetValue(newStrProperty, value); }
        }

        public static readonly DependencyProperty newStrProperty =
            DependencyProperty.Register("newStr", typeof(string), typeof(ReplaceContion), new PropertyMetadata("", (sender, e) =>
            {
                var vm = sender as ReplaceContion;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region UseRegix
        public bool UseRegix
        {
            get { return (bool)GetValue(UseRegixProperty); }
            set { SetValue(UseRegixProperty, value); }
        }
        public static readonly DependencyProperty UseRegixProperty =
            DependencyProperty.Register("UseRegix", typeof(bool), typeof(ReplaceContion), new PropertyMetadata(false, (sender, e) =>
            {
                var vm = sender as ReplaceContion;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        public override void Exec()
        {
            CanExec = !string.IsNullOrEmpty(oldStr);
            Container.ExecUpdate();
        }
    }

    public class InsertCondition : ConditionBase
    {
        public override bool CanExec { get; set; }
        public InsertCondition()
        {
            Container = MainViewModel.Instance;
        }
        #region position
        public int position
        {
            get { return (int)GetValue(positionProperty); }
            set { SetValue(positionProperty, value); }
        }
        public static readonly DependencyProperty positionProperty =
            DependencyProperty.Register("position", typeof(int), typeof(InsertCondition), new PropertyMetadata(0, (sender, e) =>
            {
                var vm = sender as InsertCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region newStr
        public string newStr
        {
            get { return (string)GetValue(newStrProperty); }
            set { SetValue(newStrProperty, value); }
        }
        public static readonly DependencyProperty newStrProperty =
            DependencyProperty.Register("newStr", typeof(string), typeof(InsertCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as InsertCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region PreFix
        public string PreFix
        {
            get { return (string)GetValue(PreFixProperty); }
            set { SetValue(PreFixProperty, value); }
        }
        public static readonly DependencyProperty PreFixProperty =
            DependencyProperty.Register("PreFix", typeof(string), typeof(InsertCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as InsertCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region Suffix
        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }
        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register("Suffix", typeof(string), typeof(InsertCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as InsertCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        public override void Exec()
        {
            this.CanExec = !string.IsNullOrEmpty(newStr) || !string.IsNullOrEmpty(PreFix) || !string.IsNullOrEmpty(Suffix);
            Container.ExecUpdate();
        }
    }

    public class RemoveStrCondition : ConditionBase
    {
        public override bool CanExec { get; set; }

        public RemoveStrCondition()
        {

            Container = MainViewModel.Instance;
        }

        #region First
        public int First
        {
            get { return (int)GetValue(FirstProperty); }
            set { SetValue(FirstProperty, value); }
        }
        public static readonly DependencyProperty FirstProperty =
            DependencyProperty.Register("First", typeof(int), typeof(RemoveStrCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as RemoveStrCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region Last
        public int Last
        {
            get { return (int)GetValue(LastProperty); }
            set { SetValue(LastProperty, value); }
        }
        public static readonly DependencyProperty LastProperty =
            DependencyProperty.Register("Last", typeof(int), typeof(RemoveStrCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as RemoveStrCondition;
                if (vm == null) return;
                vm.Exec();
            }));
        #endregion

        #region StartIndex
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(RemoveStrCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as RemoveStrCondition;
                if (vm == null) return;
                vm.EndIndex = Math.Max(vm.StartIndex, vm.EndIndex);
                vm.Exec();
            }));
        #endregion

        #region EndIndex
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }
        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(RemoveStrCondition), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as RemoveStrCondition;
                if (vm == null) return;
                vm.EndIndex = Math.Max(vm.StartIndex, vm.EndIndex);
                vm.Exec();

            }));
        #endregion

        public override void Exec()
        {
            this.CanExec = First != 0 || Last != 0 || this.EndIndex != 0;
            Container.ExecUpdate();
        }
    }

    public class ConditionBase : DependencyObject
    {
        public MainViewModel Container;
        public virtual bool CanExec { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual void Exec() { }
    }
}
