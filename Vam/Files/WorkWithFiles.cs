using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Files
{
    public class WorkWithFiles
    {
        public static UserFile GetFile(string path)
        {
            throw new NotImplementedException();
        }
        public static string NormalizePath(string path)
        {
            return path.Trim().Trim('"');
        }
        /// <summary>
        /// Возврашает полный путь до файла в зависимости от выбранного пользователем формата
        /// </summary>
        /// <param name="userPathToFile"></param>
        /// <returns></returns>
        public static string GetPathToFile(string userPathToFile)
        {
            userPathToFile = NormalizePath(userPathToFile);
            var pathToFile = "";
            // если пользователь редактирует файл в текущем каталоге (название файла начинается с .\ или ./)
            if (
                userPathToFile[0] == '.' && (userPathToFile[1] == '\\' || userPathToFile[1] == '/')
                )
            {
                userPathToFile = userPathToFile.Substring(2); // убираем из строки .\ или ./
                // путь до файла представляет собой путь до каталога в котором находится пользователь + название файла
                pathToFile = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + userPathToFile;
            }
            // если пользователь редактирует файл в текущем каталоге (в названии файла нет .\ или ./)
            else if (
                !userPathToFile.Contains('\\') || !userPathToFile.Contains('/')
                )
            {
                // путь до файла представляет собой путь до каталога в котором находится пользователь + название файла
                pathToFile = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + userPathToFile;
            }
            // если пользователь указал полный путь до файла (начиная с метки диска)
            else if (
                Char.IsLetter(userPathToFile[0]) &&
                userPathToFile[1] == ':' &&
                userPathToFile[2] == System.IO.Path.DirectorySeparatorChar
                )
            {
                pathToFile = userPathToFile; // оставляем путь до файла без изменений
            }
            // в противном случае пользователь указал путь до файла, начиная с текущей папки
            else
            {
                // путь до файла представляет собой путь до каталога в котором находится пользователь + введенный пользователем путь
                pathToFile = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + userPathToFile;
            }
            return pathToFile;
        }
    }
}
