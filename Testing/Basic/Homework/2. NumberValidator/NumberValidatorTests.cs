using NUnit.Framework;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
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

    [Test, TestCaseSource(nameof(InvalidConstructorParameters))]
    public void Constructor_Should_Throw_ArgumentException_When_Invalid_Parameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().Throw<ArgumentException>();
    }

    [TestCase(10, 5, true, TestName = "Valid positive number")]
    public void Constructor_Should_NotThrow_When_Valid_Parameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().NotThrow();
    }

    [TestCase(17, 2, true, "+123.45", TestName = "Valid positive number with plus sign")]
    [TestCase(17, 2, false, "-123.45", TestName = "Valid negative number with minus sign")]
    [TestCase(17, 2, false, "123,45", TestName = "Valid number with with ',' between integer and scale parts")]
    [TestCase(17, 0, true, "123", TestName = "Valid integer without scale part")]
    [TestCase(17, 2, true, "0000123.45", TestName = "Valid number with leading zeros")]
    [TestCase(17, 2, false, "-0", TestName = "Negative zero with onlyPositive=false")]
    [TestCase(2, 0, false, "+0", TestName = "Positive zero with onlyPositive=false")]
    public void IsValidNumber_Should_Return_True_When_Valid_Numbers(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        var actualResult = validator.IsValidNumber(value);

        actualResult.Should().BeTrue();
    }

    [TestCase(17, 2, true, "1.2.3", TestName = "multiple dots")]
    [TestCase(17, 2, true, "--123", TestName = "double minus")]
    [TestCase(17, 2, true, "++123", TestName = "double plus")]
    [TestCase(17, 2, false, "123..45", TestName = "double dots")]
    [TestCase(17, 2, true, "12-3", TestName = "minus inside number")]
    [TestCase(17, 2, true, "+-123", TestName = "mixed signs")]
    [TestCase(17, 2, false, "12a.3", TestName = "Invalid character in number")]
    [TestCase(17, 2, false, "\n1.23", TestName = "Special character before")]
    [TestCase(17, 2, true, "1234.", TestName = "Trailing dot without fractional part")]
    [TestCase(17, 2, false, ".12", TestName = "Leading dot without integer part")]
    [TestCase(17, 2, true, "-123", TestName = "Negative number with onlyPositive=true")]
    [TestCase(17, 0, true, "-0", TestName = "Negative zero with onlyPositive=true")]
    [TestCase(1, 0, false, "+0", TestName = "Positive zero with precision 1")]
    [TestCase(1, 0, false, "", TestName = "Empty string")]
    [TestCase(1, 0, false, null, TestName = "Null string")]
    [TestCase(3, 0, false, "a", TestName = "Not a number")]
    [TestCase(3, 0, false, " 1", TestName = "Space before the number")]
    [TestCase(3, 0, false, "1 ", TestName = "Space after the number")]
    [TestCase(3, 0, false, "1 1", TestName = "Space inside the number")]
    public void IsValidNumber_Should_Return_False_When_Invalid_Numbers(int precision, int scale, bool onlyPositive, string value)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        var actualResult = validator.IsValidNumber(value);

        actualResult.Should().BeFalse();
    }
}