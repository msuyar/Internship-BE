﻿using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Dtos;

public class LoginUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}