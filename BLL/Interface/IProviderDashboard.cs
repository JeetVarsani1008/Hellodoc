﻿using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IProviderDashboard
    {
        ProviderProfileVm getProviderDetails(int id, int physicianId);
        List<RequestDataProvider> getRequestDataForProvider(string statusarray, int reqTypeId, string searchdata);

        IQueryable<Request> forCountRequest { get; }

        List<ShiftDetail> getParticularScheduleData(int PhysicianId);

        List<Region> getRegions();

        //for my profile 
        bool providerResetPassword(string password, int physicianId);
        void createShiftPost(ViewShiftVm model, int aspId, List<int> WeekDaysList);
        bool checkshiftExistsForPhysician(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime);

        void editViewNotes(ViewNotesVm model, int requestId);

    }
}