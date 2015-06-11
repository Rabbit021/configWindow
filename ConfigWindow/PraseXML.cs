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

        public void StartPraseXML(string filename, string nodePath = "")
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
                ParseNodes(doc.SelectSingleNode(nodePath) as XmlElement);
            else
                ParseNodes(rootElement);
        }

        public void ParseNodes(XmlElement root)
        {
            if (root == null) return;
            var NodeList = root.ChildNodes;
            int index = 0;
            foreach (XmlElement node in NodeList)
                ParseNode(node, index++);
        }

        public void ParseNode(XmlElement node, int index)
        {
            string result = string.Empty;
            var attrs = node.Attributes;
            if (attrs == null || attrs.Count == 0) return;
            var row = this.table.NewRow();
            AddColumn(Constants.IndexName, index.ToString(), row);
            AddColumn(Constants.NodeName, node.Name, row);
            foreach (XmlAttribute attr in attrs)
            {
                AddColumn(attr.Name, attr.Value, row);
                if (this.Attrs.Contains(attr.Name)) continue;
                    this.Attrs.Add(attr.Name);
            }
            this.table.Rows.Add(row);
        }

        private void AddColumn(string name, string value, DataRow row)
        {
            if (!this.table.Columns.Contains(name))
                this.table.Columns.Add(name);
            row[name] = value;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="datas"></param>
        public bool SaveFile(string filename, DataTable datas)
        {
            try
            {
                UpdateNode(doc.SelectSingleNode(nodePath) as XmlElement, datas);
                doc.Save(filename);
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存失败");
                return false;
            }
        }
        public void UpdateNode(XmlElement root, DataTable datas)
        {
            if (root == null || datas.Rows == null) return;
            root.RemoveAll();
            foreach (DataRow row in datas.Rows)
                WriteToAttr(root, row);
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
    }
}
