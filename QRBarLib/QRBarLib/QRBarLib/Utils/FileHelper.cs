using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SmartCodeLib.Utils
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class FileHelper
    {
        /// <summary>
        /// Genera un nombre de archivo único con prefijo, fecha/hora y extensión.
        /// </summary>
        public string GenerateFileName(string prefix, string extension = "png")
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{SanitizeFileName(prefix)}_{timestamp}.{extension}";
        }

        /// <summary>
        /// Combina la carpeta y el nombre de archivo en una ruta completa.
        /// </summary>
        public string GetFullPath(string folderPath, string fileName)
        {
            return Path.Combine(folderPath, fileName);
        }

        /// <summary>
        /// Crea la carpeta si no existe.
        /// </summary>
        public void EnsureDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        /// <summary>
        /// Valida si un nombre de archivo es seguro (sin caracteres inválidos).
        /// </summary>
        public bool IsValidFileName(string fileName)
        {
            return !Regex.IsMatch(fileName, @"[\\/:*?""<>|]");
        }

        /// <summary>
        /// Elimina caracteres inválidos de un nombre de archivo.
        /// </summary>
        public string SanitizeFileName(string name)
        {
            return Regex.Replace(name, @"[\\/:*?""<>|]", "_");
        }

        /// <summary>
        /// Copia un archivo a otra ubicación.
        /// </summary>
        public void CopyFile(string sourcePath, string destinationPath, bool overwrite = true)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        /// <summary>
        /// Mueve un archivo a otra ubicación.
        /// </summary>
        public void MoveFile(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }

        /// <summary>
        /// Elimina un archivo.
        /// </summary>
        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Verifica si un archivo existe.
        /// </summary>
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
