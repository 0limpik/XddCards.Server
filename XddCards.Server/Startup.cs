using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using XddCards.Server.Controllers.Cycles.BlackJack;
using XddCards.Server.Controllers.Cycles.BlackJack.Controllers;
using XddCards.Server.Controllers.Games;
using XddCards.Server.Model.Auth;
using XddCards.Server.Services.Auth;
using XddCards.Server.Services.Games;
using XddCards.Server.Services.Games.BlackJack;
using HandController = XddCards.Server.Controllers.Cycles.BlackJack.Controllers.HandController;

namespace XddCards.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ServerLoggingInterceptor>();
            });

            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.ISSUER,

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,

                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.SecurityKey,
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });


            app.UseAuthentication();
            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                //app.UseMiddleware<PingSimulatorMiddleware>();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGrpcService<BlackJackController>();
                endpoints.MapGrpcService<BetController>();
                endpoints.MapGrpcService<GameController>();
                endpoints.MapGrpcService<HandController>();
                endpoints.MapGrpcService<BJCycleController>();
                endpoints.MapGrpcService<UserController>();
                endpoints.MapGrpcService<Controllers.Cycles.BlackJack.HandController>();

                endpoints.MapGrpcService<AuthService>();
                endpoints.MapGrpcService<GamesService>();
                endpoints.MapGrpcService<GameService>();
                endpoints.MapGrpcService<PlayerService>();
            });
        }
    }

    public class PingSimulatorMiddleware
    {
        private readonly Random random = new();

        private readonly RequestDelegate _next;

        public PingSimulatorMiddleware(RequestDelegate next)
            => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            await Task.Delay(random.Next(500, 1000));

            await _next(context);
        }
    }

    public class ServerLoggingInterceptor : Interceptor
    {
        private readonly Random random = new();

        public async override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await Delay();

            return await continuation(requestStream, context);
        }

        public async override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await Delay();

            await continuation(requestStream, responseStream, context);
        }

        public async override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await Delay();

            await continuation(request, responseStream, context);
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            await Delay();

            return await continuation(request, context);
        }

        private async Task Delay()
        {
            await Task.Delay(random.Next(50, 150));
        }
    }
}
