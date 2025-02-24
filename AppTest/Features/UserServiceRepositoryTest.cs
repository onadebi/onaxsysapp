using AppGlobal.Models;
using AppGlobal.Services;
using OnaxTools.Dto.Http;

namespace AppTest.Features;

public class UserServiceRepositoryTest
{
    private readonly Mock<IUserProfileService> _userProfileServiceMock;
    private readonly IUserProfileService _handler;
    public static TheoryData<UserLoginDto> _sampleUserLoginDto = new()
    {
        {
            new UserLoginDto(){
                Email ="test.user@onaxsys.com",
                Password = "Tester1234"
            }
        }
    };
    public UserServiceRepositoryTest()
    {
        _userProfileServiceMock = new Mock<IUserProfileService>();
        _handler = _userProfileServiceMock.Object;
    }

    [Theory, MemberData(nameof(UserServiceRepositoryTest._sampleUserLoginDto), MemberType = typeof(UserServiceRepositoryTest))]
    public async Task LoginTest(UserLoginDto userLoginDto)
    {
        // Arrange
        _userProfileServiceMock.Setup(s => s.Login(userLoginDto,"",CancellationToken.None)).ReturnsAsync(() =>
        {
            return new GenResponse<UserLoginResponse>
            {
                IsSuccess = true,
                Result = new UserLoginResponse
                {
                    Email = userLoginDto.Email,
                    FirstName = userLoginDto.Email.Split("@")[0],
                    LastName = userLoginDto.Email.Split("@")[1],
                    token = Guid.NewGuid().ToString()
                }
            };
        });

        // Act
        var objResp = await _handler.Login(userLoginDto,"", CancellationToken.None);
        // Assert
        Assert.NotNull(objResp.Result);
    }
}
