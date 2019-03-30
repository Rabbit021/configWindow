using System.Collections.ObjectModel;
using ConfigFileAlter;

namespace ConfigWindow
{
    public interface IParseFormat
    {
        ObservableCollection<XMLArch> XMLTree { get;set; }
        void PhraseFile(string filename);
        void SaveFile(string filename);
    }
}