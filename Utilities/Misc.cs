using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Api.Utilities
{
    public class Misc
    {
        public static byte[] GetImageFromBase64(string base64EncodedImage)
        {
            base64EncodedImage = Regex.Replace(base64EncodedImage, "^data:image/[^;]*;base64,?", "");
            base64EncodedImage = base64EncodedImage.Replace(" ", "+");
            return Convert.FromBase64String(base64EncodedImage);
        }

        public static string GetRandomFileName(string fileName)
        {
            return Guid.NewGuid().ToString().ToLower() + Path.GetExtension(fileName).ToLower();
        }
    }
}