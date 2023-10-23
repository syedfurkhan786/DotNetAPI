using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers{

    [Authorize] 
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase{

        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config){
            _dapper = new DataContextDapper(config);
        }    

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None"){
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";

            DynamicParameters dynamicParameters = new DynamicParameters();

            if(postId != 0){
                stringParameters += ", @PostId = @PostIdParam";
                dynamicParameters.Add("@PostIdParam", userId, DbType.Int32);
            }
            if(userId != 0){
                stringParameters += ", @UserId = @UserIdParam";
                dynamicParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if(searchParam.ToLower() != "none"){
                stringParameters += ", @SearchValue = @SearchValueParameter";
                dynamicParameters.Add("@SearchValueParameter", searchParam.ToLower(), DbType.String);
            }

            if(stringParameters.Length > 0){
                sql += stringParameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<Post>(sql, dynamicParameters); 
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(){
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParameter";

            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
        
            return _dapper.LoadDataWithParameters<Post>(sql, dynamicParameters); 
        }

        [HttpPut("UpsertPost")] 
        public IActionResult UpsertPost(Post postToUpsert){
            string sqlUpsertPost = @"EXEC TutorialAppSchema.spPosts_Upsert
                        @UserId = @UserIdParameter, @PostTitle = @PostTitleParameter,
                        @PostContent = @PostContentParameter";

            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            dynamicParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            dynamicParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);
           
            if(postToUpsert.PostId > 0){
                sqlUpsertPost += ", @PostId = @PostIdParam";
                dynamicParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }
            
            if(_dapper.ExecuteSqlWithParameters(sqlUpsertPost, dynamicParameters)){
                return Ok();
            }

            throw new Exception("Failed to Upsert new post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId){
            string sqlDeletePost = @"EXEC TutorialAppSchema.spPost_Delete 
                     @PostId = @PostIdParam, @UserId = @UserIdParam";

            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@PostIdParam", postId, DbType.Int32);
            dynamicParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
           
            if(_dapper.ExecuteSqlWithParameters(sqlDeletePost, dynamicParameters)){
                return Ok();
            }

            throw new Exception("Failed to Delete post");
        }
    }
}