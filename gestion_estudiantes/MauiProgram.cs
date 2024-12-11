using Android.Graphics;
using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudiantes.Modelos.Modelos;
using Microsoft.Extensions.Logging;

namespace gestion_estudiantes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            Registrar();
            return builder.Build();
        }

        public static void Registrar()
        {
            FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");

            var cursos = client.Child("Cursos").OnceAsync<Curso>();

            if (cursos.Result.Count == 0)
            {
                client.Child("Cursos").PostAsync(new Curso { Nombre = "1ro Basico" });
                client.Child("Cursos").PostAsync(new Curso { Nombre = "2do Basico" });
                client.Child("Cursos").PostAsync(new Curso { Nombre = "3ro Basico" });
                client.Child("Cursos").PostAsync(new Curso { Nombre = "4to Basico" });
            }
        }
    }
}
