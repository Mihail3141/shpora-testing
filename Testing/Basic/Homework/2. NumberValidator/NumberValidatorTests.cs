using NUnit.Framework;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    private static class TestData
    {
        public static IEnumerable<TestCaseData> InvalidConstructorParameters()
        {
            var onlyPositiveValues = new[] { true, false };

            foreach (var onlyPositive in onlyPositiveValues)
            {
                yield return new TestCaseData(-1, 2, onlyPositive)
                    .SetName($"Negative precision should throw (onlyPositive={onlyPositive})");
                yield return new TestCaseData(0, 5, onlyPositive)
                    .SetName($"Zero precision should throw (onlyPositive={onlyPositive})");
                yield return new TestCaseData(17, 17, onlyPositive)
                    .SetName($"Precision equals scale should throw (onlyPositive={onlyPositive})");
                yield return new TestCaseData(17, -1, onlyPositive)
                    .SetName($"Negative scale should throw (onlyPositive={onlyPositive})");
                yield return new TestCaseData(17, 18, onlyPositive)
                    .SetName($"Scale greater than precision should throw (onlyPositive={onlyPositive})");
            }
        }
    }

    [Test, TestCaseSource(typeof(TestData), nameof(TestData.InvalidConstructorParameters))]
    public void Constructor_Should_ReturnThrow_WhenInvalidParameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().Throw<ArgumentException>();
    }

    [TestCase(10, 5, true, TestName = "Valid positive number")]
    public void ConstructorShould_ReturnNotThrow_WhenValidParameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().NotThrow();
    }

    [Test]
    [TestCase(17, 2, true, "+123.45", TestName = "Valid positive number with plus sign")]
    [TestCase(17, 2, false, "-123.45", TestName = "Valid negative number with minus sign")]
    [TestCase(17, 2, false, "123,45", TestName = "Valid number with with ',' between integer and scale parts")]
    [TestCase(17, 0, true, "123", TestName = "Valid integer without scale part")]
    [TestCase(17, 2, true, "0000123.45", TestName = "Valid number with leading zeros")]
    [TestCase(int.MaxValue, 2, true, "2147483647.22", TestName = " Max int precision with scale part")]
    public void IsValidNumberShould_ReturnTrue_WhenValidNumbers(int precision, int scale, bool onlyPositive,
        string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        var actualResult = validator.IsValidNumber(value);

        actualResult.Should().BeTrue();
    }

    [Test]
    [TestCase(17, 2, true, "1.2.3", TestName = "multiple dots")]
    [TestCase(17, 2, true, "--123", TestName = "double minus")]
    [TestCase(17, 2, true, "123..45", TestName = "double dots")]
    [TestCase(17, 2, true, "12-3", TestName = "minus inside number")]
    [TestCase(17, 2, true, "+-123", TestName = "mixed signs")]
    [TestCase(17, 2, true, "12a.3", TestName = "Invalid character in number")]
    [TestCase(17, 2, true, "\n1.23", TestName = "Special character before")]
    [TestCase(17, 2, true, "1234.", TestName = "Trailing dot without fractional part")]
    [TestCase(17, 2, true, ".12", TestName = "Leading dot without integer part")]
    [TestCase(17, 2, true, "-123", TestName = "Negative number with onlyPositive=true")]
    public void IsValidNumberShould_ReturnFalse_WhenInvalidNumbers(int precision, int scale, bool onlyPositive,
        string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        var actualResult = validator.IsValidNumber(value);

        actualResult.Should().BeFalse();
    }
}