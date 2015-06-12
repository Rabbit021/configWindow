using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using ConfigFileAlter;
using System.Data;
using Microsoft.Expression.Interactivity.Core;
using StringOperation;
using System.ComponentModel;
using Microsoft.Win32;

namespace ConfigWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded -= MainWindow_Loaded;
            this.Loaded += MainWindow_Loaded;
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = MainViewModel.Instance;
            MainViewModel.Instance.Load();
        }
    }

    public class MainViewModel : DependencyObject
    {
        #region Instance
        private static MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance { get { return _instance; } }
        private MainViewModel()
        {
            RegistyCommand();
        }
        #endregion

        #region Table
        public DataTable Table
        {
            get { return (DataTable)GetValue(TableProperty); }
            set { SetValue(TableProperty, value); }
        }
        public static readonly DependencyProperty TableProperty =
            DependencyProperty.Register("Table", typeof(DataTable), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region TestStr
        public string TestStr
        {
            get { return (string)GetValue(TestStrProperty); }
            set { SetValue(TestStrProperty, value); }
        }
        public static readonly DependencyProperty TestStrProperty =
            DependencyProperty.Register("TestStr", typeof(string), typeof(MainViewModel), new PropertyMetadata("", (sender, e) => { }));
        #endregion

        #region CurrentReplace
        public ReplaceContion CurrentReplace
        {
            get { return (ReplaceContion)GetValue(CurrentReplaceProperty); }
            set { SetValue(CurrentReplaceProperty, value); }
        }
        public static readonly DependencyProperty CurrentReplaceProperty =
            DependencyProperty.Register("CurrentReplace", typeof(ReplaceContion), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region CurrentInsert
        public InsertCondition CurrentInsert
        {
            get { return (InsertCondition)GetValue(CurrentInsertProperty); }
            set { SetValue(CurrentInsertProperty, value); }
        }
        public static readonly DependencyProperty CurrentInsertProperty =
            DependencyProperty.Register("CurrentInsert", typeof(InsertCondition), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region CurrentSubStr
        public RemoveStrCondition CurrentSubStr
        {
            get { return (RemoveStrCondition)GetValue(CurrentSubStrProperty); }
            set { SetValue(CurrentSubStrProperty, value); }
        }
        public static readonly DependencyProperty CurrentSubStrProperty =
            DependencyProperty.Register("CurrentSubStr", typeof(RemoveStrCondition), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region OldStr
        public string OldStr
        {
            get { return (string)GetValue(OldStrProperty); }
            set { SetValue(OldStrProperty, value); }
        }
        public static readonly DependencyProperty OldStrProperty =
            DependencyProperty.Register("OldStr", typeof(string), typeof(MainViewModel), new PropertyMetadata("", (sender, e) =>
            {
                var vm = sender as MainViewModel;
                if (vm == null) return;
                vm.TestStr = vm.OldStr;
            }));
        #endregion

        #region SelectedItems
        public ObservableCollection<DataRow> SelectedItems
        {
            get { return (ObservableCollection<DataRow>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<DataRow>), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region NodePath
        public string NodePath
        {
            get { return (string)GetValue(NodePathProperty); }
            set { SetValue(NodePathProperty, value); }
        }
        public static readonly DependencyProperty NodePathProperty =
            DependencyProperty.Register("NodePath", typeof(string), typeof(MainViewModel), new PropertyMetadata("Root/NodeList", (sender, e) =>
            {
                var vm = sender as MainViewModel;
                if (vm == null) return;
                vm.InitTable();
            }));
        #endregion

        #region FilePath
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(MainViewModel), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as MainViewModel;
                if (vm == null) return;
                if (vm.Table != null)
                    vm.SaveFile(e.OldValue.ToString(), vm.Table);
                vm.InitTable();
            }));
        #endregion

        #region FileList
        public ObservableCollection<string> FileList
        {
            get { return (ObservableCollection<string>)GetValue(FileListProperty); }
            set { SetValue(FileListProperty, value); }
        }
        public static readonly DependencyProperty FileListProperty =
            DependencyProperty.Register("FileList", typeof(ObservableCollection<string>), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region AttrList
        public ObservableCollection<string> AttrList
        {
            get { return (ObservableCollection<string>)GetValue(AttrListProperty); }
            set { SetValue(AttrListProperty, value); }
        }
        public static readonly DependencyProperty AttrListProperty =
            DependencyProperty.Register("AttrList", typeof(ObservableCollection<string>), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        #region XmlList
        public List<XMLArch> XmlList
        {
            get { return (List<XMLArch>)GetValue(XmlListProperty); }
            set { SetValue(XmlListProperty, value); }
        }
        public static readonly DependencyProperty XmlListProperty =
            DependencyProperty.Register("XmlList", typeof(List<XMLArch>), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        public DataTable OriginValueTable { get; set; }
        public List<DataRow> OriginSeletedItems { get; set; }
        public List<string> PropertyList { get; set; }

        private void RegistyCommand()
        {
            this.SubStrCommand = new ActionCommand(SubStrAction);
            this.InsertStrCommand = new ActionCommand(InsertStrAction);
            this.ReplaceStrCommand = new ActionCommand(ReplaceStrAction);
            this.ResetCommand = new ActionCommand(Reset);
            this.RevertCommand = new ActionCommand(Revert);
            this.SelectionCommand = new ActionCommand(SelectIemAction);
            this.ApplyCommand = new ActionCommand(ApplyAction);

            this.OpenFileCommand = new ActionCommand(OpenFileAction);
            this.SaveFileCommand = new ActionCommand(SaveFileAction);
            this.GirdUpdateCommand = new ActionCommand(GirdUpdateAction);
            this.CheckColumsCommand = new ActionCommand(CheckColumsAction);
            this.ChangeItemCommand = new ActionCommand(ChangeItemAction);
        }

        public void Load()
        {
            InitCondition();
            RegistyCommand();
            this.PropertyList = new List<string>();
            this.FileList = new ObservableCollection<string>();
            this.XmlList = new List<XMLArch>();
        }

        public void InitTable()
        {
            PraseXML.Instance.StartPraseXML(FilePath, NodePath);
            Table = PraseXML.Instance.table;
            this.XmlList = PraseXML.Instance.XMLTree;
            if (Table != null)
                OriginValueTable = Table.Copy();

            var list = new ObservableCollection<string>();
            for (int i = 0; i < PraseXML.Instance.Attrs.Count; i++)
                list.Add(PraseXML.Instance.Attrs[i]);
            this.AttrList = list;

            this.SelectedItems = new ObservableCollection<DataRow>();
            this.OriginSeletedItems = new List<DataRow>();
        }

        private void InitCondition()
        {
            this.CurrentReplace = new ReplaceContion();
            this.CurrentSubStr = new RemoveStrCondition();
            this.CurrentInsert = new InsertCondition();
        }
        public ICommand SubStrCommand { get; set; }
        private void SubStrAction(object obj)
        {
        }

        public ICommand ReplaceStrCommand { get; set; }
        private void ReplaceStrAction(object obj)
        {
        }

        public ICommand InsertStrCommand { get; set; }
        private void InsertStrAction(object obj)
        {
        }

        public ICommand ResetCommand { get; set; }
        private void Reset(object obj)
        {
            InitCondition();
        }

        public ICommand RevertCommand { get; set; }
        private void Revert(object obj)
        {
            InitTable();
        }

        public ICommand SelectionCommand { get; set; }
        private void SelectIemAction(object obj)
        {
            try
            {
                var selects = (System.Collections.IList)obj;
                if (selects == null) return;
                this.SelectedItems.Clear();
                this.OriginSeletedItems.Clear();
                foreach (var item in selects)
                {
                    var rowView = item as DataRowView;
                    if (rowView == null) continue;
                    this.SelectedItems.Add(rowView.Row);
                    var index = rowView.Row[Constants.IndexName];
                    string filter = string.Format(@"{0}='{1}'", Constants.IndexName, index);
                    var origonRows = OriginValueTable.Select(filter);
                    this.OriginSeletedItems.AddRange(origonRows);
                }
            }
            catch (System.Exception ex)
            {

            }

        }

        public ICommand ApplyCommand { get; set; }
        private void ApplyAction(object obj)
        {
            ExecUpdate();
        }

        public ICommand OpenFileCommand { get; set; }
        private void OpenFileAction(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Xml files (*.xml)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (this.FileList.Contains(filename)) continue;
                    this.FileList.Add(filename);
                }
            }
        }

        public ICommand SaveFileCommand { get; set; }
        private void SaveFileAction(object obj)
        {
            var res = SaveFile(this.FilePath, Table);
            if (res) InitTable();
        }
        public bool SaveFile(string file, DataTable datas)
        {
            //return PraseXML.Instance.SaveFile(file, datas);
            return false;
        }

        public ICommand GirdUpdateCommand { get; set; }
        private void GirdUpdateAction(object obj)
        {
            try
            {
                var cell = (DataGridCellInfo)obj;
                if (cell == null) return;
                var rowView = (DataRowView)cell.Item;

                var row = rowView.Row;
                var index = row[Constants.IndexName];
                string filter = string.Format(@"{0}='{1}'", Constants.IndexName, index);
                var origonRows = OriginValueTable.Select(filter);
                var newValue = Table.Select(filter);
                if (origonRows.Length >= 0)
                {
                    string column = cell.Column.Header.ToString();
                    origonRows[0][column] = newValue[0][column];
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        public ICommand CheckColumsCommand { get; set; }
        private void CheckColumsAction(object obj)
        {
            var propertyies = (System.Collections.IList)obj;
            if (propertyies == null) return;
            this.PropertyList = propertyies.Cast<string>().ToList();
        }

        public ICommand ChangeItemCommand { get; set; }
        private void ChangeItemAction(object obj)
        {
            var arch = obj as XMLArch;
            if (arch == null) return;
            PraseXML.Instance.SaveFile(this.FilePath);

            this.Table = arch.ChildAttrs;
            this.OriginValueTable = arch.ChildAttrs.Copy();
        }

        // 执行更新
        public void ExecUpdate()
        {
            //TODO 暂时取消
            if (this.SelectedItems == null || PropertyList.Count == 0) return;

            foreach (var item in this.SelectedItems)
            {
                var currentIndex = item[Constants.IndexName];
                var row = this.OriginSeletedItems.FirstOrDefault(x => string.Equals(x[Constants.IndexName], currentIndex));
                if (row == null) continue;

                foreach (var prop in PropertyList)
                {

                    string str = row[prop].ToString();

                    //Exec Replace 
                    if (CurrentReplace.CanExec)
                        str = ExecReplace(str);

                    //Exec Remmove
                    if (CurrentSubStr.CanExec)
                        str = ExecRemove(str);

                    //Exec Insert
                    if (CurrentInsert.CanExec)
                        str = ExecInsert(str);
                    item[prop] = str;
                }

            }
        }
        public string ExecReplace(string originStr)
        {
            originStr = originStr.ReplaceExt(CurrentReplace.oldStr, CurrentReplace.newStr, CurrentReplace.UseRegix);
            return originStr;
        }

        public string ExecRemove(string originStr)
        {
            originStr = originStr.RemoveSide(CurrentSubStr.First, true);
            originStr = originStr.RemoveSide(CurrentSubStr.Last, false);
            originStr = originStr.RemoveFromTo(CurrentSubStr.StartIndex, CurrentSubStr.EndIndex);
            return originStr;
        }

        public string ExecInsert(string originStr)
        {
            originStr = originStr.InsertPreFix(CurrentInsert.PreFix);
            originStr = originStr.InsertStuffix(CurrentInsert.Suffix);
            originStr = originStr.InsertAt(CurrentInsert.position, CurrentInsert.newStr);
            return originStr;
        }
    }

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
