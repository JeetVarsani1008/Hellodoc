using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
	public class SchedulingDetailsViewModel
	{
		public int resourceId { get; set; }
		public int Id { get; set; }

		public string title { get; set; }
		public string start {  get; set; }

		public string end { get; set; }
		public string color { get; set; }
	}
}
