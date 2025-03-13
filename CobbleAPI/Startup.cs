using CobbleAPI.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using CobbleAPI.Data;
using CobbleAPI.Service;

namespace CobbleAPI;

public class Startup
{
    /// <summary>
    /// 读取配置信息
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        // 获取连接字符串
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        // 打印连接字符串
        Console.WriteLine(connectionString);
    }


    public IConfiguration Configuration { get; }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        ///身份验证
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "LogonApp.Cookie";// 设置认证Cookie的名称
            options.Cookie.HttpOnly = true;//确保Cookie只能通过HTTP传输，不能通过客户端脚本访问
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);//设置认证Cookie的过期时间为60分钟
            options.LoginPath = "/Home/Login"; // 登录页面路径
            options.AccessDeniedPath = "/Home/AccessDenied"; // 访问被拒绝页面路径
        });

        services.AddControllersWithViews();//启用MVC

        services.AddSingleton<ApplicationDbContext>();//单例，数据库访问
        services.AddHostedService<DeleteUnverifiedUsersService>();
        services.AddSingleton(provider =>
        {
            var context = provider.GetService<ApplicationDbContext>();
            return new UsersService(context.Db);
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserInfoManager API", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //异常处理
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserInfoManager API V1");
        });

        //路由
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");
        });
    }
}
