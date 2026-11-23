using NModbus;
using NModbus.Serial;
using System.IO.Ports;

namespace TICP.Helper
{
    /// <summary>
    /// Modbus RTU通信工具类
    /// </summary>
    public class ModbusRtuClient : IDisposable
    {
        private SerialPort? _serialPort;
        private IModbusSerialMaster? _modbusMaster;
        private bool _isConnected = false;

        /// <summary>
        /// 已连接的主站；未连接时给出明确异常，避免各处空引用。
        /// </summary>
        private IModbusSerialMaster Master =>
            _modbusMaster ?? throw new InvalidOperationException("Modbus RTU未连接");

        /// <summary>
        /// 串口连接状态
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// 串口名称
        /// </summary>
        public string? PortName { get; private set; }

        /// <summary>
        /// 从站地址
        /// </summary>
        public byte SlaveId { get; set; } = 1;

        private DataFormat _dataFormat;

        public ModbusRtuClient(DataFormat format = DataFormat.ABCD)
        {
            _dataFormat = format;
        }

        #region 连接管理

        /// <summary>
        /// 连接串口
        /// </summary>
        /// <param name="portName">串口名称，如"COM1"</param>
        /// <param name="baudRate">波特率，默认9600</param>
        /// <param name="dataBits">数据位，默认8</param>
        /// <param name="stopBits">停止位，默认One</param>
        /// <param name="parity">校验位，默认None</param>
        /// <returns>连接是否成功</returns>
        public bool Connect(string portName, int baudRate = 9600, int dataBits = 8,
            StopBits stopBits = StopBits.One, Parity parity = Parity.None)
        {
            // 无条件清理：读失败后可能只置了标志位但串口仍被占用，
            // 不清理会导致下次 Open 抛「端口已访问」。
            Disconnect();

            try
            {
                // 创建串口实例
                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                {
                    Handshake = Handshake.None,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };

                _serialPort.Open();

                // 创建Modbus主站
                var adapter = new SerialPortAdapter(_serialPort);
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateRtuMaster(adapter);

                PortName = portName;
                _isConnected = true;

                Log.Info($"Modbus RTU连接成功: {portName}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("连接串口失败", ex);
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _modbusMaster?.Dispose();
                _modbusMaster = null;

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort?.Dispose();
                _serialPort = null;

                _isConnected = false;
                PortName = null;
                Console.WriteLine("Modbus RTU连接已断开");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"断开连接时发生错误: {ex.Message}");
            }
        }

        #endregion

        #region 读取功能

