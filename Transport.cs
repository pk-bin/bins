using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Data;
using SharpDX;
using System.Linq;
using System.Runtime.InteropServices;
using SciChart.Charting.Model.DataSeries;
using System.IO;

namespace WpfApplication1
{
    public class Transport
    {
        // 비동기 통신을 위한 객체
        public class AsyncObject
        {
            public Byte[] Buffer;
            public Socket WorkingSocket;

            // 인자로 받은 BufferSize만큼 할당.
            public AsyncObject(Int32 bufferSize)
            {
                this.Buffer = new Byte[bufferSize];
            }
        }
        private Boolean g_Connected;
        private Socket m_ClientSocket = null;
        private AsyncCallback m_fnReceiveHandler;
        private float[] Left_Spec_Data = new float[Constant.MAX_SPEC];
        private float[] Right_Spec_Data = new float[Constant.MAX_SPEC];
        private byte[] receive_header = new byte[Constant.HEADER_SIZE];
        private byte[] data_buffer = null;
        private int header_pointer = 0;
        private int data_pointer = 0;
        private int data_length;
        private int real_length;
        private bool header_error = false;
        private UInt32 protocol;
        private Task task = null;
        public double Spec_dFreq_Start;
        public double RTS1_dFreq_Start;
        public double RTS2_dFreq_Start;
        public int Spec_iBin_SP;
        public int RTS1_iBin_SP;
        public int RTS2_iBin_SP;
        public int spec_count = 0;
        public int[] xvalue = new int[Constant.MAX_SPEC];
        SubWindow sub = new SubWindow();
        // 생성자 호출시 ReceiveHandler 할당.
        public Transport()
        {
            m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
            for (int i = 0; i < Constant.MAX_SPEC; i++)
            {
                xvalue[i] = i;
            }
        }

        // 연결 여부 확인
        public Boolean Connected
        {
            get
            {
                return g_Connected;
            }
        }

        // 서버에 연결하기 위한 함수.
        public void ConnectToServer(String hostName, UInt16 hostPort)
        {
            //이미 연결된 경우
            if (m_ClientSocket != null && m_ClientSocket.Connected == true)
            {
                Console.Write("이미 연결되어 있습니다.");
                return;
            }

            // TCP 통신을 위한 소켓을 생성합니다.
            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            Boolean isConnected = false;
            try
            {
                // 연결 시도
                m_ClientSocket.Connect(hostName, hostPort);

                // 연결 성공
                isConnected = true;
            }
            catch
            {
                // 연결 실패 (연결 도중 오류가 발생함)
                isConnected = false;
            }
            g_Connected = isConnected;

            if (isConnected)
            {
                // 4096 바이트의 크기를 갖는 바이트 배열을 가진 AsyncObject 클래스 생성
                AsyncObject ao = new AsyncObject(4096);

                // 작업 중인 소켓을 저장하기 위해 sockClient 할당
                ao.WorkingSocket = m_ClientSocket;

                // 비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 메서드 사용!
                m_ClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);

                Console.WriteLine("연결 성공!");
            }
            else
            {
                Console.WriteLine("연결 실패!");
            }
        }
        // 서버와 연결을 끊기위한 함수
        public void DisconnectToServer()
        {
            Boolean isConnected = true;

            //연결되어 있지 않은 경우
            if (m_ClientSocket == null || m_ClientSocket.Connected == false)
            {
                Console.WriteLine("연결이 되어있지 않습니다.");
                return;
            }

            Send_Sock_Close();          // Socket_Close 를 보냄

            // 양쪽 회선 절단
            m_ClientSocket.Shutdown(SocketShutdown.Both);
            m_ClientSocket.Close();
            if (m_ClientSocket.Connected)
                Console.WriteLine("연결을 끊지 못했습니다.");
            else
            {
                Console.WriteLine("연결이 끊어졌습니다.");
                isConnected = false;
            }
            g_Connected = isConnected;

        }

