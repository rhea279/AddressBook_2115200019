using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IAuthService
    {
        Task<string> Register(UserDTO userDto);
        Task<object?> Login(UserDTO userDto);
    }
}
