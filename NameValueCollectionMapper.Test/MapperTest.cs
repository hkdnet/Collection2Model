using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Collection2Model.Mapper.Test
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

            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);

            Assert.AreEqual<int>(1, ret.IntProp);
            Assert.AreEqual<bool>(true, ret.BoolProp);
            Assert.AreEqual<String>("STRING", ret.StrPropUpper);
            Assert.AreEqual<string>("string", ret.StrPropLower);
            Assert.AreEqual<double>(0.0, ret.DoubleProp);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Throw_exception_with_str_to_int_convert()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", "invalid!");

            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
            Assert.Fail();
        }
        [TestMethod]
        public void Ignore_listed_prop()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", "1");
            c.Add("IntProp2", "1");
            var ignoring = new List<String>();
            ignoring.Add("IntProp");

            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c, ignoring);
            Assert.AreEqual<int>(0, ret.IntProp);
            Assert.AreEqual<int>(1, ret.IntProp2);
        }
        [TestMethod]
        public void Ignore_field()
        {
            var c = new NameValueCollection();
            c.Add("intField", "1");
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
            Assert.AreEqual<int>(0, ret.intField);
        }

        [TestMethod]
        public void Ignore_private_prop()
        {
            var c = new NameValueCollection();
            c.Add("PrivateIntProp", "1");
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
            Assert.AreEqual<int>(0, ret.GetPrivateIntProp());
        }
        [TestMethod]
        public void Ignore_by_IgnorePropertyAttribute()
        {
            var c = new NameValueCollection();
            c.Add("IgnorePropByAttr", "1");
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
            Assert.AreEqual<int>(0, ret.IgnorePropByAttr);
        }
        [TestMethod]
        public void Nullvalue_set_null()
        {
            var c = new NameValueCollection();
            c.Add("StrPropUpper", null);
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
            Assert.AreEqual<String>(null, ret.StrPropUpper);
        }
    }
    public class TestModel
    {
        public int IntProp { get; set; }
        public int IntProp2 { get; set; }
        public bool BoolProp { get; set; }
        public String StrPropUpper { get; set; }
        public string StrPropLower { get; set; }
        public double DoubleProp { get; set; }
        public int intField;
        private int PrivateIntProp { get; set; }
        public int GetPrivateIntProp()
        {
            return PrivateIntProp;
        }
        [IgnorePropertyAttribute]
        public int IgnorePropByAttr { get; set; }
    }
}
