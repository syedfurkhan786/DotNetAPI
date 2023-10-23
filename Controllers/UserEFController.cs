using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IMapper _mapper;

    IUserRepository _userRepository;
  
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
 
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>(); 
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);

        if(userDb != null){
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Failed to Update User");
        }
        throw new Exception("Failed to Get User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);
           
        _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges()){
            return Ok();    
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId){

        User? userDb = _userRepository.GetSingleUser(userId);

        if(userDb != null){
            _userRepository.RemoveEntity<User>(userDb);
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Failed to Delete User");
        }
        throw new Exception("Failed to Get User");
    }
            
    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetUserSalaryEF(int userId)
    {
        return _userRepository.GetSingleUserSalary(userId);   
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalaryEF(UserSalary userForInsert)
    {
        _userRepository.AddEntity<UserSalary>(userForInsert);
        if(_userRepository.SaveChanges()){
            return Ok();    
        }

        throw new Exception("Adding UserSalary failed on save");
    }


    [HttpPut("UserSalary")]
    public IActionResult PutUserSalaryEF(UserSalary userForUpdate)
    {
        UserSalary? userToUpdate = _userRepository.GetSingleUserSalary(userForUpdate.UserId);

        if(userToUpdate != null){
            _mapper.Map(userForUpdate,userToUpdate);
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Updating Salary failed on save");
        }
        throw new Exception("Failed to find User Salary to update");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEF(int userId){

        UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);

        if(userToDelete != null){
            _userRepository.RemoveEntity<UserSalary>(userToDelete);
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Delete Salary failed on save");
        }
        throw new Exception("Failed to find User Salary to delete");
    }

    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfoEF(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfoEF(UserJobInfo userForInsert)
    {
        _userRepository.AddEntity<UserJobInfo>(userForInsert);
        if(_userRepository.SaveChanges()){
            return Ok();    
        }

        throw new Exception("Adding UserJobInfo failed on save");
    }


    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEF(UserJobInfo userForUpdate)
    {
        UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

        if(userToUpdate != null){
            _mapper.Map(userForUpdate,userToUpdate);
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Updating UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to update");
    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEF(int userId){

        UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfo(userId);

        if(userToDelete != null){
            _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
            
            if(_userRepository.SaveChanges()){
                return Ok();    
            }

            throw new Exception("Delete UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to delete");
    }
}
