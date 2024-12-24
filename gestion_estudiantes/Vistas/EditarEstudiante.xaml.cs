using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudiantes.Modelos.Modelos;
using System.Collections.ObjectModel;

namespace gestion_estudiantes.Vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; }
    public ObservableCollection<string> ListaCursos { get; set; } = new ObservableCollection<string>();
    private Estudiante estudianteActual = new Estudiante();
    private string estudianteId;
    public EditarEstudiante(string idEstudiante)
	{
		InitializeComponent();
        BindingContext = this;
        estudianteId = idEstudiante;
        CargarListasCursos();
        CargarEstudiantes(estudianteId);
    }

    private async void CargarListasCursos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListaCursos.Clear();
            foreach (var curso in cursos)
            {
                ListaCursos.Add(curso.Object.Nombre);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrio un error al cargar los cursos: {ex.Message}", "Ok");
        }
    }

    private async void CargarEstudiantes(string idEstudiante)
    {
        var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

        if (estudiante != null)
        {
            EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
            EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
            EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
            EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
            EditCorreoEntry.Text = estudiante.CorreoElectronico;
            EditEdadEntry.Text = estudiante.Edad.ToString();
            EditCursoPicker.SelectedItem = estudiante.Curso.Nombre;
        }
    }

    private async void EditActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditEdadEntry.Text) ||
                EditCursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son requeridos", "Ok");
                return;
            }

            if (!EditCorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "El correo electronico no es valido", "Ok");
                return;
            }

            if (!int.TryParse(EditEdadEntry.Text, out var edad))
            {
                await DisplayAlert("Error", "La edad no es un numero valido", "Ok");
                return;
            }

            if (edad <= 0)
            {
                await DisplayAlert("Error", "La edad debe ser mayor a 0", "Ok");
                return;
            }

            // Actualizar estudiante
            estudianteActual.Id = estudianteId;
            estudianteActual.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            estudianteActual.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            estudianteActual.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            estudianteActual.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            estudianteActual.CorreoElectronico = EditCorreoEntry.Text.Trim();
            estudianteActual.Edad = edad;
            estudianteActual.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };
            estudianteActual.Estado=EditEstadoSwitch.IsToggled;

            // Actualizar estudiante en la base de datos
            await client.Child("Estudiantes").Child(estudianteActual.Id).PutAsync(estudianteActual);

            // Mostrar mensaje de éxito
            await DisplayAlert("Exito", $"El Estudiante {estudianteActual.PrimerNombre} {estudianteActual.PrimerApellido} fue actualizado con exito", "Ok");

            // Regresar a la lista de estudiantes
            await Navigation.PopAsync();
        }

        catch (Exception)
        {

        }
    }
}