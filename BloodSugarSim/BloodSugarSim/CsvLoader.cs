using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodSugarSim
{
    
    public interface ILoadFile
    {
        void LoadFile();
    }

    public class CsvLoader
    {
        private ILoadFile loadFile;

        private CsvLoader() { }
        public CsvLoader(ILoadFile loadFileType)
        {
            //if (CsvPathName == string.Empty || CsvPathName == null)
            //    throw new ApplicationException("Invalid string passed to CsvLoader object");

            loadFile = loadFileType;
            // this.CsvPathName = CsvPathName;
        }

        public void LoadCsvFile()
        {
            loadFile.LoadFile();
        }
    }
}
