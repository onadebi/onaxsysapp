using AppGlobal.Models.SocialAuth;

namespace AppCore.Config;

public class AutoMapperProfile : AutoMapper.Profile
{
    public AutoMapperProfile()
    {
        //CreateMap<UserCourseGeneratedOutputDTO, CourseGenerationPromptOutputDTO>().ReverseMap();
        CreateMap<GoogleOAuthResponse,Google.Apis.Auth.GoogleJsonWebSignature.Payload>().ReverseMap();
    }
}
