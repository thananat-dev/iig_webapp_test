using Npgsql;

namespace iig_webapp_test.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postresql database.");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("ConnectionStrings:WebApiDatabase"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS public.""ChangeUserPassword""
(
    ""Id"" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    ""UserId"" bigint,
    ""UserOldPassword"" text COLLATE pg_catalog.""default"",
    ""LastUpdate"" timestamp without time zone,
    CONSTRAINT ""ChangeUserPassword_pkey"" PRIMARY KEY (""Id"")
)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS public.""Users""
(
    ""UserId"" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    ""Username"" character varying(12) COLLATE pg_catalog.""default"",
    ""Password"" text COLLATE pg_catalog.""default"",
    ""FirstName"" character varying(60) COLLATE pg_catalog.""default"",
    ""LastName"" character varying(60) COLLATE pg_catalog.""default"",
    ""ProfileImage"" text COLLATE pg_catalog.""default"",
    CONSTRAINT ""Users_pkey"" PRIMARY KEY (""UserId"")
)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}
