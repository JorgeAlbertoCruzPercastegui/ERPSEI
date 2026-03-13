using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Pages
{
    public class LibroPdfModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string PdfUrl { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (string.IsNullOrWhiteSpace(PdfUrl))
            {
                return RedirectToPage("/ManualesPoliticas");
            }

            PdfUrl = PdfUrl.Trim();

            if (PdfUrl.StartsWith("~/"))
            {
                PdfUrl = Url.Content(PdfUrl);
            }

            if (!PdfUrl.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/ManualesPoliticas");
            }

            return Page();
        }
    }
}