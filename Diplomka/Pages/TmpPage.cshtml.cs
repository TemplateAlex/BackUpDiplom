using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PDFReaderClassLibrary;
using PythonCaller;

namespace Diplomka.Pages
{
    public class TmpPageModel : PageModel
    {
        public string Check { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostRofl() 
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
            string path = "C:\\Users\\Alex\\Downloads\\grants_2019_rus_protected.pdf";
            Check = await PDFReader.CreateOrdersInDBByPDFText(path, connectionString);

            return Page();
        }

        public async Task<IActionResult> OnPostCallPython()
        {
            await PyFileCaller.CallPythonFileAsync();

            return Page();
        }
    }
}
