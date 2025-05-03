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
    Mock<IMapper> mapper;
    Mock<IRepository> repository;
    List<Milk> returnList;

    [SetUp]
    public void Setup()
    {
        //se usa al profile existente
        var myProfile = new MProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);
        //se crea un mock de las clases inyectadas
        repository = new Mock<IRepository>();
        //se configura que se devuelva lo que se requiere para la prueba
        returnList = new List<Milk>();
        returnList.Add(new Milk() { Id = Guid.NewGuid(), Liters = 10, RecolectionDate = DateTime.Now });
        repository.Setup(x => x.GetAllMilks()).ReturnsAsync(returnList);
        service = new Service(repository.Object, mapper);
    }

    [Test]
    public void Service_Exist()
    {
        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public async Task Service_CanGetMilkList()
    {
        var returnListt = await service.GetAllMilks();
        Assert.That(returnListt.Count, Is.EqualTo(1));
    }
}
