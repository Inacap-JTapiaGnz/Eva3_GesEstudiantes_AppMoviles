using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudiantes.Modelos.Modelos;
namespace gestion_estudiantes.Vistas;

public partial class CrearEstudiante : ContentPage
{
	FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");
	public List<Curso> Cursos { get; set; }
    public CrearEstudiante()
	{
		InitializeComponent();
        ListarCursos();
        BindingContext = this;
    }

    private void ListarCursos()
    {
        var cursos = client.Child("Cursos").OnceAsync<Curso>();
        Cursos = cursos.Result.Select(x => x.Object).ToList();
    }

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
        Curso curso = cursoPicker.SelectedItem as Curso;

        var estudiante = new Estudiante
        {
            PrimerNombre = primerNombreEntry.Text,
            SegundoNombre = segundoNombreEntry.Text,
            PrimerApellido = primerApellidoEntry.Text,
            SegundoApellido = segundoApellidoEntry.Text,
            CorreoElectronico = correoEntry.Text,
            Edad = int.Parse(edadEntry.Text),
            Curso = curso
        };

        try
        {
            // Guardar estudiante
            await client.Child("Estudiantes").PostAsync(estudiante);

            // Mostrar mensaje de éxito
            await DisplayAlert("Exito", $"El Estudiante {estudiante.PrimerNombre} {estudiante.PrimerApellido} fue guardado con exito", "Ok");

            // Regresar a la lista de estudiantes
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrio un error al guardar el estudiante: {ex.Message}", "Ok");
        }
    }
}