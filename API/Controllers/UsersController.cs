using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [Authorize]

  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : BaseApiController
  {
    //it initialize context filed in and return the value of nd  it'll use the db in the whole code
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
      _mapper = mapper;
      _userRepository = userRepository;
      //it will access the database
      //  _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      var users = await _userRepository.GetMembersAsync();

      //var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

      return Ok(users);
    }

    /*[HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUsers(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }*/

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      
      return await _userRepository.GetMemberAsync(username);

   }


  }
}