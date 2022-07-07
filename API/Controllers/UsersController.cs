using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
    private readonly IPhotoService _photoService;
    public UsersController(IUserRepository userRepository, IMapper mapper,
                            IPhotoService photoService)
    {
      _photoService = photoService;
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

    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdatedDto memberUpdatedDto) 
    {
      //var username = User.GetUsername();
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      _mapper.Map(memberUpdatedDto, user);

      _userRepository.Update(user);

      if(await _userRepository.SaveAllChangesAsync()) return NoContent();

      return BadRequest("Failed to update User");
    }
   
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var result = await _photoService.AddPhotoAsync(file);

      if(result.Error != null) return BadRequest(result.Error.Message);

      var photo = new Photo
      {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
      };

      if(user.Photos.Count == 0)
      {
        photo.IsMain = true;
      }
      user.Photos.Add(photo);

      if(await _userRepository.SaveAllChangesAsync())
      {
        //return _mapper.Map<PhotoDto>(photo);
        return CreatedAtRoute("GetUser", new{username = user.UserName} ,_mapper.Map<PhotoDto>(photo) );
      }

      return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if(photo.IsMain) return BadRequest("This is Already your main photo");

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if(currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      if(await _userRepository.SaveAllChangesAsync()) return NoContent();

      return BadRequest("Failed to set main photot");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if(photo == null) return NotFound();

      if(photo.IsMain) return BadRequest("You cann't Delete your main photo");

      if(photo.PublicId != null)
      {
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);
        if(result.Error != null) return BadRequest(result.Error.Message);
      }

      user.Photos.Remove(photo);

      if(await _userRepository.SaveAllChangesAsync()) return Ok();

      return BadRequest("Failed to delete photo");
    }
  }
}