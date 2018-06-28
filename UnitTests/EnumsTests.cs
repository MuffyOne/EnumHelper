using NUnit.Framework;
using System;
using System.Linq;

namespace EnumHelper.UnitTests
{
    [TestFixture]
    public class EnumsTests
    {
        #region Test Elements
        public enum ParseEnumTestEnum
        {
            Test,
            Ok,
            Ko
        }

        public sealed class ValueAttribute : Attribute
        {
            #region Constructors

            public ValueAttribute(double value)
            {
                Value = value;
            }

            #endregion

            #region Properties

            public double Value { get; private set; }

            #endregion
        }

        public enum ValueEnum
        {
            [Value(70.0)]
            Test,
            [Value(100.0)]
            Ok,
            [Value(71.1)]
            SeventyOneDotOne
        }

        #endregion

        #region ParseEnum
        [Test]
        [TestCase(ParseEnumTestEnum.Ko, "ko")]
        [TestCase(ParseEnumTestEnum.Ok, "ok")]
        [TestCase(ParseEnumTestEnum.Test, "test")]
        public void ParseEnum_EnumValueIsReturnedCorrectly(ParseEnumTestEnum expectedResult, string strintToParse)
        {
            //Act
            var result = Enums.ParseEnum<ParseEnumTestEnum>(strintToParse);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }
        #endregion

        #region GetEnumDescription
        
        public enum GetEnumDescriptionEnum
        {
            [System.ComponentModel.Description("Test description")]
            Test,
            [System.ComponentModel.Description("Ok description")]
            Ok,
            Ko
        }

        [Test]
        [TestCase(GetEnumDescriptionEnum.Ok, "Ok description")]
        [TestCase(GetEnumDescriptionEnum.Test, "Test description")]
        public void ParseEnum_EnumValueHasDescriptionAttribute_ActualDescriptionMatchesExpected(GetEnumDescriptionEnum enumValue, string expectedResult)
        {
            //Act
            string actualResult = Enums.GetEnumDescription(enumValue);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void ParseEnum_EnumValueHasNoDescriptionAttribute_MethodReturnsEnumValueToString()
        {
            // Arrange
            const string expectedResult = "Ko";
            const GetEnumDescriptionEnum testEnum = GetEnumDescriptionEnum.Ko;

            //Act
            string actualResult = Enums.GetEnumDescription(testEnum);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void ParseEnum_EnumValueIsActuallyInteger_MethodReturnsEnumValueToString()
        {
            // Arrange
            const string expectedResult = "-1";
            const GetEnumDescriptionEnum testEnum = (GetEnumDescriptionEnum)(-1);

            //Act
            string actualResult = Enums.GetEnumDescription(testEnum);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void TryGetEnumDescription_EnumValueHasNoDescriptionAttribute_MethodReturnsFalse()
        {
            // Arrange
            const bool expectedResult = false;
            const string expectedReturnValue = "";
            const GetEnumDescriptionEnum testEnum = GetEnumDescriptionEnum.Ko;


            //Act
            string returnValue;
            bool actualResult = Enums.TryGetEnumDescription(testEnum, out returnValue);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedReturnValue, returnValue);
        }

        [Test]
        [TestCase(GetEnumDescriptionEnum.Ok, "Ok description")]
        [TestCase(GetEnumDescriptionEnum.Test, "Test description")]
        public void TryGetEnumDescription_EnumValueHasDescriptionAttribute_ActualDescriptionMatchesExpected(GetEnumDescriptionEnum enumValue, string expectedReturnValue)
        {

            const bool expectedResult = true;
            //Act
            string returnValue;
            bool actualResult = Enums.TryGetEnumDescription(enumValue, out returnValue);


            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedReturnValue, returnValue);
        }

        [Test]
        public void TryParseEnum_EnumValueIsOk_MethodReturnsTrueAndEnumValueToString()
        {
            // Arrange
            const bool expectedResult = true;
            Enum expectedReturnValue = GetEnumDescriptionEnum.Ok;

            //Act
            Enum outValue;
            var actualResult = Enums.TryParseEnum<GetEnumDescriptionEnum>("Ok", out outValue);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedReturnValue, outValue);
        }

        [Test]
        public void TryParseEnum_EnumValueIsKo_MethodReturnsFalseAndNull()
        {
            // Arrange
            const bool expectedResult = false;
            const string expectedReturnValue = null;

            //Act
            Enum outValue;
            bool actualResult = Enums.TryParseEnum<GetEnumDescriptionEnum>("Oasffdasdsfak", out outValue);

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedReturnValue, outValue);
        }

