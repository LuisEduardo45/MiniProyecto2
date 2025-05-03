using Services.Dtos;

namespace Services.Common
{
    public class MProfile : AutoMapper.Profile
    {
        public MProfile()
        {
            MilkMapper();
        }

        private void MilkMapper()
        {
            CreateMap<Domain.Milk, MilkModel>()
              .ReverseMap();
        }
    }
}
