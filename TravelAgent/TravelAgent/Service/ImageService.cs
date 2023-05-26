using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TravelAgent.Core;

namespace TravelAgent.Service
{
    public class ImageService
    {
        private readonly Consts _consts;

        public ImageService(
            Consts consts)
        {
            _consts = consts;
        }

        public BitmapImage GetFromLocalStorage(string relativePath)
        {
            string fullPath = Path.GetFullPath(relativePath); ;
            Uri imageUri = new Uri(fullPath);

            return new BitmapImage(imageUri);
        }
    }
}
