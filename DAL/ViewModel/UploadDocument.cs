using Microsoft.AspNetCore.Http;

namespace DAL.ViewModel
{
    public class UploadDocument
    {
        public int ReqId { get; set; }
        public IFormFile File { get; set; }

    }
}
