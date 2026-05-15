using CertDesk.Data;
using CertDesk.Forms;
using CertDesk.Services;

namespace CertDesk;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        DbInitializer.EnsureCreated();
        StatusService.UpdateAllStatuses();
        Application.Run(new LoginForm());
    }
}
