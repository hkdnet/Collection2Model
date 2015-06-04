using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

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

        [TestMethod]
        public void StringLengthAttribute_validate_maxlength()
        {
            try
            {
                var c = new NameValueCollection();
                c.Add("Length8OrLess", "123456789");
                var ret = Mapper.MappingFromNameValueCollection<StringLengthAttributeTestModel>(c);
                Assert.Fail();
            }
            catch (ValidationException e)
            {
                StringAssert.Equals(e.Message, "It's too long.");
            }
        }
        [TestMethod]
        public void StringLengthAttribute_validate_minlength()
        {
            try
            {
                var c = new NameValueCollection();
                c.Add("Length1_8", "");
                var ret = Mapper.MappingFromNameValueCollection<StringLengthAttributeTestModel>(c);
                Assert.Fail();
            }
            catch (ValidationException e)
            {
                StringAssert.Equals(e.Message, "Length1_8 should be 1char or more and less than 9chars");
            }
        }
        [TestMethod]
        public void StringLengthAttribute_with_valid_value_doesnt_throw_exception()
        {
            var c = new NameValueCollection();
            c.Add("Length8OrLess", "");
            c.Add("Length1_8", "1");
            var ret = Mapper.MappingFromNameValueCollection<StringLengthAttributeTestModel>(c);
        }
        [TestMethod]
        public void Value_lower_than_range_causes_excetion()
        {
            var c = new NameValueCollection();
            c.Add("Plus", "1000");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<RangeAttributeTestModel>(c);
                Assert.Fail();
            }
            catch (ValidationException e)
            {
                StringAssert.Equals(e.Message, "Value should be between 1 and 100");
            }
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void when_format_and_validation_error_too_throw_format_exception()
        {
            var c = new NameValueCollection();
            c.Add("Plus", "-1.0");
            var ret = Mapper.MappingFromNameValueCollection<RangeAttributeTestModel>(c);
            Assert.Fail();
        }

        [TestMethod]
        public void with_required_prop_doesnt_throw_exception()
        {
            var c = new NameValueCollection();
            c.Add("RequiredProp", "1");
            var ret = Mapper.MappingFromNameValueCollection<RequiredAttributeTeestModel>(c);
            Assert.AreEqual<string>("1", ret.RequiredProp);
        }

        [TestMethod]
        public void without_required_prop_throws_exception()
        {
            var c = new NameValueCollection();
            c.Add("NotRequiredProp", "1");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<RequiredAttributeTeestModel>(c);
                Assert.Fail();
            }
            catch (ValidationException e)
            {
                StringAssert.Equals("RequiredProp is required!", e.Message);
            }
        }
        [TestMethod]
        public void empty_required_prop_throws_exception()
        {
            var c = new NameValueCollection();
            c.Add("RequiredProp", "");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<RequiredAttributeTeestModel>(c);
                Assert.Fail();
            }
            catch (ValidationException e)
            {
                StringAssert.Equals("RequiredProp is required!", e.Message);
            }
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
        [IgnoreProperty]
        public int IgnorePropByAttr { get; set; }
    }
    public class StringLengthAttributeTestModel
    {
        const string message = "It's too long.";
        [StringLength(8, ErrorMessage = message)]
        public string Length8OrLess { get; set; }

        [StringLength(8, MinimumLength = 1, ErrorMessage = "Length1_8 should be 1char or more and less than 9chars")]
        public string Length1_8 { get; set; }
    }
    public class RangeAttributeTestModel
    {
        [Range(1, 100, ErrorMessage = "Value should be between 1 and 100")]
        public int Plus { get; set; }
    }
    public class RequiredAttributeTeestModel
    {
        [Required(ErrorMessage = "RequiredProp is required!")]
        public String RequiredProp { get; set; }
        public int NotRequiredProp { get; set; }
    }
}
