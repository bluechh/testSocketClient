using System;
using System.Buffers;
using System.Text;
using SuperSocket;
using SuperSocket.ProtoBase;

namespace testSocketClient
{
    public class MyPackageFilter : FixedHeaderReceiveFilter<MyPackage>
    {
        /// <summary>
        /// Header size is 3
        /// 1: packet header
        /// 2-3: body length
        /// </summary>
        public MyPackageFilter(int headerSize)
            : base(headerSize)
        {

        }

        public override MyPackage ResolvePackage(IBufferStream bufferStream)
        {
            var package = new MyPackage();

            var packetHeaderByte = bufferStream.ReadByte();

            package.Header = (PACKET_HEADER)packetHeaderByte;

            // get length
            var len = bufferStream.ReadUInt16(false);
            package.length = len;

            // get type
            var packetTypeByte = bufferStream.ReadByte();
            package.OpType = (PACKET_TYPE)packetTypeByte;

            // get the rest of the data in the bufferStream and then read it as utf8 string
            var dataSource = new byte[package.length - 1];
            bufferStream.Read(dataSource, 0, package.length - 1);
            package.Body = Encoding.UTF8.GetString(dataSource) ?? "";
            Console.WriteLine($"Server response: {package.Body}");

            return package;
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            try
            {
                var packetLength = bufferStream.Skip(1).ReadUInt16();
                return packetLength;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}