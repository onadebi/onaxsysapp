﻿namespace AppGlobal.Models;
public class UserLoginQueryResponseDto : UserLoginDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsDeactivated { get; set; }
    public bool IsDeleted { get; set; }
    public int Id { get; set; }
    public string? Guid { get; set; }

}

public class UserLoginResponse
{
    public string? token { get; set; }
    public SocialLogin? SocialLogin { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    //[System.Text.Json.Serialization.JsonIgnore]
    public string? Guid { get; set; }
    public string? Picture { get; set; }
    public long Id { get; set; }
    public string[] Roles { get; set; } = [];

}
