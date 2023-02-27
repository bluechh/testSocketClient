using System;
using SuperSocket.ProtoBase;
using System.Text;

namespace testSocketClient
{
    public class MyPackage : IPackageInfo
    {

        public MyPackage()
        {
            Header = PACKET_HEADER.FIXED_HEADER;
            OpType = PACKET_TYPE.PACKET_ID_INITIALIZE;
            Body = "";
        }

        public MyPackage(string bodyMessage)
        {
            Header = PACKET_HEADER.FIXED_HEADER;
            OpType = PACKET_TYPE.PACKET_ID_INITIALIZE;
            length = (ushort)(bodyMessage.Length + 1);
            Body = bodyMessage;
        }

        public MyPackage(PACKET_TYPE packtType, string bodyMessage)
        {
            Header = PACKET_HEADER.FIXED_HEADER;
            OpType = packtType;
            length = (ushort)(bodyMessage.Length + 1);
            Body = bodyMessage;
        }


        public byte[] ToByteFormat()
        {
            var pktType = (byte)OpType;
            UInt16 bodyDataSize = 0;
            if (this.length != 0)
            {
                bodyDataSize = (UInt16)length;
            }
            var packetHeader = (byte)0xAA;
            var packetSize = (UInt16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

            var dataSource = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(packetHeader), 0, dataSource, 0, 1);

            var packetSizeByte = BitConverter.GetBytes(packetSize);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(packetSizeByte);
            }
            Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 1, 2);

            Buffer.BlockCopy(BitConverter.GetBytes(pktType), 0, dataSource, 3, 1);

            if (string.IsNullOrEmpty(Body) == false)
            {
                var bodyData = Encoding.UTF8.GetBytes(Body);
                Buffer.BlockCopy(bodyData, 0, dataSource, 4, bodyDataSize-1);
            }

            return dataSource;
        }

        public PACKET_HEADER Header { get; set; }

        public PACKET_TYPE OpType { get; set; }

        public UInt16 length { get; set; }

        public string Body { get; set; }
    }
}