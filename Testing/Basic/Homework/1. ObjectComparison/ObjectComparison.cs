using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));
        
        actualTsar
            .Should()
            .BeEquivalentTo(expectedTsar, options => options
            .ExcludingMembersNamed("Id"));
        
        //Преимущества такой реализации:
        //-тест занимает меньше строк кода,
        //-добавление новых свойств не приводит к изменению теста (кроме самой инициализации expectedTsar),
        //-добавление вложенных объектов не приводит к изменению теста (кроме самой инициализации expectedTsar),
        //-явно указывается несовпадающее свойство.
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));
        
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
        
        //Какие недостатки у такого подхода? 
        //Недостатки:
        //-линейный рост количества строк кода при добавлении новых свойств,
        //-при добавлении новых свойст в класс их необходимо вносить в метод AreEqual,
        //-можно забыть внести проверку совпадения свойств в AreEqual, при этом тест будет выполняться,
        //-при падении теста явно не указывается свойство, из-за которого тест падает.
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
