using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly ReusableSql _reusableSql;
  
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection(){
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = "";

        DynamicParameters dynamicParameters = new DynamicParameters();

        if (userId != 0){
            stringParameters += ", @UserId = @UserIdParameter";
            dynamicParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive){
            stringParameters += ", @Active = @ActiveParameter";
            dynamicParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }

        if(stringParameters.Length > 0){
            sql += stringParameters.Substring(1);
        }
     
        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, dynamicParameters);
        return users;
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete userComplete)
    {
        if(_reusableSql.UpsertUser(userComplete)){
            return Ok();    
        }

        throw new Exception("Failed to Update/Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId){

        string sql = @"EXEC TutorialAppSchema.spUser_Delete @UserId = @UserIdParameter";

        DynamicParameters dynamicParameters = new DynamicParameters();

        dynamicParameters.Add("@UserIdParameter", userId, DbType.Int32);
        
        
        if(_dapper.ExecuteSqlWithParameters(sql, dynamicParameters)){
            return Ok();    
        }

        throw new Exception("Failed to Delete User");
    }
}
