using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudiantes.Modelos.Modelos;
using System.Collections.ObjectModel;

namespace gestion_estudiantes.Vistas;

public partial class ListarEstudiantes : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://gestionestudiantes-b6894-default-rtdb.firebaseio.com/");
    public ObservableCollection<Estudiante> Lista { get; set; } =  new ObservableCollection<Estudiante>();
    public ListarEstudiantes()
	{
		InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    private async void CargarLista()
    {
        //Limpiar la lista
        Lista.Clear();

        //Obtener los estudiantes de la base de datos
        var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

        //Filtrar los estudiantes activos
        var estudiantesActivos = estudiantes.Where(e => e.Object.Estado == true).ToList();

        foreach (var estudiante in estudiantesActivos)
        {
            Lista.Add(new Estudiante
            {
                Id = estudiante.Key,
                PrimerNombre = estudiante.Object.PrimerNombre,
                SegundoNombre = estudiante.Object.SegundoNombre,
                PrimerApellido = estudiante.Object.PrimerApellido,
                SegundoApellido = estudiante.Object.SegundoApellido,
                CorreoElectronico = estudiante.Object.CorreoElectronico,
                Edad = estudiante.Object.Edad,
                Estado = estudiante.Object.Estado,
                Curso = estudiante.Object.Curso
            });
        }
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if (filtro.Length > 0 )
        {
            listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoEstudianteButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearEstudiante());
    }

    private async void EditarImgButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;

        if (estudiante != null &&!string.IsNullOrEmpty(estudiante.Id))
        {
            await Navigation.PushAsync(new EditarEstudiante(estudiante.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener el id del estudiante", "Ok");
        }
    }

    private async void DeshabilitarImgButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var estudiante = boton?.CommandParameter as Estudiante;

        if (estudiante is null)
        {
            await DisplayAlert("Error", "No se pudo obtener el id del estudiante", "Ok");
            return;
        }

        //Mensaje de confirmación
        bool confirmacion = await DisplayAlert("Confirmación", $"¿Está seguro que desea deshabilitar al estudiante {estudiante.NombreCompleto}?", "Si", "No");

        if (confirmacion)
        {
            try
            {
                //Actualizar el estado del estudiante
                estudiante.Estado = false;

                //Actualizar el estudiante en la base de datos
                await client.Child("Estudiantes").Child(estudiante.Id).PutAsync(estudiante);

                //Mostrar mensaje de éxito
                await DisplayAlert("Exito", $"El estudiante {estudiante.NombreCompleto} fue deshabilitado con exito", "Ok");

                //Recargar la lista
                CargarLista();
            }
            catch (Exception)
            {
            }
        }
    }
}