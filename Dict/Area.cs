using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dict
{
    public class Area
    {
        private static Dictionary<int, string> d = null;
        static Area()
        {
            d = new Dictionary<int, string>(); 
            d.Add(1, "福田区");
            d.Add(2, "罗湖区");
            d.Add(3, "南山区");
            d.Add(4, "盐田区");
            d.Add(5, "宝安区");
            d.Add(6, "龙岗区");
            d.Add(7, "坪山区");
            d.Add(8, "龙华区");
            d.Add(9, "光明区");
            d.Add(10, "大鹏新区");
            d.Add(11, "市直属");
        }
        public static Dictionary<int,string> Get()
        {
            return d;
        }
        public static string GetVal(int key)
        {
            return d.ContainsKey(key) ? d[key] : "";
        }
    }
}
