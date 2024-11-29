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
                Console.WriteLine($"File {input.Name} cannot be read\nPlease ensure the target file exists and bStream has permission to access it");
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(input.FullName);
            Console.WriteLine($"Read {fileBytes.Length} bytes from file");
            byte[] output;
            string ext;

            if (CompressMode)
            {
                try
                {
                    output = CompressBrotli(fileBytes);
                    ext = ".brt";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while compressing\n\n{ex}");
                    return;
                }
            }
            else
            {
                try
                {
                    output = DecompressBrotli(fileBytes);
                    ext = ".bin";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while decompressing\n\n{ex}");

                    if (ex is InvalidOperationException)
                    {
                        Console.WriteLine("\nThe decompressor reported an invalid operation. This usually means the input is not brotli compressed");
                    }

                    return;
                }
            }

            FileInfo oFile = new(Path.GetFileNameWithoutExtension(input.FullName) + ext);

            if (!oFile.Exists)
            {
                Console.WriteLine($"Writing to {oFile.FullName}");

                try
                {
                    File.WriteAllBytes(oFile.FullName, output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while writing to the output file\nEnsure bStream has write permission in the folder containing the input file\n{ex}");
                    return;
                }

            }
            else
            {
                Console.WriteLine($"Error: Output file already exists!\nRefusing to overwrite existing file: {oFile.Name}");
                return;
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
