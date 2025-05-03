using Domain;

namespace UnitTest.DomainTest
{
    public class MilkTests
    {
        Milk milk;

        [SetUp]
        public void Setup()
        {
            milk = new Milk();
            milk.Liters = 100;
            milk.RecolectionDate = DateTime.Now;
        }

        [Test]
        public void Milk_Exist()
        {
            Assert.That(milk, Is.Not.Null);
        }

        [Test]
        public void Milk_CanSetLiters()
        {
            milk.Liters = 200;
            Assert.That(milk.Liters, Is.EqualTo(200));
        }
    }
}
