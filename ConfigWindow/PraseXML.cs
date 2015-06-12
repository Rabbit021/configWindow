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
        private int index = 4;
        public string nodePath = "";
        public DataTable table;
        public List<string> Attrs = new List<string>();
        public List<XMLArch> XMLTree = new List<XMLArch>();

        public void StartPraseXML(string filename, string nodePath = "")
        {
            try
            {
                doc = new XmlDocument();
                if (!File.Exists(filename)) return;
                doc.Load(filename);
                var rootElement = doc.DocumentElement;
                table = new DataTable();
                index = 0;
                this.nodePath = nodePath;
                Attrs.Clear();
                if (!string.IsNullOrEmpty(nodePath))
                    this.table = ParseNodes(doc.SelectSingleNode(nodePath) as XmlElement);
                else
                    this.table = ParseNodes(rootElement);

                this.XMLTree = PraseFile(filename);
            }
            catch (System.Exception ex)
            {

            }
        }

        public DataTable ParseNodes(XmlElement root)
        {
            var dataTable = new DataTable();
            if (root == null) return dataTable;
            var NodeList = root.ChildNodes;
            int index = 0;
            foreach (XmlElement node in NodeList)
                ParseNode(node, index++, dataTable);
            return dataTable;
        }

        public void ParseNode(XmlElement node, int index, DataTable dataTable)
        {
            var row = dataTable.NewRow();
            AddColumn(Constants.IndexName, index.ToString(), row, dataTable);
            AddColumn(Constants.NodeName, node.Name, row, dataTable);
            dataTable.Rows.Add(row);

            string result = string.Empty;
            var attrs = node.Attributes;
            if (attrs == null || attrs.Count == 0) return;
            foreach (XmlAttribute attr in attrs)
            {
                AddColumn(attr.Name, attr.Value, row, dataTable);
                if (this.Attrs.Contains(attr.Name)) continue;
                this.Attrs.Add(attr.Name);
            }
        }

        private void AddColumn(string name, string value, DataRow row, DataTable dataTable)
        {
            if (!dataTable.Columns.Contains(name))
                dataTable.Columns.Add(name);
            row[name] = value;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="datas"></param>
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
        public bool SaveFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            try
            {
                doc.RemoveAll();
                foreach (var item in XMLTree)
                {
                    doc.AppendChild(item.Node);
                    UpdateNode(item.Node, item.ChildAttrs);
                    SaveNode(item.ChildNode, item.Node);
                }
                doc.Save(filename);
                return true;
            }
            catch
            {
                MessageBox.Show("保存异常");
                return false;
            }
        }

        private void SaveNode(List<XMLArch> childNode, XmlElement root)
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
                WriteToAttr(root, row);
            }
        }

        private void WriteToAttr(XmlElement node, DataRow row)
        {
            var nodeName = row[Constants.NodeName].ToString();
            if (string.IsNullOrEmpty(nodeName)) return;
            var child = doc.CreateElement(nodeName);
            node.AppendChild(child);
            foreach (var attr in Attrs)
            {
                if (string.IsNullOrEmpty(attr)) continue;
                child.SetAttribute(attr, row[attr].ToString());
            }
        }

        private List<XMLArch> PraseFile(string fileName)
        {
            List<XMLArch> tree = new List<XMLArch>();
            try
            {
                var arch = PraseRoot(doc.DocumentElement);
                tree.Add(arch);
            }
            catch
            {

            }
            return tree;
        }

        private XMLArch PraseRoot(XmlElement root)
        {
            XMLArch arch = new XMLArch();
            arch.Node = root;
            arch.ChildAttrs = this.ParseNodes(root);

            var NodeList = root.ChildNodes;
            List<XMLArch> tree = new List<XMLArch>();
            foreach (XmlElement node in NodeList)
                tree.Add(PraseRoot(node));
            arch.ChildNode = tree;

            return arch;
        }
    }
    public class XMLArch : DependencyObject
    {
        public XmlElement Node { get; set; }
        public DataTable ChildAttrs { get; set; }
        public List<string> Attrs { get; set; }
        public List<XMLArch> ChildNode { get; set; }
    }
}
