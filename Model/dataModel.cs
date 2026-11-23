using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICP.Model
{
    public class dataModel
    {
        //序号
        public int Id { get; set; }
        //检测时间
        public string testTime { get; set; } = "";
        //产品名称
        public string projectName { get; set; } = "";
        //通道1数据
        public string data1 { get; set; } = "";
        //通道2数据
        public string data2 { get; set; } = "";
        //通道3数据
        public string data3 { get; set; } = "";
        //通道4数据
        public string data4 { get; set; } = "";
        //通道5数据
        public string data5 { get; set; } = "";
        //通道6数据
        public string data6 { get; set; } = "";
        //通道7数据
        public string data7 { get; set; } = "";
        //通道8数据
        public string data8 { get; set; } = "";
        //整条记录的判定：全部合格 = "合格"，任一通道超差 = "不合格"，未标定 = "未标定"
        public string result { get; set; } = "未标定";
        //不合格通道号列表，如 "1,3,5"；没有则为 ""
        public string ngChannels { get; set; } = "";
    }
}
