using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public void Null_should_be_mapped_to_default_value()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", null);
            c.Add("BoolProp", null);
            c.Add("DoubleProp", null);
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);

            Assert.AreEqual<int>(0, ret.IntProp);
            Assert.AreEqual<bool>(false, ret.BoolProp);
            Assert.AreEqual<double>(0.0, ret.DoubleProp);
        }
        [TestMethod]
        public void Empty_should_be_mapped_to_default_value()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", "");
            c.Add("BoolProp", "");
            c.Add("DoubleProp", "");
            var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);

            Assert.AreEqual<int>(0, ret.IntProp);
            Assert.AreEqual<bool>(false, ret.BoolProp);
            Assert.AreEqual<double>(0.0, ret.DoubleProp);
        }

        [TestMethod]
        public void Throw_exception_with_str_to_int_convert()
        {
            var c = new NameValueCollection();
            c.Add("IntProp", "invalid!");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<TestModel>(c);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                Assert.AreEqual<Type>(typeof(FormatException), ex.GetType());
            }
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
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                StringAssert.Equals("It's too long.", ex.Message);
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
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                StringAssert.Equals("Length1_8 should be 1char or more and less than 9chars", ex.Message);
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
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                StringAssert.Equals("Value should be between 1 and 100", ex.Message);
            }
        }
        [TestMethod]
        public void when_format_and_validation_error_too_throw_format_exception()
        {
            var c = new NameValueCollection();
            c.Add("Plus", "-1.0");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<RangeAttributeTestModel>(c);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                Assert.AreEqual<Type>(typeof(FormatException), ex.GetType());
            }
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
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                StringAssert.Equals("RequiredProp is required!", ex.Message);
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
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                StringAssert.Equals("RequiredProp is required!", ex.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CollectionCantBeNull()
        {
            var ret = Mapper.MappingFromNameValueCollection<ThrowMultipleExceptionTestModel>(null);
        }
        [TestMethod]
        public void CatchMultiExceptions()
        {
            var c = new NameValueCollection();
            c.Add("S", "");
            c.Add("I", "0");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<ThrowMultipleExceptionTestModel>(c);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(2, e.InnerExceptions.Count);
                var exceptions = new List<Exception>(e.InnerExceptions);
                Assert.IsTrue(exceptions.Any(ex =>
                {
                    return typeof(ValidationException) == ex.GetType() && ex.Message == "S is required!";
                }));
                Assert.IsTrue(exceptions.Any(ex =>
                {
                    return typeof(ValidationException) == ex.GetType() && ex.Message == "I should be 1 or 2.";
                }));
            }
        }
        [TestMethod]
        public void ThrowExceptionWithDefaultValueOfNull()
        {
            var c = new NameValueCollection();
            c.Add("S", "No Error");
            c.Add("I", null);
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<ThrowMultipleExceptionTestModel>(c);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                Assert.AreEqual<Type>(typeof(ValidationException), ex.GetType());
                StringAssert.Equals("I should be 1 or 2.", ex.Message);
            }
        }
        [TestMethod]
        public void ThrowExceptionWithDefaultValueOfEmpty()
        {
            var c = new NameValueCollection();
            c.Add("S", "No Error");
            c.Add("I", "");
            try
            {
                var ret = Mapper.MappingFromNameValueCollection<ThrowMultipleExceptionTestModel>(c);
                Assert.Fail();
            }
            catch (AggregateException e)
            {
                Assert.AreEqual<int>(1, e.InnerExceptions.Count);
                var ex = e.InnerExceptions[0];
                Assert.AreEqual<Type>(typeof(ValidationException), ex.GetType());
                StringAssert.Equals("I should be 1 or 2.", ex.Message);
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
    public class ThrowMultipleExceptionTestModel
    {
        [Required(ErrorMessage = "S is required!")]
        public string S { get; set; }
        [Range(1, 2, ErrorMessage = "I should be 1 or 2.")]
        public int I { get; set; }
    }
}
