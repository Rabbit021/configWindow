using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace groupNodeList
{
    public class PraseXML
    {
        #region Instance
        private static PraseXML _instance = new PraseXML();
        public static PraseXML Instance { get { return _instance; } }
        private PraseXML() { }
        #endregion

        public List<XmlElement> ElementList { get; set; }
        public XmlDocument Doc { get; set; }
        private string parentNodePath { get; set; }

        public bool InitXml(string filename, string xpath, string childNode)
        {
            this.parentNodePath = xpath;
            return LoadXml(filename) && PrasePath(xpath, childNode);
        }

        public bool LoadXml(string filename)
        {
            try
            {
                Doc = new XmlDocument();
                if (!File.Exists(filename))
                {
                    Console.WriteLine("{0} not exist", filename);
                    return false;
                }
                Doc.Load(filename);
                this.ElementList = new List<XmlElement>();
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool PrasePath(string xpath, string childNode)
        {
            try
            {
                var path = string.Join("/", xpath, childNode);
                var nodes = Doc.SelectNodes(path);
                foreach (var node in nodes)
                {
                    var element = node as XmlElement;
                    if (element == null) continue;
                    this.ElementList.Add(element);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool SaveFile(string filename, IEnumerable<IGrouping<string, XmlElement>> group, string groupKey)
        {
            if (group == null) return false;
            try
            {
                var path = Path.GetDirectoryName(filename);
                var name = Path.GetFileName(filename);
                var newPath = Path.Combine(path, "Grouped");
                if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                string newfilname = Path.Combine(newPath, name);
                if (File.Exists(newfilname)) File.Delete(filename);
                var node = Doc.SelectSingleNode(parentNodePath);
                node.RemoveAll();
                foreach (var item in group)
                {
                    var newParent = item.First().Clone() as XmlElement;
                    if (newParent == null) continue;
                    var groupValue = newParent.GetAttribute(groupKey);
                    if (string.IsNullOrEmpty(groupValue))
                    {
                        // 此处不分组写入文件
                        foreach (var child in item)
                            node.AppendChild(child.Clone());
                    }
                    else
                    {
                        foreach (var child in item)
                            newParent.AppendChild(child.Clone());
                        node.AppendChild(newParent);
                    }
                }
                Doc.Save(newfilname);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
