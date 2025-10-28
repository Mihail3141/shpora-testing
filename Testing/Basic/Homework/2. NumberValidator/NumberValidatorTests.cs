using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1, 2, true)]
    [TestCase(17, -1, true)]
    [TestCase(17, 18, true)]
    public void Constructor_Should_Throw_For_InvalidParameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().Throw();
    }

    [TestCase(5, 2, false)]
    [TestCase(10, 5, true)]
    [TestCase(10, 0, false)]
    public void Constructor_Should_NotThrow_For_ValidParameters(int precision, int scale, bool onlyPositive)
    {
        var action = () => new NumberValidator(precision, scale, onlyPositive);

        action.Should().NotThrow();
    }

    [TestCase(17, 2, true, null, false)]
    [TestCase(17, 2, true, "", false)]
    [TestCase(17, 2, true, "a.1d", false)]
    [TestCase(2, 1, true, "+0.0", false)]
    [TestCase(2, 1, true, "00.0", false)]
    [TestCase(3, 1, true, "0.00", false)]
    [TestCase(17, 2, true, "-0.0", false)]
    [TestCase(17, 2, true, "123.45", true)]
    [TestCase(17, 2, true, "123,45", true)]
    [TestCase(17, 2, true, "+0.0", true)]
    [TestCase(17, 2, false, "-0.0", true)]
    [TestCase(17, 0, true, "0", true)]
    [TestCase(17, 0, false, "-10", true)]
    public void IsValidNumber_Should_ReturnExpectedResults(int precision, int scale, bool onlyPositive, string value,
        bool expectedResult)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        var actualResult = validator.IsValidNumber(value);

        actualResult.Should().Be(expectedResult);
    }
}