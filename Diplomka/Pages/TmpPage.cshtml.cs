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
    }
}
