using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp1
{
    public interface ILogixProject
    {
        void Close(bool IgnorePrompts, bool AcceptDefaultActions);



    }

    public interface ILogixApplication
    {
        ILogixProject FileOpen(string PathName, bool ShowDialog, bool UseAutoSave, bool AutoImportDB);
    }

    public interface IProgramFiles_Collection
    {

    }

    public interface IProgramFile
    {

    }

    public interface IDataFiles_Collection
    {

    }

    public interface IDataFile
    {

    }
    public interface Rung
    {

    }

}
