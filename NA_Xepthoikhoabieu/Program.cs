using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NA_Entities.DBContext;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Authorization;
using NA_Xepthoikhoabieu.Mapping;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Đăng ký AutoMapper trong DI container
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Add services for DB Connection
builder.Services.AddDbContext<NA_DbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Repository
//builder.Services.AddScoped<IAuthenRepository, AuthenRepository>();
builder.Services.AddScoped<IPasswordHasherRepository, PasswordHasherRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDM_CaphocRepository, DM_CaphocRepository>();
builder.Services.AddScoped<IDM_CahocRepository, DM_CahocRepository>();
builder.Services.AddScoped<IClaimHelperRepository, ClaimHelperRepository>();
builder.Services.AddScoped<IDM_DonviRepository, DM_DonviRepository>();
builder.Services.AddScoped<IAuth_MenusRepository, Auth_MenusRepository>();
builder.Services.AddScoped<IAuth_RolesRepository, Auth_RolesRepository>();
builder.Services.AddScoped<IDM_DiemtruongRepository, DM_DiemtruongRepository>();
builder.Services.AddScoped<IDM_LoaiphonghocRepository, DM_LoaiphonghocRepository>();
builder.Services.AddScoped<IDM_KhoikienthucRepository, DM_KhoikienthucRepository>();
builder.Services.AddScoped<IDM_TochuyenmonRepository, DM_TochuyenmonRepository>();
builder.Services.AddScoped<IDM_KhoilopRepository, DM_KhoilopRepository>();
builder.Services.AddScoped<IDM_PhonghocRepository, DM_PhonghocRepository>();
builder.Services.AddScoped<IDM_TiethocRepository, DM_TiethocRepository>();
builder.Services.AddScoped<IDM_NgayhocRepository, DM_NgayhocRepository>();
builder.Services.AddScoped<IDM_MonhocRepository, DM_MonhocRepository>();
builder.Services.AddScoped<INgay_DonviRepository, Ngay_DonviRepository>();
builder.Services.AddScoped<ITiet_co_dinhRepository, Tiet_co_dinhRepository>();
builder.Services.AddScoped<IDM_BanhocRepository,DM_BanhocRepository>();
builder.Services.AddScoped<ITo_hop_monRepository, To_hop_monRepository>();
builder.Services.AddScoped<IMon_KhoiRepository, Mon_KhoiRepository>();
builder.Services.AddScoped<IDM_GiaovienRepository,DM_GiaovienRepository>();
builder.Services.AddScoped<IDM_LophocRepository,DM_LophocRepository>();
builder.Services.AddScoped<ILop_MonRepository,Lop_MonRepository>();
builder.Services.AddScoped<IDanhsach_ThoikhoabieuRepository, ThoiKhoaBieuRepository>();
builder.Services.AddScoped<IObjectRepository, ObjectRepository>();
builder.Services.AddScoped<ISoTietDanhMucRepository, SoTietDanhMucRepository>();
builder.Services.AddScoped<IExportExcelRepository, ExportExcelRepository>();
builder.Services.AddScoped<ICheckTenRepository, CheckTenRepository>();
builder.Services.AddScoped<IDM_TinhRepository, DM_TinhRepository>();
// Đọc cấu hình từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Không tồn tại key"));

// Cấu hình JwtHelperRepository vào DI container
builder.Services.AddSingleton<IJwtHelperRepository, JwtHelperRepository>();

// Thêm Authorization
builder.Services.AddAuthorization();

// Thêm Controller
builder.Services.AddControllers();

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Swagger để hỗ trợ JWT Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Phần mềm Xếp thời khóa biểu", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào ô bên dưới. Ví dụ: Bearer {your_token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Cấu hình Middleware
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Đặt đường dẫn đến Swagger JSON, đường dẫn này cần phải đúng với Swagger JSON endpoint
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    // Thay đổi đường dẫn truy cập Swagger UI
    //options.RoutePrefix = "api-nguyenanh";  // Đây là đường dẫn mới thay vì 'swagger/index.html'
});

// Bật CORS nếu cần
app.UseCors("AllowAll");

// Sử dụng JwtMiddleware thay vì UseAuthentication
app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization(); // Authorization vẫn phải có
app.MapControllers();

app.Run();
