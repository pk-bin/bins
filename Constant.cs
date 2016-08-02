using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Constant
    {
        // Control
        public const Int32 CTRL_BASE = 0x6EAE1000;
        public const Int32 SOCK_OPEN = CTRL_BASE + 0x01;
        public const Int32 SOCK_CLOSE = CTRL_BASE + 0x02;
        public const Int32 SOCK_OK = CTRL_BASE + 0x03;
        public const Int32 SERV_INIT_DATA = CTRL_BASE + 0x04;
        public const Int32 WR_REG = CTRL_BASE + 0x05;
        public const Int32 RD_REG = CTRL_BASE + 0x06;
        public const Int32 SER_COMM = CTRL_BASE + 0x07;
        public const Int32 RF_PWR = CTRL_BASE + 0x08;

        // Data Type
        public const UInt32 DATA_BASE = 0x6EAE0010;
        public const UInt32 DATA_IQ = DATA_BASE + 0x01;
        public const UInt32 DATA_SPEC = DATA_BASE + 0x02;
        public const UInt32 DATA_RTS1 = DATA_BASE + 0x03;
        public const UInt32 DATA_RTS2 = DATA_BASE + 0x04;
        public const UInt32 DATA_RCDF = DATA_BASE + 0x05;
        public const UInt32 DATA_LTEUP = DATA_BASE + 0x06;
        public const UInt32 DATA_LTEDN = DATA_BASE + 0x07;
        public const UInt32 DATA_TDOA = DATA_BASE + 0x08;

        // Measurement 
        public const UInt32 MEA_BASE = 0x6EAE0100;
        public const UInt32 PS_SET_SPEC = MEA_BASE + 0x01;
        public const UInt32 PS_GET_SPEC = MEA_BASE + 0x02;
        public const UInt32 PS_SET_RTS1 = MEA_BASE + 0x03;
        public const UInt32 PS_GET_RTS1 = MEA_BASE + 0x04;
        public const UInt32 PS_SET_RTS2 = MEA_BASE + 0x05;
        public const UInt32 PS_GET_RTS2 = MEA_BASE + 0x06;
        public const UInt32 PS_SET_RCDF = MEA_BASE + 0x07;
        public const UInt32 PS_GET_RCDF = MEA_BASE + 0x08;
        public const UInt32 PS_SET_LTEUP = MEA_BASE + 0x09;
        public const UInt32 PS_GET_LTEUP = MEA_BASE + 0x0A;
        public const UInt32 PS_SET_LTEDN = MEA_BASE + 0x0B;
        public const UInt32 PS_GET_LTEDN = MEA_BASE + 0x0C;
        public const UInt32 PS_SET_TDOA = MEA_BASE + 0x0D;
        public const UInt32 PS_GET_TDOA = MEA_BASE + 0x0E;
        public const UInt32 PS_SET_RCV = MEA_BASE + 0x0F;
        public const UInt32 PS_GET_RCV = MEA_BASE + 0x10;

        // Error Check
        public const UInt32 ERRCHK_BASE = 0x6EAE0EE0;
        public const UInt32 HEADER_ERR = ERRCHK_BASE + 0x01;

        // GPS Test
        public const UInt32 GPS_HEADER = 0x6EAEFFFF;

        public const Int32 START_FREQ = 692;
        public const Int32 END_FREQ = 812;
        public const Int32 MAX_SPEC = 1201;
        public const Int32 TIME_VALUE = 100;
        public const Int32 KHZ = 1000;
        public const Int32 RTS_X = 801;
        public const Int32 RTS_Y = 512;
        public const Int32 ANG_NUM = 360;          // Resolution : 1 degree
        public const Int32 MAX_CONSTE = 100;          // Not exact(?)
        public const Int32 HEADER_SIZE = 8;            // OP용

        public const UInt32 OP_MASK1 = 0xFFFF0000;
        public const UInt32 OP_MASK2 = 0xF0000000;
        public const UInt32 OP_MASK3 = 0x0FFFFFFF;
        public const UInt32 HEAD_MARK1 = 0x6EAE0000;
        public const UInt32 HEAD_MARK2 = 0x60000000;

        public const double LATITUDE = 36.382847;       //
        public const double LONGITUDE = 127.365067;
        public const double ALTITUDE = 69.2;

        public const Int16 REMAIN_COUNT = 10;
        //public const double LATITUDE = 36.382793;     // 실제 위치
        //public const double LONGITUDE = 127.366103;
        //public const double ALTITUDE = 62.191738;
    }

    // -----------------------------------------
    // HR Transmit data packet 
    // OP Receive data packet
    // ----------------------------------------- 
    struct SERV_DATA
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint uOp_protocol;                               // Protocol
        [MarshalAs(UnmanagedType.U4)]
        public uint lData_len;                                  // Data length
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096 * 1024 / 4)]
        public byte[] Data;                                     // Data

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(SERV_DATA))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (SERV_DATA)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SERV_DATA));
            gch.Free();
        }
    }

    // -----------------------------------------
    // OP Transmit data packet 
    // HR Receive data packet
    // ----------------------------------------- 
    struct OP_DATA
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint uOp_protocol;                               // Protocol
        [MarshalAs(UnmanagedType.U4)]
        public uint lData_len;                                  // Data length
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Data;                                     // Data

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(OP_DATA))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (OP_DATA)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(OP_DATA));
            gch.Free();
        }
    }

    // -----------------------------------------
    // Receiver Input packet
    // -----------------------------------------
    struct RCV_IN
    {
        [MarshalAs(UnmanagedType.R4)]
        public float fRef_Lv;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Notch_Filt;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Rcv_Mode;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RCV_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RCV_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RCV_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // Spectrum Input packet
    // -----------------------------------------
    struct SPEC_IN
    {
        [MarshalAs(UnmanagedType.R8)] //  [MHz]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)] //  [kHz]
        public float fSpan;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Ant_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8New_Spec;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(SPEC_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (SPEC_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SPEC_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // Spectrum Output packet
    // -----------------------------------------
    struct SPEC_OUT
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq_Start;
        [MarshalAs(UnmanagedType.I4)]
        public float iBin_SP;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.MAX_SPEC * 4)]
        public float[] fSpec;                                     // 120001 : 120MHz(1kHz)

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(SPEC_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (SPEC_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(SPEC_OUT));
            gch.Free();
        }
    }

    public struct GPGGA
    {
        [MarshalAs(UnmanagedType.I4)]
        public int time_hour;
        [MarshalAs(UnmanagedType.I4)]
        public int time_minute;
        [MarshalAs(UnmanagedType.I4)]
        public int time_second;
        [MarshalAs(UnmanagedType.R8)]
        public double latitude;
        [MarshalAs(UnmanagedType.R8)]
        public double longitude;
        [MarshalAs(UnmanagedType.R8)]
        public double altitude;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(GPGGA))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (GPGGA)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(GPGGA));
            gch.Free();
        }
    }
    public struct Satellite
    {
        [MarshalAs(UnmanagedType.I2)]
        public short number;
        [MarshalAs(UnmanagedType.I2)]
        public short elevation;
        [MarshalAs(UnmanagedType.I2)]
        public short azimuth;
        [MarshalAs(UnmanagedType.I2)]
        public short SNR;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(Satellite))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (Satellite)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(Satellite));
            gch.Free();
        }
    }
    struct GPGLL
    {
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(GPGLL))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (GPGLL)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(GPGLL));
            gch.Free();
        }
    }
    struct GPRMC
    {
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(GPRMC))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (GPRMC)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(GPRMC));
            gch.Free();
        }
    }
    struct GPVTG
    {
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(GPVTG))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (GPVTG)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(GPVTG));
            gch.Free();
        }
    }
    // -----------------------------------------
    // RTS1 Input packet
    // -----------------------------------------
    struct RTS1_IN
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)]
        public float fSpan;
        [MarshalAs(UnmanagedType.I4)]
        public int iFrame_PS;           // Frame per Sec.
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Ant_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Plot_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8New_Rts1;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RTS1_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RTS1_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RTS1_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // RTS1 Output packet
    // -----------------------------------------
    struct RTS1_OUT
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq_Start;
        [MarshalAs(UnmanagedType.I4)]
        public float iBin_SP;
        [MarshalAs(UnmanagedType.I2, SizeConst = Constant.RTS_X * Constant.RTS_Y)]   // 801*512
        public short[] iHit_Cnt;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RTS1_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RTS1_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RTS1_OUT));
            gch.Free();
        }
    }

    // -----------------------------------------
    // RTS2 Input packet
    // -----------------------------------------
    struct RTS2_IN
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)]
        public float fSpan;
        [MarshalAs(UnmanagedType.I4)]
        public int iFrame_PS;           // Frame per Sec.
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Ant_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Plot_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8New_Rts2;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RTS2_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RTS2_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RTS2_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // RTS2 Output packet
    // -----------------------------------------
    struct RTS2_OUT
    {
        [MarshalAs(UnmanagedType.I2, SizeConst = Constant.RTS_X * Constant.RTS_Y)]   // 801*512
        public short[] sHit_Cnt;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RTS2_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RTS2_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RTS2_OUT));
            gch.Free();
        }
    }

    // -----------------------------------------
    // DF Input packet
    // -----------------------------------------
    struct RCDF_IN
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)]
        public float fCh_BW;
        [MarshalAs(UnmanagedType.R4)]
        public float fDF_Th;
        [MarshalAs(UnmanagedType.I4)]
        public int iTurn_Speed;
        [MarshalAs(UnmanagedType.I4)]
        public int iTurn_Resol;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8New_RCdf;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RCDF_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RCDF_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RCDF_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // DF Output packet
    // -----------------------------------------
    struct RCDF_OUT
    {
        [MarshalAs(UnmanagedType.I4, SizeConst = Constant.ANG_NUM)]
        public int[] iTurn_Ang;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.ANG_NUM)]
        public float[] f0mni_Lv;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.ANG_NUM)]
        public float[] fDirec_Lv;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.ANG_NUM)]
        public float[] fLv_Ratio;
        [MarshalAs(UnmanagedType.R4)]
        public float fLv_Ratio_peak;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(RCDF_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (RCDF_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(RCDF_OUT));
            gch.Free();
        }
    }
    // -----------------------------------------
    // LTE Uplink Input packet
    // -----------------------------------------
    struct LTEUP_IN
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)]
        public float fCh_BW;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Ant_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(LTEUP_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (LTEUP_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(LTEUP_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // LTE Downlink Output packet
    // -----------------------------------------
    struct LTEUP_OUT
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq_Offset;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.MAX_CONSTE)]
        public float[] fConstell_I;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.MAX_CONSTE)]
        public float[] fConstell_Q;
        [MarshalAs(UnmanagedType.I4)]
        public int iConstell_Num;
        [MarshalAs(UnmanagedType.R4)]
        public float fEVM;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_Lfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_Rfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_OBW;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_Lfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_Rfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_OBW;
        [MarshalAs(UnmanagedType.U1)]
        public byte bInterp;
        [MarshalAs(UnmanagedType.R8)]
        public double dInterp_Freq;
        [MarshalAs(UnmanagedType.I4)]
        public int iCell_ID;
        [MarshalAs(UnmanagedType.R4, SizeConst = 7)]
        public float[] fCCDF;                   // [100/10/1/0.1/0.01/
        [MarshalAs(UnmanagedType.I4)]
        public int iSpur_Mask;                  // LTE Mask


        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(LTEUP_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (LTEUP_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(LTEUP_OUT));
            gch.Free();
        }
    }

    // -----------------------------------------
    // LTE Downlink Input packet
    // -----------------------------------------
    struct LTEDN_IN
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq;
        [MarshalAs(UnmanagedType.R4)]
        public float fCh_BW;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Ant_Mode;
        [MarshalAs(UnmanagedType.U1)]
        public byte u8Data_Send;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(LTEDN_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (LTEDN_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(LTEDN_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // LTE Downlink Output packet
    // -----------------------------------------
    struct LTEDN_OUT
    {
        [MarshalAs(UnmanagedType.R8)]
        public double dFreq_Offset;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.MAX_CONSTE)]
        public float[] fConstell_I;
        [MarshalAs(UnmanagedType.R4, SizeConst = Constant.MAX_CONSTE)]
        public float[] fConstell_Q;
        [MarshalAs(UnmanagedType.I4)]
        public int iConstell_Num;
        [MarshalAs(UnmanagedType.R4)]
        public float fEVM;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_Lfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_Rfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dBeta_OBW;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_Lfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_Rfreq;
        [MarshalAs(UnmanagedType.R8)]
        public double dXdB_OBW;
        [MarshalAs(UnmanagedType.U1)]
        public byte bInterp;
        [MarshalAs(UnmanagedType.R8)]
        public double dInterp_Freq;
        [MarshalAs(UnmanagedType.I4)]
        public int iCell_ID;
        [MarshalAs(UnmanagedType.I4)]
        public int iSpur_Mask;                  // LTE Mask


        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(LTEDN_OUT))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (LTEDN_OUT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(LTEDN_OUT));
            gch.Free();
        }
    }

    // -----------------------------------------
    // Measurement Param. Input packet
    // -----------------------------------------

    struct PARAM_IN
    {
        public SPEC_IN StSpec;
        public RTS1_IN StRts1;
        public RTS2_IN StRts2;
        public RCDF_IN StRCdf;
        public RCV_IN StRcv;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(PARAM_IN))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (PARAM_IN)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(PARAM_IN));
            gch.Free();
        }
    }

    // -----------------------------------------
    // Client Information packet
    // -----------------------------------------
    struct CLNT_INFO
    {
        [MarshalAs(UnmanagedType.I4)]
        public int clnt_sock;
        [MarshalAs(UnmanagedType.I4)]
        public int dev_file;

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(CLNT_INFO))];
            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (CLNT_INFO)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(CLNT_INFO));
            gch.Free();
        }
    }
}

