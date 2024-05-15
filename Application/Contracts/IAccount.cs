using Application.DTOs.Request.Account;
using Application.DTOs.Request;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;

namespace Application.Contracts
{
    public interface IAccount
    {
        Task CreateAdmin();
        Task<GeneralResponseDTO> CreateAccountAsyc(CreateDTO model);
        Task<LoginResponseDTO> LoginAccountAsync(LoginDTO model);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenDTO model);

        Task<GeneralResponseDTO> CreateRoleAsync(CreateRoleDTO model);
        Task <IEnumerable<GetRoleDTO>> GetRolesAsync();
        Task <IEnumerable<GetUsersWithRolesResponseDTO>>GetUsersWithRolesAsync();
        Task ChangeUserRoleAsync(ChangeUserRoleRequestDTO model);

    }
}
