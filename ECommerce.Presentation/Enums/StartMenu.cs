using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Enums;

public enum StartMenu
{
    [Display(Name = "Go to home page")]
    GoToHomePage,
    [Display(Name = "Exit the program")]
    ExitProgram,
}