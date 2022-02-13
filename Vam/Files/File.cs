using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Files
{
    /// <summary>
    /// Информация о текстовом файле
    /// </summary>
    public class UserFile
    {
        public string Path { get; set; }
        public string Information { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserFile(string path, string info, DateTime createdDate)
        {
            this.Path = path;
            this.Information = info;
            this.CreatedDate = createdDate;
        }
    }
}
