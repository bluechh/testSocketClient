namespace testSocketClient
{
    class PacketDef
    {
        public const UInt16 PACKET_HEADER_SIZE = 3;
        public const UInt16 PACKET_TYPE_SIZE = 1;
    }

    public enum PACKET_HEADER : byte
    {
        FIXED_HEADER = 0xAA,
    }

    public enum PACKET_TYPE : byte
    {
        PACKET_ID_INITIALIZE = 41,
        PACKET_ID_SYNC = 42,
    }


    public enum ERROR_CODE : UInt16
    {
        ERROR_NONE = 0,
    }
}


