using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MIS.Infrastucture;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MIS.Services
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> register(DoctorRegisterModel doctor);

        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);

        Task<ActionResult> logout(string token, ClaimsPrincipal user);

        Task<DoctorModel> getProfile(ClaimsPrincipal doctor);

        Task<ActionResult> editProfile(DoctorEditModel doctorEdit, ClaimsPrincipal user);
    }
    public class DoctorService : IDoctorService
    {
        private readonly MisDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly ITokenBlackListService _tokenBlackListService;

        public DoctorService(
            MisDbContext context, 
            IPasswordHasher passwordHasher, 
            IJwtProvider jwtProvider, 
            ITokenBlackListService tokenBlackListService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _tokenBlackListService = tokenBlackListService;
        }

        // регистрация доктора
        public async Task<TokenResponseModel> register(DoctorRegisterModel doctor)
        {
            var hashedPassword = _passwordHasher.Generate(doctor.password);

            // проверка существует ли уже доктор с такой почтой или телефоном
            var exists = await _context.Doctors
                                .AsNoTracking()
                                .AnyAsync(d => d.email == doctor.email || d.phone == doctor.phone);

            if (exists)
                throw new ValidationAccessException("Email Or Phone Already Registered"); //ex

            // проверка существует ли такая специализация
            exists = await _context.Specialties
                .AsNoTracking()
                .AnyAsync(i => i.id == doctor.speciality);

            if (!exists)
                throw new ValidationAccessException("Wrong Specialty"); // ex

            DbDoctor newDoctor = new DbDoctor
            {
                birthday = doctor.birthday,
                createTime = DateTime.UtcNow,

                email = doctor.email,
                gender = doctor.gender,
                name = doctor.name,
                phone = doctor.phone,
                passwordHash = hashedPassword,
                specialtyId = doctor.speciality
            };

            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();

            return _jwtProvider.GenerateToken(newDoctor);
        }

        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.email == loginCredentials.email);

            if (doctor == null)
            {
                throw new ValidationAccessException("Wrong Email Or Password"); // ex
            }

            var result = _passwordHasher.Verify(loginCredentials.password, doctor.passwordHash);

            if (!result)
            {
                throw new ValidationAccessException("Wrong Email Or Password"); // ex
            }
            else
            {
                return _jwtProvider.GenerateToken(doctor);
            }
        }

        public async Task<ActionResult> logout(string token, ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); // ex
            }

            if (!string.IsNullOrEmpty(token))
            { 
                await _tokenBlackListService.AddTokenToBlackList(token);
                return null;
            }
            else
            {
                throw new UnauthorizedAccessException(); // ex
            }
        }

        public async Task<DoctorModel> getProfile(ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); // ex
            }

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.id == parsedId);

            // проверка на наличие профиля
            if (doctor == null)
            {
                throw new KeyNotFoundException("doctor not found");  //ex
            }

            return new DoctorModel
            {
                id = doctor.id,
                createTime = doctor.createTime,
                name = doctor.name,
                birthday = doctor.birthday,
                gender = doctor.gender,
                email = doctor.email,
                phone = doctor.phone
            };
        }

        public async Task<ActionResult> editProfile(DoctorEditModel doctorEdit, ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); // ex
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.id == parsedId);

            // проверка на наличие профиля
            if (doctor == null)
                throw new KeyNotFoundException("doctor not found");  //ex

            if (doctorEdit.birthday > DateTime.UtcNow)
                throw new ValidationAccessException(""); // ex

            // проверка существует ли уже доктор с такой почтой или телефоном
            var exists = await _context.Doctors
                                .AsNoTracking()
                                .AnyAsync(d => d.email == doctorEdit.email || d.phone == doctorEdit.phone);

            if (exists && doctorEdit.email != doctor.email && doctorEdit.phone != doctor.phone)
            {
                throw new ValidationAccessException("this email or phone already exists");
            }

            doctor.email = doctorEdit.email;
            doctor.name = doctorEdit.name;
            doctor.birthday = doctorEdit.birthday;
            doctor.gender = doctorEdit.gender;
            doctor.phone = doctorEdit.phone;

            await _context.SaveChangesAsync();
            return null;
        }
    }
}
