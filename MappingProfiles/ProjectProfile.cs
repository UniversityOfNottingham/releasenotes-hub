using AutoMapper;

namespace releasenotes.MappingProfiles {
    public class ProjectProfile : Profile {
        public ProjectProfile()
        {
            CreateMap<Dtos.Project, Entities.Project>();
        }
    }
}