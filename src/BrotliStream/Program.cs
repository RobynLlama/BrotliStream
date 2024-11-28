using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BrotliStreamApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Brotli stream app startup");

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: bStream [mode] [filename]\n  Mode: -c (compress) -d (decompress)");
                return;
            }

            bool CompressMode = args[0] == "-c";
            FileInfo input = new(args[1]);

            if (!input.Exists)
            {
                Console.WriteLine($"File {input.Name} cannot be read");
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(input.FullName);
            Console.WriteLine($"Read {fileBytes.Length} bytes from file");
            byte[] output;
            string ext;

            if (CompressMode)
            {
                output = CompressBrotli(fileBytes);
                ext = ".brt";
            }
            else
            {
                output = DecompressBrotli(fileBytes);
                ext = ".bin";
            }

            FileInfo oFile = new(Path.GetFileNameWithoutExtension(input.FullName) + ext);

            if (!oFile.Exists)
            {
                Console.WriteLine($"Writing to {oFile.FullName}");
                File.WriteAllBytes(oFile.FullName, output);
            }
            else
            {
                Console.WriteLine("Error: Output file already exists!");
            }

        }

        static byte[] CompressBrotli(byte[] data)
        {
            //Console.WriteLine($"data: {data}");

            using (var outputStream = new MemoryStream())
            {
                using (var brotliStream = new BrotliStream(outputStream, CompressionMode.Compress))
                {
                    brotliStream.Write(data, 0, data.Length);
                }

                byte[] compressedData = outputStream.ToArray();
                //Console.WriteLine("Compressed Data: " + BitConverter.ToString(compressedData));
                return compressedData;
            }
        }

        static byte[] DecompressBrotli(byte[] data)
        {

            using (var inputStream = new MemoryStream(data))
            {
                using (var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress))
                using (var decompressedStream = new MemoryStream())
                {
                    brotliStream.CopyTo(decompressedStream);
                    byte[] decompressedData = decompressedStream.ToArray();
                    //string decompressedText = Encoding.UTF8.GetString(decompressedData);

                    //Console.WriteLine("Decompressed Text: " + decompressedText);
                    return decompressedData;
                }
            }
        }
    }
}
