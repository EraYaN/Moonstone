using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppLocalPLayer
{
    public class MusicList
    {
        #region properties

        private string[] filePaths;
        public string[] FilePaths
        {
            get
            {
                return filePaths;
            }
        }

        #endregion

        public MusicList()
        {
            filePaths = Directory.GetFiles(MainWindow.musicPath, "*.mp3",SearchOption.AllDirectories);
        }
    }
}
