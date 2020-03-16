using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Drawing;
using System.IO;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Prompt();
                return;
            }

            if (args[0] == "?")
            {
                ShowHelp();
                return;
            }

            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments.");
                Prompt();
                return;
            }

            var path = args[0];
            var mode = args[1];
            var size = args[2];

            bool[] valid = new bool[3];

            valid[0] = ValidatePath(path);
            valid[1] = ValidateMode(mode);
            valid[2] = ValidateSize(size);

            if (Array.IndexOf(valid, false) > -1)
            {
                return;
            }

            ProcessImage(path, mode, size);

        }

        private static void ProcessImage(string path, string mode, string size)
        {
            var fileInfo = new DirectoryInfo(path).GetFiles();

            foreach (var file in fileInfo)
            {
                byte[] photoBytes = File.ReadAllBytes(file.FullName);
                ISupportedImageFormat format = new JpegFormat { Quality = 70 };
                
                Size imageSize;
                if (mode == "width")
                {
                    imageSize = new Size(int.Parse(size), 0);
                }
                else
                {
                    imageSize = new Size(0, int.Parse(size));
                }

                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        using (ImageFactory imageFactory = new ImageFactory())
                        {
                            imageFactory.Load(inStream)
                                        .Resize(imageSize)
                                        .Format(format)
                                        .Save(outStream);
                        }

                        if (!Directory.Exists(file.Directory + "/" + size))
                        {
                            Directory.CreateDirectory(file.Directory + "/" + size);
                        }
                        // Do something with the stream.
                        using (FileStream outFile = new FileStream(file.Directory + "/" + size + "/" + file.Name, FileMode.Create, FileAccess.Write))
                        {
                            byte[] bytes = new byte[outStream.Length];
                            outStream.Read(bytes, 0, (int)outStream.Length);
                            outFile.Write(bytes, 0, bytes.Length);
                            outStream.Close();
                        }
                    }
                }

            }


        }

        private static bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("No such directory.");
                return false;
            }

            var directory = Path.GetDirectoryName(path);
            var invalidPathChars = Path.GetInvalidPathChars();

            if (directory.IndexOfAny(invalidPathChars) > -1)
            {
                Console.WriteLine("Invalid characters in directory.");
                return false;
            }

            
            //var fileName = Path.GetFileName(path);
            //var invalidFileChars = Path.GetInvalidFileNameChars();

            //if (fileName.IndexOfAny(invalidFileChars) > -1) {
            //    Console.WriteLine("Invalid characters in file name.");
            //    return false;
            //}

            return true;

        }

        private static bool ValidateMode(string mode)
        {
            if (mode.ToLower() != "width" && mode.ToLower() != "height")
            {
                Console.WriteLine("Invalid mode.  Mode Should be either \"width\" or \"height\".");
                return false;
            }

            return true;

        }

        private static bool ValidateSize(string size)
        {
            int result;
            bool isInt = int.TryParse(size, out result);
            return isInt;
        }

        private static void Prompt ()
        {
            Console.WriteLine("Usage: <path> <mode width | height> <pixel> or ? for help.");
        }

        private static void ShowHelp()
        {
            throw new NotImplementedException();
        }
    }
}
