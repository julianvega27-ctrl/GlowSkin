using System.IO;
using System.Text;

class Program {
    static void Main() {
        string path = @"c:\Users\LEGION\source\repos\AläiaStore\AlaiaStore.Web\Views\Shared\_Layout.cshtml";
        string text = File.ReadAllText(path, Encoding.UTF8);
        text = text.Replace("<a class=\"nav-link\" href=\"#\"><i class=\"bi bi-bag\"></i></a>", "<a class=\"nav-link\" asp-controller=\"Cart\" asp-action=\"Index\"><i class=\"bi bi-cart3\"></i></a>");
        File.WriteAllText(path, text, Encoding.UTF8);
    }
}
