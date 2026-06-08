import io
with io.open(r'c:\Users\LEGION\source\repos\AläiaStore\AlaiaStore.Web\Views\Shared\_Layout.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()
content = content.replace('href="#"><i class="bi bi-bag"></i>', 'asp-controller="Cart" asp-action="Index"><i class="bi bi-cart3"></i>')
with io.open(r'c:\Users\LEGION\source\repos\AläiaStore\AlaiaStore.Web\Views\Shared\_Layout.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
