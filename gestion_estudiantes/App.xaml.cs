using gestion_estudiantes.Vistas;

namespace gestion_estudiantes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ListarEstudiantes());
        }
    }
}
