using DAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IJWT
    {
        string GenerateJwtToken(AspNetUserRole userRole);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

    }
}
