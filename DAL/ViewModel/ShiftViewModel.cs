using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
	public class ShiftViewModel
	{
		public int Id { get; set; }
		public string title { get; set; }

		public string imageUrl { get; set; }

		public List<Region> regions {  get; set; } 

		public List<RequestedShift> requestedShift {  get; set; }
	}

	public class RequestedShift
	{
		public int ShiftDetailId { get; set; }
		public string Staff { get; set; }

		public DateOnly Day { get; set; }

		public string Time { get; set; }

		public string Region { get; set; }

	}
}
