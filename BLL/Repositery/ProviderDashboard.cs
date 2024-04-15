﻿using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
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
                RequestId = x.RequestId,
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

        #region providerResetPassword
        public bool providerResetPassword(string password, int physicianId)
        {
            var aspId = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId).AspNetUserId;
            var passdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            passdata.PasswordHash = password;
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region createShiftPost
        public void createShiftPost(ViewShiftVm model, int aspId, List<int> WeekDaysList)
        {
            Shift shift = new Shift()
            {
                PhysicianId = model.PhysicianId,
                StartDate = model.ShiftDate,
                RepeatUpto = model.RepeatUpto,
                CreatedDate = DateTime.Now,
                CreatedBy = aspId,
                IsRepeat = model.IsRepeat,

            };
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            ShiftDetail shiftDetail = new ShiftDetail()
            {
                ShiftId = shift.ShiftId,
                ShiftDate = model.ShiftDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Status = 0,
                IsDeleted = false,
                RegionId = model.RegionId,

            };
            _context.ShiftDetails.Add(shiftDetail);
            _context.SaveChanges();


            for (int i = 0; i < model.RepeatUpto; i++)
            {
                DateOnly shiftDate = model.ShiftDate.AddDays(7 * i);
                foreach (DayOfWeek dow in WeekDaysList)
                {
                    ShiftDetail shiftDetail1 = new ShiftDetail();
                    shiftDetail1.ShiftId = shift.ShiftId;
                    shiftDetail1.ShiftDate = dow - shiftDate.DayOfWeek > 0 ? shiftDate.AddDays(dow - model.ShiftDate.DayOfWeek) : shiftDate.AddDays(7 + dow - model.ShiftDate.DayOfWeek);
                    shiftDetail1.RegionId = model.RegionId;
                    shiftDetail1.StartTime = model.StartTime;
                    shiftDetail1.EndTime = model.EndTime;
                    shiftDetail1.Status = 0;
                    shiftDetail1.IsDeleted = false;
                    _context.ShiftDetails.Add(shiftDetail1);
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region checkshiftExistsForPhysician
        public bool checkshiftExistsForPhysician(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime)
        {
            var data = _context.ShiftDetails.Any(x => x.Shift.PhysicianId == physicianId && (x.ShiftDate == shiftdate) && (x.StartTime <= starttime && x.EndTime >= starttime));

            if (data)
            {
                return true;
            }
            return false;
        }
        #endregion


        #region editViewNotes
        public void editViewNotes(ViewNotesVm model, int requestId)
        {
            var reqnotes = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            var req = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            if (reqnotes != null)
            {
                reqnotes.PhysicianNotes = model.PhysicianNotes;
                reqnotes.ModifiedBy = (int)_context.Users.FirstOrDefault(x => x.UserId == req.UserId).AspNetUserId;
                reqnotes.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

        }
        #endregion
    }
}