        // 데이터 수신시 자동적으로 호출되는 함수.
        public void handleDataReceive(IAsyncResult ar)
        {

            AsyncObject ao = (AsyncObject)ar.AsyncState;

            Int32 recv_bytes;
            Int32 buffer_pointer;

            try
            {
                recv_bytes = ao.WorkingSocket.EndReceive(ar);
                buffer_pointer = 0;
            }
            catch
            {
                return;
            }
            if (header_error)
            {
                int header_locate = Find_Header(ao.Buffer, buffer_pointer);
                if (!header_error)
                {
                    recv_bytes -= (Constant.HEADER_SIZE + header_locate);              //패킷을 사용한 만큼 받은패킷수를 빼주고,
                    buffer_pointer += (Constant.HEADER_SIZE + header_locate);           //받은패킷의 포인터의 값을 증가시킨다.
                }
            }

            while (recv_bytes > 0)
            {
                if (header_pointer < Constant.HEADER_SIZE)                       // 헤더를 다 받지 않은 경우 수행
                {
                    int header_limit = Constant.HEADER_SIZE - header_pointer;   // 덜 채워진 헤더의 크기
                    int copy_length = (header_limit < recv_bytes ? header_limit : recv_bytes);    // 복사할 헤더의 크기
                    byte[] keys = ao.Buffer;
                    Buffer.BlockCopy(ao.Buffer, buffer_pointer, receive_header, header_pointer, copy_length);   // 버퍼에서 copy_length만큼 헤더에 복사
                    header_pointer += copy_length;      // 헤더를 채운 만큼 header_pointer 크기 증가
                    buffer_pointer += copy_length;      // 버퍼에서 사용한 만큼 위치 증가시킴
                    recv_bytes -= copy_length;          // 받은 패킷에서 사용한 만큼 크기 감소
                }
                if (header_pointer == Constant.HEADER_SIZE)      // 헤더를 다 받은 경우 (순차적으로 수행될 수 있기 때문에 else if가 아닌 if를 사용하는 것이 맞다.)
                {
                    if (data_pointer == 0)      // 헤더를 모두 채운 후 맨 처음에는 protocol과 data_size정보를 헤더에서 받아와야 한다.
                    {
                        protocol = BitConverter.ToUInt32(receive_header, 0);
                        data_length = BitConverter.ToInt32(receive_header, 4);
                        Valid_Check();
                        if (header_error)       // 만약에 헤더에 이상이 생겼을 경우 이 루틴으로 들어간다.
                        {
                            Console.WriteLine("Error State");
                            int header_locate = Find_Header(ao.Buffer, buffer_pointer);     // 헤더를 찾아낸다.
                            if (!header_error)                                               // 헤더를 찾았다면?
                            {
                                recv_bytes -= (Constant.HEADER_SIZE + header_locate);              // 패킷을 사용한 만큼 받은패킷수를 빼주고,
                                buffer_pointer += (Constant.HEADER_SIZE + header_locate);           // 받은패킷의 포인터의 값을 증가시킨다.
                            }
                            else                                            // 못찾았으면? while문 빠져나옴
                            {
                                break;
                            }
                        }
                    }
                    if (recv_bytes <= 0) break;

                    int data_limit = real_length - data_pointer;                                    // 현재 최대로 받을수 있는 데이터
                    int copy_length = data_limit < recv_bytes ? data_limit : recv_bytes;            // 지금 받은 데이터와 받을수 있는 데이터중 작은 값
                    Buffer.BlockCopy(ao.Buffer, buffer_pointer, data_buffer, data_pointer, copy_length);    // 복사
                    data_pointer += copy_length;
                    buffer_pointer += copy_length;
                    recv_bytes -= copy_length;                  // 포인터류 크기 재조정

                    if (data_pointer == real_length)            // 가득 차면?
                    {
                        data_pointer = 0;
                        header_pointer = 0;
                        switch (protocol)                       // 프로토콜에 따라 처리
                        {
                            case Constant.PS_GET_SPEC:
                                task = new Task(Receive_Spec_Data);
                                break;
                            case Constant.PS_GET_RTS1:
                                task = new Task(Receive_RTS1_Data);
                                break;
                            case Constant.PS_GET_RTS2:
                                task = new Task(Receive_RTS2_Data);
                                break;
                            case Constant.GPS_HEADER:
                                task = new Task(Receive_GPS_Data);
                                break;
                        }
                    }
                }
            }

            try
            {
                ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);    // Receive 준비
            }
            catch (Exception ex)
            {
                Console.WriteLine("자료 수신 대기 도중 오류 발생! 메세지: {0}", ex.Message);
                return;
            }
        }

        public void Valid_Check()
        {
            UInt32 valid_protocol = protocol & Constant.OP_MASK1;
            UInt32 valid_length = (UInt32)data_length & Constant.OP_MASK2;
            real_length = data_length & (int)Constant.OP_MASK3;

            if (valid_protocol == Constant.HEAD_MARK1 && valid_length == Constant.HEAD_MARK2)    // 유효성 검사
            {
                if (task != null)   // 다음 패킷이 유효한 경우 Task 실행
                {
                    task.Start();
                    task.Wait();
                    task = null;
                }
                data_buffer = new byte[real_length];
            }
            else                    // 다음 패킷이 유효하지 않았을 경우 헤더시작점을 다시 찾아야 함.
            {
                task = null;
                header_error = true;
            }
        }

        private int Find_Header(byte[] buffer, int buffer_pointer)
        {
            for (int i = buffer_pointer; i < buffer.Length - Constant.HEADER_SIZE; i++)         // 버퍼의 끝 까지 돌면서 헤더를 찾는다.
            {
                UInt32 valid_protocol = BitConverter.ToUInt32(buffer, i) & Constant.OP_MASK1;
                UInt32 valid_length = BitConverter.ToUInt32(buffer, i + 4) & Constant.OP_MASK2;

                if (valid_protocol == Constant.HEAD_MARK1 && valid_length == Constant.HEAD_MARK2)  //유효성 체크.
                {
                    Buffer.BlockCopy(buffer, i, receive_header, 0, Constant.HEADER_SIZE);
                    header_pointer = Constant.HEADER_SIZE;
                    header_error = false;
                    return i - buffer_pointer;
                }
            }
            header_error = true;
            return -1;
        }
        public void Receive_RTS1_Data()
        {
                    /*
            byte[] temp = null;
            double dFreq_Start;
            int iBin_SP;

            Zlib.DecompressData(data_buffer, out temp);

            if (temp == null) return;

            dFreq_Start = BitConverter.ToDouble(temp, 0);
            iBin_SP = BitConverter.ToInt32(temp, 8);

            if (dFreq_Start != RTS1_dFreq_Start || iBin_SP != RTS1_iBin_SP)
            {
                RTS1_dFreq_Start = dFreq_Start;
                RTS1_iBin_SP = iBin_SP;

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ((MainWindow)Application.Current.MainWindow).RTS1_CreateXAxis();
                });
            }
            int k = 12;

            byte[,] RTS_Data = new byte[Constant.RTS_Y, Constant.RTS_X];
            for (int i = 0; i < Constant.RTS_X; i++)
            {
                for (int j = 0; j < (Constant.RTS_Y); j++)
                {
                    RTS_Data[j, i] = temp[k];
                    k++;
                }
            }
            //Buffer.BlockCopy(temp, 12, RTS_Data, 0, (Constant.RTS_Y * Constant.RTS_X));

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).set_RSA1(ref RTS_Data);
            });
            */
        }
        public void Receive_RTS2_Data()
        {
            /*
            byte[] temp;
            double dFreq_Start;
            int iBin_SP;

            Zlib.DecompressData(data_buffer, out temp);
            if (temp == null) return;

            dFreq_Start = BitConverter.ToDouble(temp, 0);
            iBin_SP = BitConverter.ToInt32(temp, 8);

            if (dFreq_Start != RTS2_dFreq_Start || iBin_SP != RTS2_iBin_SP)
            {
                RTS2_dFreq_Start = dFreq_Start;
                RTS2_iBin_SP = iBin_SP;

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ((MainWindow)Application.Current.MainWindow).RTS2_CreateXAxis();
                });
            }

            byte[,] RTS_Data = new byte[Constant.RTS_X, Constant.RTS_Y];
            Buffer.BlockCopy(temp, 0, RTS_Data, 0, (Constant.RTS_Y * Constant.RTS_X));

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).set_RSA2(ref RTS_Data);
            });
            */
        }
        public void Receive_Spec_Data()
        {
            
            byte[] temp;
            double dFreq_Start;
            int iBin_SP;

            Zlib.DecompressData(data_buffer, out temp);

            if (temp == null) return;

           /* dFreq_Start = BitConverter.ToDouble(temp, 0);
            iBin_SP = BitConverter.ToInt32(temp, 8);

            if (dFreq_Start != Spec_dFreq_Start || iBin_SP != Spec_iBin_SP)
            {
                Spec_dFreq_Start = dFreq_Start;
                Spec_iBin_SP = iBin_SP;

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ((Setting_window)Application.Current.Windows).Spec_CreateXAxis();
                });
            }*/
            Buffer.BlockCopy(temp, 12, Left_Spec_Data, 0, Constant.MAX_SPEC * 4);
            Buffer.BlockCopy(temp, 12 + (Constant.MAX_SPEC * 4)*3 , Right_Spec_Data, 0, Constant.MAX_SPEC * 4);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).Left_Spectrum(ref Left_Spec_Data);
                ((MainWindow)Application.Current.MainWindow).Right_Spectrum(ref Right_Spec_Data);
            });
            
        }
        public void Receive_GPS_Data()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).Add_GPS_Data(ref data_buffer);
            });
        }
        public void Send_Sock_Close()
        {
            AsyncObject ao = new AsyncObject(1);
            OP_DATA buffer = new OP_DATA();
            Byte[] temp;
            Byte[] temp2;

            buffer.lData_len = 1;
            buffer.lData_len &= 0x0FFFFFFF;
            buffer.lData_len |= 0x60000000;
            buffer.uOp_protocol = Constant.SOCK_CLOSE;
            buffer.Data = new Byte[256];

            temp = buffer.Serialize();
            temp2 = new Byte[(int)(buffer.lData_len & 0x0FFFFFFF) + 8];
            Array.Copy(temp, temp2, (int)(buffer.lData_len & 0x0FFFFFFF) + 8);

            ao.Buffer = temp2;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, null, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine("전송 중 오류 발생!\n메세지: {0}", ex.Message);
            }
        }
        public void Send_Spec_In()  // Default Function
        {
            Send_Spec_In(752, 120, 0, 1, 1);
        }
        public void Send_Spec_In(double dFreq, float fSpan, byte u8Ant_Mode, byte u8Data_Send, byte u8New_Spec)
        {
            AsyncObject ao = new AsyncObject(1);

            OP_DATA buffer = new OP_DATA();
            SPEC_IN spec_in = new SPEC_IN();
            Byte[] temp;
            Byte[] temp2;

            spec_in.dFreq = dFreq;
            spec_in.fSpan = fSpan;
            spec_in.u8Ant_Mode = u8Ant_Mode;
            spec_in.u8Data_Send = u8Data_Send;
            spec_in.u8New_Spec = u8New_Spec;

            buffer.lData_len = 15;
            buffer.lData_len &= 0x0FFFFFFF;
            buffer.lData_len |= 0x60000000;
            buffer.uOp_protocol = 0x6EAE0101;
            buffer.Data = new Byte[256];
            temp = spec_in.Serialize();

            Array.Copy(temp, buffer.Data, temp.Length);

            temp = buffer.Serialize();
            temp2 = new Byte[(int)(buffer.lData_len & 0x0FFFFFFF) + 8];
            Array.Copy(temp, temp2, (int)(buffer.lData_len & 0x0FFFFFFF) + 8);

            ao.Buffer = temp2;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, null, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine("전송 중 오류 발생!\n메세지: {0}", ex.Message);
            }
        }
        public void Send_RTS1_In()  // Default Function
        {
            Send_RTS1_In(723, 12000, 10, 1, 0, 1, 1);
        }
        public void Send_RTS1_In(double dFreq, float fSpan, int iFrame_PS, byte u8Ant_Mode, byte u8Plot_Mode, byte u8Data_Send, byte u8New_Rts1)
        {
            AsyncObject ao = new AsyncObject(1);

            OP_DATA buffer = new OP_DATA();
            RTS1_IN rsa_in = new RTS1_IN();
            Byte[] temp;
            Byte[] temp2;

            rsa_in.dFreq = dFreq;
            rsa_in.fSpan = fSpan;
            rsa_in.iFrame_PS = iFrame_PS;
            rsa_in.u8Ant_Mode = u8Ant_Mode;
            rsa_in.u8Plot_Mode = u8Plot_Mode;
            rsa_in.u8Data_Send = u8Data_Send;
            rsa_in.u8New_Rts1 = u8New_Rts1;

            buffer.lData_len = 20;
            buffer.lData_len &= 0x0FFFFFFF;
            buffer.lData_len |= 0x60000000;
            buffer.uOp_protocol = 0x6EAE0103;
            buffer.Data = new Byte[256];
            temp = rsa_in.Serialize();

            Array.Copy(temp, buffer.Data, temp.Length);

            temp = buffer.Serialize();
            temp2 = new Byte[(int)(buffer.lData_len & 0x0FFFFFFF) + 8];
            Array.Copy(temp, temp2, (int)(buffer.lData_len & 0x0FFFFFFF) + 8);

            ao.Buffer = temp2;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, null, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine("전송 중 오류 발생!\n메세지: {0}", ex.Message);
            }
        }
        public void Send_RTS2_In()  // Default Function
        {
            Send_RTS2_In(778, 12000, 10, 1, 0, 1, 1);
        }
        public void Send_RTS2_In(double dFreq, float fSpan, int iFrame_PS, byte u8Ant_Mode, byte u8Plot_Mode, byte u8Data_Send, byte u8New_Rts2)
        {
            AsyncObject ao = new AsyncObject(1);

            OP_DATA buffer = new OP_DATA();
            RTS2_IN rsa_in = new RTS2_IN();
            Byte[] temp;
            Byte[] temp2;

            rsa_in.dFreq = dFreq;
            rsa_in.fSpan = fSpan;
            rsa_in.iFrame_PS = iFrame_PS;
            rsa_in.u8Ant_Mode = u8Ant_Mode;
            rsa_in.u8Plot_Mode = u8Plot_Mode;
            rsa_in.u8Data_Send = u8Data_Send;
            rsa_in.u8New_Rts2 = u8New_Rts2;

            buffer.lData_len = 20;
            buffer.lData_len &= 0x0FFFFFFF;
            buffer.lData_len |= 0x60000000;
            buffer.uOp_protocol = 0x6EAE0105;
            buffer.Data = new Byte[256];
            temp = rsa_in.Serialize();

            Array.Copy(temp, buffer.Data, temp.Length);

            temp = buffer.Serialize();
            temp2 = new Byte[(int)(buffer.lData_len & 0x0FFFFFFF) + 8];
            Array.Copy(temp, temp2, (int)(buffer.lData_len & 0x0FFFFFFF) + 8);

            ao.Buffer = temp2;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, null, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine("전송 중 오류 발생!\n메세지: {0}", ex.Message);
            }
        }
        public void Send_GPS_In()
        {
            AsyncObject ao = new AsyncObject(1);

            OP_DATA buffer = new OP_DATA();

            Byte[] temp;
            Byte[] temp2;

            buffer.lData_len = 0;
            buffer.lData_len &= 0x0FFFFFFF;
            buffer.lData_len |= 0x60000000;
            buffer.uOp_protocol = 0x6EAEFFFF;
            buffer.Data = new Byte[256];

            temp = buffer.Serialize();
            temp2 = new Byte[(int)(buffer.lData_len & 0x0FFFFFFF) + 8];
            Array.Copy(temp, temp2, (int)(buffer.lData_len & 0x0FFFFFFF) + 8);

            ao.Buffer = temp2;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, null, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine("전송 중 오류 발생!\n메세지: {0}", ex.Message);
            }
        }
    }
}
