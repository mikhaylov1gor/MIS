using MIS.Models.DB;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IDoctorService
    {
        Task register(DoctorRegisterModel doctor);

        Task login(LoginCredentialsModel loginCredentials);

        bool logout();

        DoctorModel getProfile();

        bool editProfile(DoctorEditModel doctorEdit);
    }
    public class DoctorService : IDoctorService
    {
        private readonly MisDbContext _context;

        public DoctorService(MisDbContext context)
        {
            _context = context;
        }

        // регистрация доктора
        public async Task register(DoctorRegisterModel doctor)
        {
            DbDoctor newDoctor = new DbDoctor{ birthday = doctor.birthday, createTime = DateTime.Now, 
                                               email = doctor.email, gender = doctor.gender, name = doctor.name, phone = doctor.phone};

            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();
        }

        public async Task login(LoginCredentialsModel loginCredentials)
        {
            await _context.SaveChangesAsync();
        }

        public bool logout()
        {
            return true;
        }

        public DoctorModel getProfile()
        {
            return null;
        }

        public bool editProfile(DoctorEditModel doctorEdit)
        {
            return true;
        }
    }
}
