# BrotliStream

Super basic dotnet brotli cmdline app

## Usage

`BrotliStream` [mode] [filename]

`Mode`: `-c` (compress) `-d` (decompress)

## Notes

- Compressed files will attempt to write to [filename].brt
- Decompressed files will attempt to write to [filename].bin
- Either mode will fail if those exact names are already present on the system (to prevent accidents)
