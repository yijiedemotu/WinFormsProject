using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TICP.Helper;

namespace TICP.Common
{
    public static class mainCommon
    {
        // 整个程序只创建一个 Modbus 对象、一个 SqlHelper 对象

        // 创建ModbusRtuClient的实例
        public static ModbusRtuClient modbusRtuClient = new ModbusRtuClient();

        // 创建SqlHelper的实例
        public static SqlHelper sqlHelper { get; set; } = new SqlHelper();
        // 创建一个ushort数组，用于存储8个通道的原始数据
        public static ushort[] oriData = new ushort[8];
        // 创建一个double数组，用于存储8个通道的数据
        public static double[] data = new double[8];
    }
}
