using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using MIS.Models.DB;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text.Json;
namespace MIS.Services
{
    public interface ISeedDataService
    {
        Task SeedSpecialties();
        Task SeedIcd10();
    }
    public class SeedDataService: ISeedDataService
    {
        private readonly MisDbContext _context;

        public SeedDataService(MisDbContext context)
        {
            _context = context;
        }

        // сидирование специальностей
        public async Task SeedSpecialties()
        {
            if (!await _context.Specialties.AnyAsync())
            {
                var json = await File.ReadAllTextAsync("Specialties.json");
                var specialties = JsonSerializer.Deserialize<List<DbSpecialty>>(json);

                if (specialties != null)
                {
                    foreach (var specialty in specialties) 
                    {
                        specialty.createTime = DateTime.UtcNow;
                    }
                    await _context.Specialties.AddRangeAsync(specialties);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // сидирование МКБ-10
        public async Task SeedIcd10()
        {
            if (!await _context.Icd10.AnyAsync())
            {
                var json = await File.ReadAllTextAsync("Icd10.json");
                var icd10Json = JsonSerializer.Deserialize<List<Icd10HelpSeed>>(json);

                if (icd10Json != null)
                {
                    foreach (var record in icd10Json)
                    {
                        if (record.ID_PARENT == null)
                        {
                            var dbIcd10 = new DbIcd10
                            {
                                createTime = DateTime.UtcNow,
                                code = addRec(record.MKB_CODE),
                                name = record.MKB_NAME,
                                recordCode = record.REC_CODE,
                                parentId = null
                            };

                            await _context.Icd10.AddAsync(dbIcd10);
                            await _context.SaveChangesAsync();
                        }
                    }

                    foreach (var record in icd10Json)
                    {
                        if (record.ID_PARENT != null) 
                        {
                            string prefix = record.REC_CODE.Substring(0, 2);

                            var parentId = await _context.Icd10
                                .Where(x => x.recordCode != null && x.recordCode.StartsWith(prefix) && x.parentId == null)
                                .Select(x => x.id)
                                .FirstOrDefaultAsync();

                            var dbIcd10 = new DbIcd10
                            {
                                createTime = DateTime.UtcNow,
                                code = record.MKB_CODE,
                                name = record.MKB_NAME,
                                recordCode = record.REC_CODE,
                                parentId = parentId
                            };

                            await _context.Icd10.AddAsync(dbIcd10);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private string addRec(string code)
        {
            {
                var codeMapping = new Dictionary<string, string>
    {
        { "I", "A00-B99" },
        { "II", "C00-D48" },
        { "III", "D50-D89" },
        { "IV", "E00-E90" },
        { "V", "F00-F99" },
        { "VI", "G00-G99" },
        { "VII", "H00-H59" },
        { "VIII", "H60-H95" },
        { "IX", "I00-I99" },
        { "X", "J00-J99" },
        { "XI", "K00-K93" },
        { "XII", "L00-L99" },
        { "XIII", "M00-M99" },
        { "XIV", "N00-N99" },
        { "XV", "O00-O99" },
        { "XVI", "P00-P96" },
        { "XVII", "Q00-Q99" },
        { "XVIII", "R00-R99" },
        { "XIX", "S00-T98" },
        { "XX", "V01-Y98" },
        { "XXI", "Z00-Z99" },
        { "XXII", "U00-U85" }
    };

                return codeMapping.TryGetValue(code, out var result) ? result : code;
            }
        }
    }
}

public class Icd10HelpSeed
{
    public int ID { get; set; }
    public string? REC_CODE {  get; set; }
    public string? MKB_CODE {  get; set; }
    public string? MKB_NAME { get; set; }
    public string? ID_PARENT { get; set; }
    public int ACTUAL {  get; set; }
}