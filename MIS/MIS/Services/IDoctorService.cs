using Microsoft.EntityFrameworkCore;
using MIS.Infrastucture;
using MIS.Models.DB;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IDoctorService
    {
        Task<ResponseModel> register(DoctorRegisterModel doctor);

        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);

        bool logout();

        DoctorModel getProfile();

        bool editProfile(DoctorEditModel doctorEdit);
    }
    public class DoctorService : IDoctorService
    {
        private readonly MisDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public DoctorService(MisDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        // регистрация доктора
        public async Task<ResponseModel> register(DoctorRegisterModel doctor)
        {
            var hashedPassword = _passwordHasher.Generate(doctor.password);

            // проверка существует ли уже доктор с такой почтой или эмаилом
            var exists = await _context.Doctors
                                .AsNoTracking()
                                .AnyAsync(d => d.email == doctor.email || d.phone == doctor.phone);

            if (exists)
            {
                return new ResponseModel { status = "400", message = "this email or phone number already registered" };
            }

            DbDoctor newDoctor = new DbDoctor
            {
                birthday = doctor.birthday,
                createTime = DateTime.Now,

                email = doctor.email,
                gender = doctor.gender,
                name = doctor.name,
                phone = doctor.phone,
                passwordHash = hashedPassword
            };

            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();

            return new ResponseModel { status = "200", message = "Success" };
        }

        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.email == loginCredentials.email);

            if (doctor == null)
            {
                return null;
            }

            var result = _passwordHasher.Verify(loginCredentials.password, doctor.passwordHash);

            if (!result)
            {
                return null;
            }
            else
            {
                return _jwtProvider.GenerateToken(doctor);
            }

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
