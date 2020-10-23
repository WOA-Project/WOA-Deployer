using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Zafiro.Core.Files;

namespace Zafiro.Uwp.Controls
{
    public class UwpFile : IZafiroFile
    {
        private readonly StorageFile file;

        public string Name => file.Name;
        public Uri Source => new Uri(file.Path);

        public UwpFile(StorageFile file)
        {
            this.file = file;
        }


        public Task<Stream> OpenForRead()
        {
            return file.OpenStreamForReadAsync();
        }

        public Task<Stream> OpenForWrite()
        {
            return file.OpenStreamForWriteAsync();
        }
    }
}