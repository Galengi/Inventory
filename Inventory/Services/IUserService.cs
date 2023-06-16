using Inventory.Models.Request;
using Inventory.Models.Response;

namespace Inventory.Services
{
    public interface IUserService
    {
        UserResponse Auth(AuthRequest model);
    }
}
