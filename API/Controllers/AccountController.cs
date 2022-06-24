using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
            _tokenService = tokenService;
            _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDtos registerDtos)
    {
        if(await UserExits(registerDtos.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser{
            UserName = registerDtos.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDtos.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Username =user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //get the user from db
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

        if(user == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for(int i=0; i<computeHash.Length ; i++)
        {
            if(computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username =user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExits(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

  }
}