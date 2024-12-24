﻿using Firebase.Database;
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
            ActualizarCursos();
            ActualizarEstudiantes();
            return builder.Build();
        }

        public static async Task ActualizarCursos()
        {
            FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");

            var cursos = await client.Child("Cursos").OnceAsync<Curso>();

            if (cursos.Count == 0)
            {
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1ro Basico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2do Basico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3ro Basico" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "4to Basico" });
            }
            else
            {
                foreach (var curso in cursos)
                {
                    if (curso.Object.Estado == null)
                    {
                        var cursoActualizado = curso.Object;
                        cursoActualizado.Estado = true;

                        await client.Child("Cursos").Child(curso.Key).PutAsync(cursoActualizado);
                    }
                }
            }
        }

        public static async Task ActualizarEstudiantes()
        {
            FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");

            var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

            foreach (var estudiante in estudiantes)
            {
                if (estudiante.Object.Estado == null)
                {
                    var estudianteActualizado = estudiante.Object;
                    estudianteActualizado.Estado = true;

                    await client.Child("Estudiantes").Child(estudiante.Key).PutAsync(estudianteActualizado);
                }
            }
        }
    }
}
