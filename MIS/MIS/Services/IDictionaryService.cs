using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IDictionaryService
    {
        Task<SpecialtiesPagedListModel> getList(
                [FromQuery] string name,
                [FromQuery] int page,
                [FromQuery] int size);

        Task<Icd10SearchModel> getIcd10(
                [FromQuery] string request,
                [FromQuery] int page,
                [FromQuery] int size);

        Task<List<Icd10RecordModel>> getIcdRoots();
    }
    public class DictionaryService : IDictionaryService
    {
        private readonly MisDbContext _context;

        public DictionaryService(MisDbContext context)
        {
            _context = context;
        }

        public async Task<SpecialtiesPagedListModel> getList(
                [FromQuery] string name,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            if (page < 0 || size < 0)
            {
                throw new ValidationAccessException(); // ex
            }

            var totalItems = await _context.Specialties
                .Where(s => EF.Functions.Like(s.name, $"%{name}%"))
                .CountAsync();

            var specialties = await _context.Specialties
                .Where(s => EF.Functions.Like(s.name, $"%{name}%"))
                .Skip((page - 1) * size) 
                .Take(size)             
                .Select(s => new SpecialtyModel { id = s.id, createTime = s.createTime, name = s.name })  
                .ToListAsync();

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };

            return new SpecialtiesPagedListModel
            {
                specialties = specialties,
                pagination = pageInfo
            };

        }

        public async Task<Icd10SearchModel> getIcd10(
                [FromQuery] string request,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            if (page < 0 || size < 0)
            {
                throw new ValidationAccessException();//ex
            }

            var query = _context.Icd10
                .Where(i => EF.Functions.Like(i.code, $"%{request}%") || EF.Functions.Like(i.name, $"%{request}%"));

            var totalItems = await query.CountAsync();

            var records = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new Icd10RecordModel { id = i.id, createTime = i.createTime, name = i.name, code = i.code })
                .ToListAsync();

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };

            return new Icd10SearchModel
            {
                records = records,
                pagination = pageInfo
            };
        }

        public async Task<List<Icd10RecordModel>> getIcdRoots()
        {
            var dbList = _context.Icd10
                    .Where(i => i.parentId == null);

            var dtoList = await dbList
                .Select(i => new Icd10RecordModel { id = i.id, createTime = i.createTime, name = i.name, code = i.code })
                .ToListAsync();

            return dtoList;
        }

    }
}
