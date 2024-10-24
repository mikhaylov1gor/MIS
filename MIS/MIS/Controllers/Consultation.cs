using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MIS.Models;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        [HttpGet]
        public List<ConsultationModel>? get()
        {
            List<ConsultationModel> consultationModels = new List<ConsultationModel>();
            return consultationModels;
        }
        [HttpGet("{id}")]
        public ConsultationModel getId(int id)
        {
            ConsultationModel consultationModel = new ConsultationModel();
            return consultationModel;
        }
        [HttpPost("{id}/comment")]
        public string postComment(int id, string comment)
        {
            return comment;
        }

        [HttpPut("/comment/{id}")]
        public string editComment(int id, string comment)
        {
            return comment;
        }

    }
}