        /// <summary>
        /// 读取线圈状态（0x01功能码）
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numPoints">读取点数</param>
        /// <returns>线圈状态数组</returns>
        public bool[] ReadCoils(ushort startAddress, ushort numPoints)
        {
            ValidateConnection();

            try
            {
                return Master.ReadCoils(SlaveId, startAddress, numPoints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取线圈失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取离散输入状态（0x02功能码）
        /// </summary>
        public bool[] ReadInputs(ushort startAddress, ushort numPoints)
        {
            ValidateConnection();

            try
            {
                return Master.ReadInputs(SlaveId, startAddress, numPoints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取离散输入失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取保持寄存器（0x03功能码）
        /// </summary>
        public ushort[] ReadHoldingRegisters(ushort startAddress, ushort numPoints)
        {
            ValidateConnection();

            try
            {
                return Master.ReadHoldingRegisters(SlaveId, startAddress, numPoints);
            }
            catch (Exception ex)
            {
                Log.Error("读取保持寄存器失败", ex);
                // 彻底释放串口，保证下一轮自动重连不会撞上仍被占用的端口。
                Disconnect();
                throw;
            }
        }

        /// <summary>
        /// 读取输入寄存器（0x04功能码）
        /// </summary>
        public ushort[] ReadInputRegisters(ushort startAddress, ushort numPoints)
        {
            ValidateConnection();

            try
            {
                return Master.ReadInputRegisters(SlaveId, startAddress, numPoints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取输入寄存器失败: {ex.Message}");
                throw;
            }
        }

        #region 读取short16类型

        /// <summary>
        /// 读取1个int类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <returns></returns>
        public short ReadShort16(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 1);

            return ModbusDataConverter.ConvertFromRegisters<short>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取多个int32类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public short[] ReadShorts(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(1 * numPoints));

            short[] result = new short[numPoints];

            for (int i = 0; i < original.Length; i += 1)
            {
                ushort[] caculate = new ushort[1];
                caculate[0] = original[i];
                result[i] = ModbusDataConverter.ConvertFromRegisters<short>(caculate, 0, _dataFormat);
            }

            return result;
        }
        #endregion

        #region 读取short16类型

        /// <summary>
        /// 读取1个int类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <returns></returns>
        public ushort ReadUShort(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 1);

            return ModbusDataConverter.ConvertFromRegisters<ushort>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取多个int32类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public ushort[] ReadUShorts(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(1 * numPoints));

            ushort[] result = new ushort[numPoints];

            for (int i = 0; i < original.Length; i += 1)
            {
                ushort[] caculate = new ushort[1];
                caculate[0] = original[i];
                result[i] = ModbusDataConverter.ConvertFromRegisters<ushort>(caculate, 0, _dataFormat);
            }

            return result;
        }
        #endregion

        #region 读取Int32类型

        /// <summary>
        /// 读取1个int类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <returns></returns>
        public int ReadInt32(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 2);

            return ModbusDataConverter.ConvertFromRegisters<int>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取多个int32类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public int[] ReadInt32s(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(2 * numPoints));

            int[] result = new int[numPoints];

            for (int i = 0; i < original.Length; i+=2)
            {
                ushort[] caculate = new ushort[2];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                result[i / 2] = ModbusDataConverter.ConvertFromRegisters<int>(caculate, 0, _dataFormat);
            }

            return result;
        }
        #endregion

        #region 读取UInt32类型

        /// <summary>
        /// 读取1个int类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <returns></returns>
        public uint ReadUInt32(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 2);

            return ModbusDataConverter.ConvertFromRegisters<uint>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取多个int32类型的数据
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public uint[] ReadUInt32s(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(2 * numPoints));

            uint[] result = new uint[numPoints];

            for (int i = 0; i < original.Length; i += 2)
            {
                ushort[] caculate = new ushort[2];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                result[i / 2] = ModbusDataConverter.ConvertFromRegisters<uint>(caculate, 0, _dataFormat);
            }

            return result;
        }
        #endregion

        #region 读取float32类型

        /// <summary>
        /// 读取float类型的数据
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <returns></returns>
        public float ReadFloat(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 2);
            return ModbusDataConverter.ConvertFromRegisters<float>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取float类型的数组
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public float[] ReadFloats(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(2 * numPoints));
            
            float[] result = new float[numPoints];
            for (int i = 0; i < original.Length; i += 2)
            {
                ushort[] caculate = new ushort[2];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                result[i / 2] = ModbusDataConverter.ConvertFromRegisters<float>(caculate, 0, _dataFormat);
            }
            return result;
        }
        #endregion

        #region 读取long64类型

        /// <summary>
        /// 读取long类型的数据
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <returns></returns>
        public long ReadLong(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 4);
            return ModbusDataConverter.ConvertFromRegisters<long>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取long类型的数组
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public long[] ReadLongs(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(4 * numPoints));

            long[] result = new long[numPoints];
            for (int i = 0; i < original.Length; i += 4)
            {
                ushort[] caculate = new ushort[4];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                caculate[2] = original[i + 2];
                caculate[3] = original[i + 3];
                result[i / 4] = ModbusDataConverter.ConvertFromRegisters<long>(caculate, 0, _dataFormat);
            }
            return result;
        }
        #endregion

        #region 读取Ulong64类型

        /// <summary>
        /// 读取long类型的数据
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <returns></returns>
        public ulong ReadULong(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 4);
            return ModbusDataConverter.ConvertFromRegisters<ulong>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取long类型的数组
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public ulong[] ReadULongs(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(4 * numPoints));

            ulong[] result = new ulong[numPoints];
            for (int i = 0; i < original.Length; i += 4)
            {
                ushort[] caculate = new ushort[4];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                caculate[2] = original[i + 2];
                caculate[3] = original[i + 3];
                result[i / 4] = ModbusDataConverter.ConvertFromRegisters<ulong>(caculate, 0, _dataFormat);
            }
            return result;
        }
        #endregion

        #region 读取double64类型

