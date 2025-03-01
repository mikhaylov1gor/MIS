namespace MIS.Models.DTO
{
    public class SpecialtiesPagedListModel
    {
        public List<SpecialtyModel>? specialties { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
