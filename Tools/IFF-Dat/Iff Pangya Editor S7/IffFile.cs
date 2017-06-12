using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Iff_Pangya_Editor_S7
{
        public class IffFile
        {
            public ushort[] MagicNumber = new ushort[] { 11, 12, 13 };
            public ushort ObjectsInFile;
            public IFF_REGION Region;

            public bool CheckMagicNumber(BinaryReader reader)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(4L, SeekOrigin.Begin);
                ushort num2 = reader.ReadUInt16();
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
                return this.MagicNumber.Contains<ushort>(num2);
            }

            public static Encoding FileEncoding(IFF_REGION region)
            {
                IFF_REGION iff_region = region;
                if (iff_region <= IFF_REGION.Japan)
                {
                    if (iff_region == IFF_REGION.Default)
                    {
                        return Encoding.GetEncoding(0x36a);
                    }
                    if (iff_region != IFF_REGION.Japan)
                    {
                        return Encoding.GetEncoding(0x36a);
                    }
                }
                switch (iff_region)
                {
                    case IFF_REGION.Japan_8960:
                    case IFF_REGION.Japan_52428:
                        return Encoding.GetEncoding(0x3a4);

                case IFF_REGION.Korea_30395:
                        return Encoding.GetEncoding(0x3b5);
                }

            //unknow so normal encoding
            return Encoding.GetEncoding(0x36a);
        }

            public void GetIffRegion(BinaryReader reader)
            {
                long position = reader.BaseStream.Position;

                reader.BaseStream.Seek(2L, System.IO.SeekOrigin.Begin);
                ushort RegionIff = reader.ReadUInt16();
                switch (RegionIff)
                {
                    case 0:
                        this.Region = IFF_REGION.Default;
                        return;

                    case 0x224:
                    case 0xcccc:
                        this.Region = IFF_REGION.Japan;
                        return;

                    case 0x2300:
                        this.Region = IFF_REGION.Japan_8960;
                        return;

                    case 0x76bb:
                        this.Region = IFF_REGION.Korea_30395;
                        return;
                }
                this.Region = IFF_REGION.Default;
            }

            public ushort GetNumberOfRecords(BinaryReader reader)
            {
                long position = reader.BaseStream.Position;
                reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                ushort num2 = reader.ReadUInt16();
                reader.BaseStream.Seek(position, SeekOrigin.Begin);
                return num2;
            }

            public long GetRecordLength(BinaryReader reader)
            {
                ushort numberOfRecords = this.GetNumberOfRecords(reader);
                long num3 = reader.BaseStream.Length - 8L;
                return (long)(num3 / ((long)numberOfRecords));
            }

            public void JumpToFirstRecord(BinaryReader reader)
            {
                reader.BaseStream.Seek(8L, System.IO.SeekOrigin.Begin);
            }

            public void JumpToFirstRecord(BinaryWriter writer)
            {
                writer.BaseStream.Seek(8L, System.IO.SeekOrigin.Begin);
            }

            public void StubRecords(BinaryWriter writer, int recordsetLength, int itemCount)
            {
                byte[] buffer = new byte[recordsetLength];
                for (int i = 0; i < recordsetLength; i++)
                {
                    buffer[i] = 0;
                }
                for (int j = 0; j < itemCount; j++)
                {
                    writer.BaseStream.Write(buffer, 0, recordsetLength);
                }
                writer.Seek(8, SeekOrigin.Begin);
            }

            public void WriteIffFileHeader(BinaryWriter writer)
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(this.ObjectsInFile);
                this.WriteIffRegion(writer);
                switch (this.Region)
                {
                    case IFF_REGION.Default:
                    case IFF_REGION.Japan:
                    case IFF_REGION.Japan_8960:
                    case IFF_REGION.Japan_52428:
                        writer.Write(this.MagicNumber[2]);
                        break;

                    case IFF_REGION.Korea_30395:
                        writer.Write(this.MagicNumber[1]);
                        break;

                    default:
                        writer.Write(this.MagicNumber[0]);
                        break;
                }
                writer.Seek(2, SeekOrigin.Current);
            }

            public void WriteIffRegion(BinaryWriter writer)
            {
                writer.Write((ushort)this.Region);
            }

            [Serializable, StructLayout(LayoutKind.Sequential)]
            public struct _SYSTEMTIME
            {
                public short wYear;
                public short wMonth;
                public short wDayOfWeek;
                public short wDay;
                public short wHour;
                public short wMinute;
                public short wSecond;
                public short wMilliseconds;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct IFF_FILE_HEADER
            {
                public ushort NumberOfRecords;
                public ushort Region;
                public uint Version;
            }

            public enum IFF_REGION
            {
                Default = 0,
                Japan = 0x224,
                Japan_52428 = 0xcccc,
                Japan_8960 = 0x2300,
                Korea_30395 = 0x76bb
            }
        }

}