        /// <summary>
        /// 读取float类型的数据
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <returns></returns>
        public double ReadDouble(ushort startAddress)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, 4);
            return ModbusDataConverter.ConvertFromRegisters<double>(original, 0, _dataFormat);
        }
        /// <summary>
        /// 读取float类型的数组
        /// </summary>
        /// <param name="startAddress">开始的地址</param>
        /// <param name="numPoints">读取数量</param>
        /// <returns></returns>
        public double[] ReadDoubles(ushort startAddress, ushort numPoints)
        {
            var original = Master.ReadHoldingRegisters(SlaveId, startAddress, (ushort)(4 * numPoints));

            double[] result = new double[numPoints];
            for (int i = 0; i < original.Length; i += 4)
            {
                ushort[] caculate = new ushort[4];
                caculate[0] = original[i];
                caculate[1] = original[i + 1];
                caculate[2] = original[i + 2];
                caculate[3] = original[i + 3];
                result[i / 4] = ModbusDataConverter.ConvertFromRegisters<double>(caculate, 0, _dataFormat);
            }
            return result;
        }
        #endregion

        #endregion

        #region 写入功能

        /// <summary>
        /// 写入单个线圈（0x05功能码）
        /// </summary>
        public void WriteSingleCoil(ushort coilAddress, bool value)
        {
            ValidateConnection();

            try
            {
                Master.WriteSingleCoil(SlaveId, coilAddress, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入单个线圈失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 写入多个线圈（0x0F功能码）
        /// </summary>
        public void WriteMultipleCoils(ushort startAddress, bool[] values)
        {
            ValidateConnection();

            try
            {
                Master.WriteMultipleCoils(SlaveId, startAddress, values);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入多个线圈失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 写入单个寄存器（0x06功能码）
        /// </summary>
        public void WriteSingleRegister(ushort registerAddress, ushort value)
        {
            ValidateConnection();

            try
            {
                Master.WriteSingleRegister(SlaveId, registerAddress, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入单个寄存器失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 写入多个寄存器（0x10功能码）
        /// </summary>
        public void WriteMultipleRegisters(ushort startAddress, ushort[] values)
        {
            ValidateConnection();

            try
            {
                Master.WriteMultipleRegisters(SlaveId, startAddress, values);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入多个寄存器失败: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 验证连接状态
        /// </summary>
        private void ValidateConnection()
        {
            if (!_isConnected || _serialPort == null || !_serialPort.IsOpen || _modbusMaster is null)
            {
                throw new InvalidOperationException("Modbus RTU未连接");
            }
        }

        /// <summary>
        /// 获取可用的串口列表
        /// </summary>
        /// <returns>串口名称数组</returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// 测试通信是否正常
        /// </summary>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>测试结果</returns>
        public bool TestConnection(int timeoutMs = 1000)
        {
            if (!_isConnected || _serialPort is null) return false;
            var port = _serialPort;

            try
            {
                var originalTimeout = port.ReadTimeout;
                port.ReadTimeout = timeoutMs;

                // 尝试读取一个寄存器来测试连接
                Master.ReadHoldingRegisters(SlaveId, 0, 1);

                port.ReadTimeout = originalTimeout;
                return true;
            }
            catch (Exception ex)
            {
                Log.Warn($"通信测试失败: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region IDisposable实现

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ModbusRtuClient()
        {
            Dispose(false);
        }

        #endregion
    }

    public enum DataFormat
    {
        /// <summary>
        /// 大端序 (ABCD) - 寄存器顺序和寄存器内部都是高字节在前
        /// 寄存器0: 高字节A, 低字节B
        /// 寄存器1: 高字节C, 低字节D
        /// 内存顺序: A, B, C, D
        /// </summary>
        ABCD,

        /// <summary>
        /// 小端序 (CDAB) - 寄存器顺序颠倒，但每个寄存器内部高字节在前
        /// 寄存器0: 高字节C, 低字节D
        /// 寄存器1: 高字节A, 低字节B
        /// 内存顺序: C, D, A, B
        /// </summary>
        CDAB,

        /// <summary>
        /// 字节交换 (BADC) - 每个寄存器内部字节交换
        /// 寄存器0: 低字节B, 高字节A
        /// 寄存器1: 低字节D, 高字节C
        /// 内存顺序: B, A, D, C
        /// </summary>
        BADC,

        /// <summary>
        /// 寄存器顺序颠倒且字节交换 (DCBA)
        /// 寄存器0: 低字节D, 高字节C
        /// 寄存器1: 低字节B, 高字节A
        /// 内存顺序: D, C, B, A
        /// </summary>
        DCBA
    }

    public static class ModbusDataConverter
    {
        #region 基础转换方法
        /// <summary>
        /// 从ushort数组转换为指定数据类型的通用方法
        /// </summary>
        public static T ConvertFromRegisters<T>(ushort[] registers, int startIndex, DataFormat format)
        {
            if (registers == null) throw new ArgumentNullException(nameof(registers));

            return typeof(T) switch
            {
                Type t when t == typeof(short) => (T)(object)ConvertToInt16(registers, startIndex, format),
                Type t when t == typeof(ushort) => (T)(object)ConvertToUInt16(registers, startIndex, format),
                Type t when t == typeof(int) => (T)(object)ConvertToInt32(registers, startIndex, format),
                Type t when t == typeof(uint) => (T)(object)ConvertToUInt32(registers, startIndex, format),
                Type t when t == typeof(long) => (T)(object)ConvertToInt64(registers, startIndex, format),
                Type t when t == typeof(ulong) => (T)(object)ConvertToUInt64(registers, startIndex, format),
                Type t when t == typeof(float) => (T)(object)ConvertToFloat(registers, startIndex, format),
                Type t when t == typeof(double) => (T)(object)ConvertToDouble(registers, startIndex, format),
                _ => throw new NotSupportedException($"不支持的数据类型: {typeof(T).Name}")
            };
        }
        #endregion

        #region 16位数据类型转换
        public static short ConvertToInt16(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 1, format);
            // 修复：手动拼接大端序，无视系统端序
            return (short)((bytes[0] << 8) | bytes[1]);
        }

        public static ushort ConvertToUInt16(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 1, format);
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }
        #endregion

        #region 32位数据类型转换
        public static int ConvertToInt32(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 2, format);
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        }

        public static uint ConvertToUInt32(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 2, format);
            return (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
        }

        public static float ConvertToFloat(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 2, format);
            // 浮点型：必须反转字节适配系统小端序
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        #endregion

        #region 64位数据类型转换
        public static long ConvertToInt64(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 4, format);
            return ((long)bytes[0] << 56) | ((long)bytes[1] << 48) | ((long)bytes[2] << 40) | ((long)bytes[3] << 32) |
                   ((long)bytes[4] << 24) | ((long)bytes[5] << 16) | ((long)bytes[6] << 8) | bytes[7];
        }

        public static ulong ConvertToUInt64(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 4, format);
            return ((ulong)bytes[0] << 56) | ((ulong)bytes[1] << 48) | ((ulong)bytes[2] << 40) | ((ulong)bytes[3] << 32) |
                   ((ulong)bytes[4] << 24) | ((ulong)bytes[5] << 16) | ((ulong)bytes[6] << 8) | bytes[7];
        }

        public static double ConvertToDouble(ushort[] registers, int startIndex, DataFormat format)
        {
            byte[] bytes = GetBytesFromRegisters(registers, startIndex, 4, format);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 从寄存器数组获取字节数组（核心逻辑不变，修复端序问题）
        /// </summary>
        private static byte[] GetBytesFromRegisters(ushort[] registers, int startIndex, int registerCount, DataFormat format)
        {
            if (registers == null) throw new ArgumentNullException(nameof(registers));
            if (startIndex < 0 || startIndex + registerCount > registers.Length)
                throw new ArgumentException($"寄存器数组索引越界。需要{registerCount}个寄存器，数组长度{registers.Length}，起始索引{startIndex}");

            byte[] bytes = new byte[registerCount * 2];

            for (int i = 0; i < registerCount; i++)
            {
                ushort register = registers[startIndex + i];
                int byteIndex = i * 2;

                switch (format)
                {
                    case DataFormat.ABCD:
                        bytes[byteIndex] = (byte)(register >> 8);
                        bytes[byteIndex + 1] = (byte)(register & 0xFF);
                        break;
                    case DataFormat.CDAB:
                        int reverseIndex = registerCount - 1 - i;
                        bytes[reverseIndex * 2] = (byte)(register >> 8);
                        bytes[reverseIndex * 2 + 1] = (byte)(register & 0xFF);
                        break;
                    case DataFormat.BADC:
                        bytes[byteIndex] = (byte)(register & 0xFF);
                        bytes[byteIndex + 1] = (byte)(register >> 8);
                        break;
                    case DataFormat.DCBA:
                        reverseIndex = registerCount - 1 - i;
                        bytes[reverseIndex * 2] = (byte)(register & 0xFF);
                        bytes[reverseIndex * 2 + 1] = (byte)(register >> 8);
                        break;
                    default:
                        throw new ArgumentException($"不支持的数据格式: {format}");
                }
            }
            return bytes;
        }

        /// <summary>
        /// 批量转换（修复空值+越界校验）
        /// </summary>
        public static T[] ConvertMultiple<T>(ushort[] registers, int startIndex, int count, DataFormat format)
        {
            if (registers == null) throw new ArgumentNullException(nameof(registers));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            int dataSize = GetDataTypeSize<T>();
            int registersPerData = dataSize / 2;
            int totalRegistersNeeded = startIndex + count * registersPerData;

            if (totalRegistersNeeded > registers.Length)
                throw new ArgumentException($"批量转换寄存器越界。需要{totalRegistersNeeded}个寄存器，数组仅有{registers.Length}个");

            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                int dataStartIndex = startIndex + i * registersPerData;
                result[i] = ConvertFromRegisters<T>(registers, dataStartIndex, format);
            }
            return result;
        }

        /// <summary>
        /// 获取数据类型字节大小
        /// </summary>
        private static int GetDataTypeSize<T>()
        {
            return typeof(T) switch
            {
                Type t when t == typeof(short) || t == typeof(ushort) => 2,
                Type t when t == typeof(int) || t == typeof(uint) || t == typeof(float) => 4,
                Type t when t == typeof(long) || t == typeof(ulong) || t == typeof(double) => 8,
                _ => throw new NotSupportedException($"不支持的数据类型: {typeof(T).Name}")
            };
        }
        #endregion
    }
}
