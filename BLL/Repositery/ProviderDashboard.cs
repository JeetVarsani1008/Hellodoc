using BLL.Interface;
using DAL.Models;
using DAL.ViewModelProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositery
{
    public class ProviderDashboard : IProviderDashboard
    {
        private readonly HellodocContext _context;
        public ProviderDashboard(HellodocContext context)
        {
            _context = context;
        }


        #region getRequestDataForProvider
        public List<RequestDataProvider> getRequestDataForProvider(string statusarray, int reqTypeId, string searchdata)
        {
            var list = _context.Requests.Where(o => statusarray.Contains(o.Status.ToString()) && o.PhysicianId.HasValue);

            if (searchdata != null)
            {
                searchdata = searchdata.ToLower();
                list = list.Where(x => x.FirstName.ToLower().Contains(searchdata));
            }
            if (reqTypeId != 0 )
            {
                list = list.Where(o => o.RequestTypeId == reqTypeId);
            }

            var data = list.Select(x => new RequestDataProvider()
            {
                
                RequestTypeId = x.RequestTypeId,
                Name = x.RequestClients.Select(j => j.FirstName).First() + " " + x.RequestClients.Select(h => h.LastName).First(),
                Phone = x.PhoneNumber,
                Address = x.RequestClients.Select(x => x.Street).First() + "," + x.RequestClients.Select(x => x.City).First() + "," + x.RequestClients.Select(x => x.State).First(),
                Status = "1",
                StatusForDash = x.Status,
            }).ToList();

            return data;
        }
        #endregion

        #region forCountRequest
        public IQueryable<Request> forCountRequest => _context.Requests.Where(x => x.PhysicianId.HasValue);
        #endregion

        #region getProviderDetails : for my profile
        public ProviderProfileVm getProviderDetails(int id, int physicianId)
        {
            List<Region> region = _context.Regions.ToList();    
            List<PhysicianRegion> physicianRegion = _context.PhysicianRegions.Where(x => x.PhysicianId == physicianId).ToList();

            var phy = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);
            var aspdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            ProviderProfileVm providerProfileVm = new ProviderProfileVm
            {
                PhysicianId = physicianId,
                UserName = aspdata.UserName,
                FirstName = phy.FirstName,
                LastName = phy.LastName,
                Email = phy.Email,
                Mobile = phy.Mobile,
                MedicalLicense = phy.MedicalLicense,
                NPINumber = phy.Npinumber,
                Address1 = phy.Address1,
                Address2 = phy.Address2,
                City = phy.City,
                State = _context.Regions.FirstOrDefault(x => x.RegionId == phy.RegionId).Name,
                Zip = phy.Zip,
                AlternateMobile = phy.AltPhone,
                BusinessName = phy.BusinessName,    
                BusinessWebSite = phy.BusinessWebsite,
                Photo = phy.Photo,
                Signature = phy.Signature,
                regions = region,
                PhysicianRegions = physicianRegion,
            };
            return providerProfileVm;
        }
        #endregion

        #region getParticularScheduleData
        public List<ShiftDetail> getParticularScheduleData(int PhysicianId)
        {
            try
            {
                return _context.ShiftDetails.Include(m => m.Shift).ThenInclude(j => j.Physician).Where(m => m.IsDeleted == false && m.Shift.PhysicianId == PhysicianId).ToList();
            }
            catch { return new List<ShiftDetail> { }; }
        }
        #endregion

        #region getRegions 
        public List<Region> getRegions()
        {
            var data = _context.Regions.ToList();
            return data;
        }
        #endregion
    }
}
