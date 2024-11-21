using MudBlazor;

namespace WebProject.Theme
{
  public static class CustomTheme
  {
    public static MudTheme MyTheme = new MudTheme()
    {
      PaletteLight = new PaletteLight()
      {
        Primary = "#1A1A1D", // Your primary color
        Error = "#e50000",   // Your error color
      }
    };
  }
}