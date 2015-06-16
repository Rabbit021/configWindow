using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace groupNodeList
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
                GroupNodeList(args[0]);
            else if (args.Length == 2)
                GroupNodeList(args[0], args[1]);
            else if (args.Length == 3)
                GroupNodeList(args[0], args[1], args[2]);
            else if (args.Length >= 4)
                GroupNodeList(args[0], args[1], args[2], args[3]);
        }

        private static void GroupNodeList(string filename, string xpath = "Root/NodeList", string childNode = "PageSubDataDicNode", string groupKey = "Expression")
        {
            try
            {
                PraseXML.Instance.InitXml(filename, xpath, childNode);
                var elements = PraseXML.Instance.ElementList;
                if (elements == null) return;

                var grouped = from element in elements
                              group element by element.GetAttribute(groupKey) into g
                              select g;
                PraseXML.Instance.SaveFile(filename, grouped);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
