using MIS.Models;

namespace MIS.Services
{
    public interface IInspectionService
    {
        InspectionModel getInspection(Guid id);

        bool editInspection(Guid id, InspectionEditModel inspectionEdit);

        InspectionPreviewModel[] getChainInspections(Guid id);

    }
    public class InspectionService : IInspectionService
    {
        public InspectionModel getInspection(Guid id)
        {
            return null;
        }

        public bool editInspection (Guid id, InspectionEditModel inspectionEdit)
        {
            return true;
        }

        public InspectionPreviewModel[] getChainInspections(Guid id)
        {
            return null;
        }
    }
}
