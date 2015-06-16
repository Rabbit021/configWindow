using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StringOperation;
using System.Collections.ObjectModel;
using StringOperation;
using ConfigFileAlter;
using Microsoft.Expression.Interactivity.Core;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ConfigWindow.VM
{
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

        #region CurrentTable
        public DataTable CurrentTable
        {
            get { return (DataTable)GetValue(CurrentTableProperty); }
            set { SetValue(CurrentTableProperty, value); }
        }
        public static readonly DependencyProperty CurrentTableProperty =
            DependencyProperty.Register("CurrentTable", typeof(DataTable), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
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
                if (vm.CurrentTable != null)
                    vm.SaveFile(e.OldValue.ToString());
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
        public ObservableCollection<XMLArch> XmlList
        {
            get { return (ObservableCollection<XMLArch>)GetValue(XmlListProperty); }
            set { SetValue(XmlListProperty, value); }
        }
        public static readonly DependencyProperty XmlListProperty =
            DependencyProperty.Register("XmlList", typeof(ObservableCollection<XMLArch>), typeof(MainViewModel), new PropertyMetadata((sender, e) => { }));
        #endregion

        public DataTable OriginValueTable { get; set; }
        public List<DataRow> OriginSeletedItems { get; set; }
        public List<string> PropertyList { get; set; }
        public List<string> SortOrGroupList { get; set; }
        public XMLArch Arch { get; set; }

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
            this.SortOrGroupCheckCommand = new ActionCommand(SortOrGroupCheckAction);
        }

        public void Load()
        {
            InitCondition();
            RegistyCommand();
            this.PropertyList = new List<string>();
            this.FileList = new ObservableCollection<string>();
            this.XmlList = new ObservableCollection<XMLArch>();
        }

        public void InitTable()
        {
            PraseXML.Instance.StartPraseXML(FilePath, NodePath);
            this.XmlList = PraseXML.Instance.XMLTree;
            this.SelectedItems = new ObservableCollection<DataRow>();
            this.OriginSeletedItems = new List<DataRow>();
            if (CurrentTable != null)
                OriginValueTable = CurrentTable.Copy();

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
                this.FilePath = openFileDialog.FileNames.First();
            }
        }

        public ICommand SaveFileCommand { get; set; }
        private void SaveFileAction(object obj)
        {
            var res = SaveFile(this.FilePath);
            if (CurrentTable != null)
                OriginValueTable = CurrentTable.Copy();
        }
        public bool SaveFile(string fileName)
        {
            return PraseXML.Instance.SaveFile(fileName);
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
                var newValue = CurrentTable.Select(filter);
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
        public ICommand SortOrGroupCheckCommand { get; set; }
        private void SortOrGroupCheckAction(object obj)
        {
            var propertyies = (System.Collections.IList)obj;
            if (propertyies == null) return;
            this.SortOrGroupList = propertyies.Cast<string>().ToList();
            this.CurrentTable = this.CurrentTable.Select("", "SubDataDisplayName asc").CopyToDataTable();
        }

        public ICommand ChangeItemCommand { get; set; }
        private void ChangeItemAction(object obj)
        {
            Arch = obj as XMLArch;
            if (Arch == null) return;
            this.CurrentTable = Arch.ChildAttrs;
            var list = new ObservableCollection<string>();
            for (int i = 0; i < Arch.Attrs.Count; i++)
                list.Add(Arch.Attrs[i]);
            this.AttrList = list;
            this.OriginValueTable = Arch.ChildAttrs.Copy();
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

}