        #region GetEnumFromDescription - Generic
        [Test]
        public void GetEnumFromDescription_DescriptionIsOk_EnumIsReturned()
        {
            //Arrange
            const GetEnumDescriptionEnum expectedValue = GetEnumDescriptionEnum.Ok;

            //Act
            var actualResult = Enums.GetValueFromDescription<GetEnumDescriptionEnum>("Ok description");

            Assert.AreEqual(expectedValue, actualResult);
        }

        [Test]
        public void GetEnumFromDescription_DescriptionIsKo_EnumIsReturned()
        {
            //Arrange
            const string expectedValue = "Not found.\r\nParameter name: description";

            //Act
            TestDelegate actDelegate = () => Enums.GetValueFromDescription<GetEnumDescriptionEnum>("Ok fsasfa");

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(actDelegate);
            Assert.AreEqual(expectedValue, exception.Message);
        }
        #endregion

        #region GetEnumFromDescription - Type parameter
        [Test]
        public void GetEnumFromDescription_TypeParameter_DescriptionIsOk_EnumIsReturned()
        {
            //Arrange
            const GetEnumDescriptionEnum expectedValue = GetEnumDescriptionEnum.Ok;

            //Act
            var actualResult = Enums.GetValueFromDescription(typeof(GetEnumDescriptionEnum), "Ok description");

            Assert.AreEqual(expectedValue, actualResult);
        }

        [Test]
        public void GetEnumFromDescription_TypeParameter_DescriptionIsKo_EnumIsReturned()
        {
            //Arrange
            const string expectedValue = "Not found.\r\nParameter name: description";

            //Act
            TestDelegate actDelegate = () => Enums.GetValueFromDescription(typeof(GetEnumDescriptionEnum), "Ok fsasfa");

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(actDelegate);
            Assert.AreEqual(expectedValue, exception.Message);
        }
        #endregion

        #endregion

        #region GetAllEnumDescriptions
        [Test]
        public void GetAllEnumDescriptions_AllDescriptionsCorrectlyReturned_ExpectedArrayMatchesActual()
        {
            // Arrange
            string[] expectedValues = new string[]
            {
                "Test description",
                "Ok description",
                "Ko"
            };

            // Act
            string[] actualValues = Enums.GetAllEnumDescriptions(typeof(GetEnumDescriptionEnum));

            //  Assert
            CollectionAssert.AreEqual(expectedValues, actualValues);
        }
        #endregion

        #region Custom Enums

        [Test]
        [TestCase(ValueEnum.Ok, 100.0)]
        [TestCase(ValueEnum.Test, 70.0)]
        public void ParseEnum_EnumValueHasDescriptionAttribute_ActualDescriptionMatchesExpected(ValueEnum enumValue, double expectedResult)
        {
            //Act
            ValueAttribute actualResult = Enums.GetCustomAttribute<ValueAttribute>(enumValue);

            //Assert
            Assert.AreEqual(expectedResult, actualResult.Value);
        }

        [Test]
        public void GetTheListOfValues_ListIsReturnedProperly()
        {

            //Arrange
            var expectedListLength = 3;
            var expectedFirstMemeber = 70.0;
            var expectedSecondMember = 100.0;
            var expectedThirdMember = 71.1;

            //Act
            var actualResult = Enums.GetCustomAttributesList<ValueAttribute, ValueEnum>();
            var values = actualResult.Select(i => i.Value).ToArray();

            //Assert
            Assert.AreEqual(expectedListLength, actualResult.Count);
            Assert.AreEqual(expectedListLength, values.Length);
            Assert.AreEqual(expectedFirstMemeber, values.First());
            Assert.AreEqual(expectedSecondMember, values[1]);
            Assert.AreEqual(expectedThirdMember, values.Last());
        }

        [Test]
        [TestCase(70.0, ValueEnum.Test)]
        [TestCase(71.1, ValueEnum.SeventyOneDotOne)]
        [TestCase(100.0, ValueEnum.Ok)]
        public void GetValueFromCustomAttribute_PassCustomAttribute_ValueIsReturned(double searchString, ValueEnum expectedEnum)
        {
            //Act
            var actualResult = Enums.GetValueFromCustomAttribute<ValueEnum, ValueAttribute, double>(searchString, a => a.Value);

            Assert.That(actualResult, Is.EqualTo(expectedEnum));
        }
        #endregion
    }
}
