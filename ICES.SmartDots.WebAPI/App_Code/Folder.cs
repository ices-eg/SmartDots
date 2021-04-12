using System;
using System.Collections.Generic;
using System.IO;

namespace WebAPI.App_Code
{
    public partial class Folder
    {
        public Folder()
        {
            File = new HashSet<File>();
        }

        public Guid Id { get; set; }
        public string Path { get; set; }
        public DateTime DateCreation { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<File> File { get; set; }
    }
}
