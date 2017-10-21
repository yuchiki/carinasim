using System;
using System.IO;
using System.Collections.Generic;

namespace CarinaSim
{
    static class BinaryReaderExtension
    {
        public static UInt32 ReadUInt32BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return ((uint)bytes[0] << 24) + ((uint)bytes[1] << 16) + ((uint)bytes[2] << 8) + (uint)bytes[3];
        }
    }

    public class MipsProgram
    {
        public struct ProgramInfo
        {
            
            public UInt32 textsize;
            public UInt32 datasize;
            public UInt32 entrypoint;
            public UInt32 textoffset;
            public UInt32 dataoffset;
            public UInt32 totalsize;
            public UInt32 stackoffset;

            public override string ToString()
            {
                return $"text:{textoffset:X8}-{textoffset+textsize:X8}, data:{dataoffset:X8}-{dataoffset+datasize:X8}, stack:{stackoffset:X8}-, entrypoint:{entrypoint:X8}";
            }
        }

        public readonly ProgramInfo Info;
        public List<UInt32> program;

        public MipsProgram(FileStream f)
        {
            BinaryReader reader = new BinaryReader(f);
            if (new String(reader.ReadChars(4)) != "CARN")
                throw new Exception("Not a carn file!");

            Func<UInt32> getWord = () => reader.ReadUInt32BigEndian();
            Info = new ProgramInfo
            { textsize = getWord() << 2,
                datasize = getWord() << 2,
                entrypoint = getWord() << 2
            };

            Info.textoffset = 0;
            Info.dataoffset = Info.textsize;
            Info.stackoffset = Info.textsize + Info.datasize;
            Info.totalsize = Info.textsize + Info.datasize;

            program = new List<UInt32>(checked((int)Info.totalsize));
            for (int i = 0; i < Info.totalsize / 4; i++)
            {
                program.Add(getWord());
            }
        }

        public MipsProgram(string fileName)
            : this(File.Open(fileName, FileMode.Open))
        {
        }
    }
}

