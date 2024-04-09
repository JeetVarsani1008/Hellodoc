//using BLL.Interface;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace HelloDoc2.Auth
//{
//    public class RoleAuthorize : Attribute, IAuthorizationFilter
//    {

//    }
//}



//using HalloDoc_mvc.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DAL.Models;

namespace HelloDoc2.Auth
{
    [AttributeUsage(AttributeTargets.All)]
    public class RoleAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly int _menuId;
        private readonly HellodocContext _context = new HellodocContext();

        public RoleAuthorize(int menuId = 0)
        {
            _menuId = menuId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var roleCookie = request.Cookies["RoleMenu"];

            var Role = _context.RoleMenus.Where(u => u.RoleId == Int32.Parse(roleCookie!)).ToList();
            bool flag = false;

            if (Role.Any(u => u.MenuId == _menuId))
            {
                flag = true;
            }

            if (flag == false)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "AccessDenied" }));
                return;
            }

        }

    }
}
