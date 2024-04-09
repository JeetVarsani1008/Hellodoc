using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AccessVm
    {
        public List<Access> access {  get; set; }
        public List<UserAccess> userAccess { get; set; }
        public List<AspNetRole> aspNetRole { get; set; }
        public List<Menu> menu { get; set; }

        public List<RoleMenu> roleMenu { get; set; }

        [Required(ErrorMessage = "Please Enter Role Name")]
        public string RoleName { get; set; }
        public int RoleId { get; set; }

        public int AccountType { get; set; }
    }

    public class Access
    {
        public int AccountTypeId { get; set; }
        public string Name { get; set; }
        public string AccountType { get; set; }
        public int RoleId { get; set; }
    }

    public class UserAccess
    {
        public int PhysicianId { get; set; }
        public int AdminId { get; set; }
        public int RoleId { get; set; }
        public string AccountType {  get; set; }

        public string AccountPOC {  get; set; }

        public string Phone {  get; set; }
        public int Status { get; set; }

        public string OpenRequest { get; set; }

    }
}
