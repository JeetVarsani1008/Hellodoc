using DAL.Models;
using System;
using System.Collections.Generic;
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
        public int RoleId { get; set; }
        public string AccountType {  get; set; }

        public string AccountPOC {  get; set; }

        public string Phone {  get; set; }
        public int Status { get; set; }

        public string OpenRequest { get; set; }

    }
}
