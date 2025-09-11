using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Enums;

public enum StartMenu
{
    [Display(Name = "Login into the system")]
    Login,
    [Display(Name = "Register as a new customer")]
    Register,
    [Display(Name = "Exit the program")]
    Exit,
}