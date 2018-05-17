using AutoMapper;

namespace releasenotes.MappingProfiles {
    public class ReleaseProfile : Profile {
        public ReleaseProfile()
        {
            CreateMap<Dtos.Release, Entities.Release>();
        }
    }
}