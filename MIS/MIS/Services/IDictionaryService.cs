using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IDictionaryService
    {
        SpecialtiesPagedListModel getList(
                [FromQuery] string name,
                [FromQuery] int page,
                [FromQuery] int size);

        Icd10SearchModel getIcd10(
                [FromQuery] string request,
                [FromQuery] int page,
                [FromQuery] int size);

        Icd10RecordModel[] getIcdList();
    }
    public class DictionaryService : IDictionaryService
    {
        public SpecialtiesPagedListModel getList(
                [FromQuery] string name,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            return null;
        }

        public Icd10SearchModel getIcd10(
                [FromQuery] string request,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            return null;
        }

        public Icd10RecordModel[] getIcdList()
        {
            return null;
        }

    }
}
