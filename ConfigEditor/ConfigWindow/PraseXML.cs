using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data;
using StringOperation;
using System.IO;

namespace ConfigFileAlter
{
    public class PraseXML
    {
        #region Instance
        private static PraseXML _instance = new PraseXML();
        public static PraseXML Instance { get { return _instance; } }
        private PraseXML() { }
        #endregion
        private XmlDocument doc;
        private string filename;
        public string nodePath = "";
        public DataTable table;
        public List<string> Attrs = new List<string>();

        public DataTable ParseNodes(XmlElement root, List<string> attrs)
        {
            var dataTable = new DataTable();
            if (root == null) return dataTable;
            var NodeList = root.ChildNodes;
            foreach (var node in NodeList)
                ParseNode(node as XmlElement, dataTable, attrs);
            return dataTable;
        }
        public void ParseNode(XmlElement node, DataTable dataTable, List<string> attrList)
        {
            if (node == null) return;
            var row = dataTable.NewRow();
            AddColumn(Constants.NodeName, node.Name, row, dataTable);
            dataTable.Rows.Add(row);
            string result = string.Empty;
            var attrs = node.Attributes;
            if (attrs == null || attrs.Count == 0) return;
            foreach (XmlAttribute attr in attrs)
            {
                AddColumn(attr.Name, attr.Value, row, dataTable);
                if (attrList.Contains(attr.Name)) continue;
                attrList.Add(attr.Name);
            }
        }
        private void AddColumn(string name, string value, DataRow row, DataTable dataTable)
        {
            if (!dataTable.Columns.Contains(name))
                dataTable.Columns.Add(name);
            row[name] = value;
        }
        public bool SaveFile(string filename, DataTable datas)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            try
            {
                UpdateNode(doc.SelectSingleNode(nodePath) as XmlElement, datas);
                doc.Save(filename);
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存异常");
                return false;
            }
        }
        private void UpdateDocumnent(XmlNode doc, ObservableCollection<XMLArch> XMLTree)
        {
            foreach (var child in XMLTree)
            {
                UpdateArch(child);
                HelperIndex(child.Node, HelperIndexType.Remove);
                doc.AppendChild(child.Node);
                UpdateDocumnent(child.Node, child.ChildNode);
            }
        }
        private void UpdateArch(XMLArch child)
        {
            var datas = child.ChildAttrs;
            var attrList = child.Attrs;
            if (attrList == null) return;
            attrList.Sort();
            foreach (DataRow row in datas.Rows)
            {
                var nodeName = row[Constants.NodeName].ToString();
                if (string.IsNullOrEmpty(nodeName)) return;
                var index = row[Constants.IndexName];
                string findstr = string.Format("{0}[@{1}='{2}']", nodeName, Constants.IndexName, index);
                XmlElement xmlChild = child.Node.SelectSingleNode(findstr) as XmlElement;
                if (xmlChild == null) continue;
                foreach (var attr in attrList)
                {
                    if (string.IsNullOrEmpty(attr)) continue;
                    xmlChild.SetAttribute(attr, row[attr].ToString());
                }
            }
        }
        private void SaveNode(ObservableCollection<XMLArch> childNode, XmlElement root)
        {
            foreach (var item in childNode)
            {
                root.AppendChild(item.Node);
                UpdateNode(item.Node, item.ChildAttrs);
                SaveNode(item.ChildNode, item.Node);
            }
        }
        public void UpdateNode(XmlElement root, DataTable datas)
        {
            if (root == null || datas.Rows == null) return;
            //root.RemoveAll();
            foreach (DataRow row in datas.Rows)
            {
                var nodeName = row[Constants.NodeName].ToString();
                if (string.IsNullOrEmpty(nodeName)) return;
                var child = doc.CreateElement(nodeName);
                root.AppendChild(child);
                foreach (var attr in Attrs)
                {
                    if (string.IsNullOrEmpty(attr)) continue;
                    child.SetAttribute(attr, row[attr].ToString());
                }
            }
        }
        private ObservableCollection<XMLArch> PraseFile(string fileName)
        {
            this.XMLTree = null;
            ObservableCollection<XMLArch> tree = new ObservableCollection<XMLArch>();
            try
            {
                HelperIndex(doc.DocumentElement, HelperIndexType.Add, 0);
                var arch = PraseRoot(doc.DocumentElement, 0);
                tree.Add(arch);
            }
            catch
            {

            }
            return tree;
        }
        private XMLArch PraseRoot(XmlElement root, int index)
        {
            XMLArch arch = new XMLArch();
            arch.Node = root;
            arch.DisplayName = string.Format("{0}-{1}", root.GetAttribute(Constants.IndexName), root.Name);
            arch.Attrs = new List<string>();
            arch.ChildAttrs = this.ParseNodes(root, arch.Attrs);
            var NodeList = root.ChildNodes;

            ObservableCollection<XMLArch> tree = new ObservableCollection<XMLArch>();
            int count = 0;
            foreach (XmlNode node in NodeList)
            {
                var element = node as XmlElement;
                if (element == null) continue;
                tree.Add(PraseRoot(element, count++));
            }
            arch.ChildNode = tree;
            return arch;
        }
        private void HelperIndex(XmlElement root, HelperIndexType addOrRemove, int index = 1)
        {
            if (root == null) return;
            if (addOrRemove == HelperIndexType.Add)
            {
                XmlAttribute xmlID = doc.CreateAttribute(Constants.IndexName);
                xmlID.Value = index.ToString();
                root.Attributes.Prepend(xmlID);
            }
            else
                root.RemoveAttribute(Constants.IndexName);
            var NodeList = root.ChildNodes;
            int count = 1;
            foreach (var node in NodeList)
            {
                var element = node as XmlElement;
                if (element == null) continue;
                HelperIndex(element, HelperIndexType.Add, count++);
            }
        }

        #region Refrence By external
        public ObservableCollection<XMLArch> XMLTree;
        public void StartPraseXML(string filename, string nodePath = "")
        {
            try
            {
                this.filename = filename;
                doc = new XmlDocument();
                if (!File.Exists(this.filename)) return;
                doc.Load(this.filename);
                var rootElement = doc.DocumentElement;
                table = new DataTable();
                this.nodePath = nodePath;

                if (!string.IsNullOrEmpty(nodePath))
                    this.table = ParseNodes(doc.SelectSingleNode(nodePath) as XmlElement, this.Attrs);
                else
                    this.table = ParseNodes(rootElement, this.Attrs);

                this.XMLTree = PraseFile(filename);
            }
            catch (System.Exception ex)
            {

            }
        }
        public bool SaveFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            try
            {
                doc.RemoveAll();
                UpdateDocumnent(doc, XMLTree);
                doc.Save(filename);
                return true;
            }
            catch
            {
                MessageBox.Show("保存异常");
                return false;
            }
        }
        #endregion
    }

    public class XMLArch : DependencyObject
    {
        public XmlElement Node { get; set; }
        public string DisplayName { get; set; }
        public DataTable ChildAttrs { get; set; }
        public List<string> Attrs { get; set; }
        public ObservableCollection<XMLArch> ChildNode { get; set; }
    }

    public enum HelperIndexType : int
    {
        Add = 0,
        Remove = 1
    }
}
