using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class ChatViewModel
    {

        public int? ProviderId { get; set; } = 0;

        public int? CurrentAspNetUserId { get; set; }

        public int? ProviderAspNetUserId { get; set; }

        public int? AdminAspNetUserId { get; set; }

        public int? PatientAspNetUserId { get; set; }


        public int? RequestId { get; set; } = 0;

        public string? ProviderPhoto { get; set; } = null;

        public string? ProviderName { get; set; } = null;

        public int? AdminId { get; set; } = 0;

        public string? AdminName { get; set; } = null;

        public string? Message { get; set; } = null;

        public string? PatientName { get; set; } = null;

        public List<ChatViewModel> ListOfChats { get; set; }

        public DateTime? sentDate { get; set; }

        public int sentBy { get; set; }

        public bool IsSender { get; set; } = false;

    }
}
