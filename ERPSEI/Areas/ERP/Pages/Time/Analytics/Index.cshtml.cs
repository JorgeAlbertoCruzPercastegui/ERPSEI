using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.ERP.Pages.Time.Analytics
{
    [Authorize(Roles = "Master,Administrador,Administrador TH")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}