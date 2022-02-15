using System;
using System.IO;
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
            // если пользователь указал полный путь до файла (начиная с метки диска)
            else if (
                Char.IsLetter(userPathToFile[0]) &&
                userPathToFile[1] == ':' &&
                (userPathToFile[2] == System.IO.Path.DirectorySeparatorChar || userPathToFile[2] == '/' || userPathToFile[2] == '\\')
                )
            {
                pathToFile = userPathToFile; // оставляем путь до файла без изменений
            }
            // если пользователь редактирует файл в текущем каталоге (в названии файла нет .\ или ./)
            else if (
                !userPathToFile.Contains('\\') || !userPathToFile.Contains('/')
                )
            {
                // путь до файла представляет собой путь до каталога в котором находится пользователь + название файла
                pathToFile = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + userPathToFile;
            }
            // в противном случае пользователь указал путь до файла, начиная с текущей папки
            else
            {
                // путь до файла представляет собой путь до каталога в котором находится пользователь + введенный пользователем путь
                pathToFile = System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + userPathToFile;
            }
            return pathToFile;
        }
        /// <summary>
        /// Возвращает содержимого текстового файла в виде строки.
        /// Кодировка определяется средствами .NET
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllFile(string path)
        {
            using (var file = new StreamReader(path, true))
            {
                return file.ReadToEnd();
            }
        }
    }
}
