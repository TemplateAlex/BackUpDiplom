using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PDFReaderClassLibrary;
using PythonCaller;

namespace Diplomka.Pages
{
    public class AdminPageModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostParsePDFFile()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
            string path = "C:\\Users\\Alex\\Downloads\\grants_2019_rus_protected.pdf";
            string check = await PDFReader.CreateOrdersInDBByPDFText(path, connectionString);

            return Page();
        }

        public async Task<IActionResult> OnPostCreatePredictions()
        {
            await PyFileCaller.CallPythonFileAsync();

            return Page();
        }
    }
}
