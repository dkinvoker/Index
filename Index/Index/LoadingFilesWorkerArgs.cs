using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Index
{
    class LoadingFilesWorkerArgs
    {
        public StorageFolder Folder { get; set; }
        public ICollection<StorageFile> Files { get; set; }
    }
}
