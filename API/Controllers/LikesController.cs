using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
  public class LikesController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;
    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

        if(likedUser == null) return NotFound();

        if(sourceUser.UserName == username) return BadRequest("You can not like your self");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if(userLike != null) return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            LikedUserId =likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if(await _userRepository.SaveAllChangesAsync()) return Ok();

        return BadRequest("Failed to like");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLike([FromQuery]LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await _likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount ,users.TotalPages);
        return Ok(users);
    }



  }
}