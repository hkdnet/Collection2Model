using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NameValueCollectionMapper;
using System.Collections.Specialized;

namespace NameValueCollectionMapper.Test
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", "1");
            c.Add("BoolProp", "true");
            c.Add("StrPropUpper", "STRING");
            c.Add("StrPropLower", "string");
            c.Add("DoubleProp", "0.0");

            var ret = NemeValueCollectionMapper.Mapper.Mapping<TestModel>(c);

            Assert.AreEqual<int>(1, ret.IntProp);
            Assert.AreEqual<bool>(true, ret.BoolProp);
            Assert.AreEqual<String>("STRING", ret.StrPropUpper);
            Assert.AreEqual<string>("string", ret.StrPropLower);
            Assert.AreEqual<double>(0.0, ret.DoubleProp);

        }
    }
    public class TestModel
    {
        public int IntProp { get; set; }
        public bool BoolProp { get; set; }
        public String StrPropUpper { get; set; }
        public string StrPropLower { get; set; }
        public double DoubleProp { get; set; }
    }
}
