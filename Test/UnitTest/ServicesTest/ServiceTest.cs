using AutoMapper;
using Domain;
using Infrastructure.Repositories.IRepositories;
using Moq;
using Services.Common;
using Services.Services;

namespace UnitTest;

public class ServiceTest
{
    Service service;
    Mock<IRepository> repository;
    List<Milk> returnList;
    IMapper mapper; // ✅ Added field declaration for mapper

    [SetUp]
    public void Setup()
    {
        var myProfile = new MProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        mapper = new Mapper(configuration); // ✅ Assigned to the class-level field

        repository = new Mock<IRepository>();
        returnList = new List<Milk>();
        returnList.Add(new Milk() { Id = Guid.NewGuid(), Liters = 10, RecolectionDate = DateTime.Now });

        repository.Setup(x => x.GetAllMilks()).ReturnsAsync(returnList);

        service = new Service(repository.Object, mapper);
    }

    [Test]
    public async Task Service_CanGetMilkList()
    {
        var returnListt = await service.GetAllMilks();
        var mappedList = mapper.Map<List<Milk>>(returnListt); // ✅ Use the 'mapper' field here
        Assert.That(mappedList.Count, Is.EqualTo(1));
    }
} // ✅ Added missing closing brace for the class
