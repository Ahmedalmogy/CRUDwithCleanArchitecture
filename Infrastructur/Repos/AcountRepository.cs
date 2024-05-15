using Domain.Entity.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Application.Contracts;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Mapster;
using Application.Extensions;



namespace Infrastructur.Repos
{
    public class AcountRepository(RoleManager<IdentityRole> rolemanager, UserManager<ApplicationUser> usermanager,
        IConfiguration config, SignInManager<ApplicationUser> signInManager) : IAccount
    {
        private async Task <ApplicationUser>   FindUserByEmailAsync(string email) 
            => await usermanager.FindByEmailAsync(email);
        private async Task<IdentityRole> FindRoleByNameAsync(string name)
            => await rolemanager.FindByNameAsync(name);

        private static String GenerateRefreshToken() =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        private async Task<string> GenerateToken(ApplicationUser user) 
        {
            try

            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:key"]!));
                var userClaims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role,(await usermanager.GetRolesAsync(user)).FirstOrDefault().ToString()),
                    new Claim ("Fullname",user.Name)
                };
                var credentials =  new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken
                (
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: userClaims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            catch 
            {
            return null!;
            }
        }
        private async Task<GeneralResponseDTO> AssignUserToRole(ApplicationUser user, IdentityRole role) 
        {
            if (user is null || role is null) return new GeneralResponseDTO(false, "Model state can not be Empty");
            if (await FindRoleByNameAsync(role.Name) == null)
                await CreateRoleAsync(role.Adapt(new CreateRoleDTO()));
              IdentityResult result = await usermanager.AddToRoleAsync(user, role.Name);

            string error = CheckResponse(result);

            if (!string .IsNullOrEmpty(error)) 
                    return new GeneralResponseDTO(false, error);
                else
             return new GeneralResponseDTO(true, "User Added Successfully");
        
        }

        private static string CheckResponse(IdentityResult result)
        {

            if (!result.Succeeded) 
            { 
                var errors = result.Errors.Select(e => e.Description);
                return string.Join(Environment.NewLine, errors);
            }
            return null; 
        }
        public Task ChangeUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponseDTO> CreateAccountAsyc(CreateDTO model)
        {


            try

            {
                if (await FindUserByEmailAsync(model.Email) != null)
                    return new GeneralResponseDTO(false, "sorry user is already exist");
                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = model.Password
                };

                var result = await usermanager.CreateAsync(user, model.Password);
                string error = CheckResponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponseDTO(false, error);
                var (flag, message) = await AssignUserToRole(user, new IdentityRole() { Name = model.Role });

                return new GeneralResponseDTO(false, message);

            }
            catch (Exception ex) { return new GeneralResponseDTO(false, ex.Message); }

        }

        public async Task CreateAdmin()
        {
            try
            { 
                if(await FindRoleByNameAsync(Constant.Role.Admin) != null) return;
                var admin = new CreateDTO()
                {
                    Name = "Admin",
                    Email = "admin@123",
                    Password = "admin@admin.com",
                    Role = Constant.Role.Admin
                };
                await CreateAccountAsyc(admin);
            }
            catch
            {
                
            }
        }

        public Task<GeneralResponseDTO> CreateRoleAsync(CreateRoleDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetRoleDTO>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDTO> LoginAccountAsync(LoginDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
