$path = "AlaiaStore.Web\Views\Shared\_Layout.cshtml"
$text = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
$text = $text.Replace("<a class=`"nav-link`" href=`"#`"><i class=`"bi bi-bag`"></i></a>", "<a class=`"nav-link`" asp-controller=`"Cart`" asp-action=`"Index`"><i class=`"bi bi-cart3`"></i></a>")
[System.IO.File]::WriteAllText($path, $text, [System.Text.Encoding]::UTF8)
Write-Output "Done"
