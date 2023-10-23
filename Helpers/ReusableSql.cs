using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers{

    public class ReusableSql{

        private readonly DataContextDapper _dapper;

        public ReusableSql(IConfiguration config){
            _dapper = new DataContextDapper(config);
        }
        public bool UpsertUser(UserComplete userComplete)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                    @FirstName = @FirstNameParameter, @LastName = @LastNameParameter, @Email = @EmailParameter,
                    @Gender = @GenderParameter, @Active = @ActiveParameter, @JobTitle = @JobTitleParameter,
                    @Department = @DepartmentParameter, @Salary = @SalaryParameter, @UserId = @UserIdParameter";
        
            DynamicParameters dynamicParameters = new DynamicParameters();

            dynamicParameters.Add("@FirstNameParameter", userComplete.FirstName, DbType.String);
            dynamicParameters.Add("@LastNameParameter", userComplete.LastName, DbType.String);
            dynamicParameters.Add("@EmailParameter", userComplete.Email, DbType.String);
            dynamicParameters.Add("@GenderParameter", userComplete.Gender, DbType.String);
            dynamicParameters.Add("@ActiveParameter", userComplete.Active, DbType.Boolean);
            dynamicParameters.Add("@JobTitleParameter", userComplete.JobTitle, DbType.String);
            dynamicParameters.Add("@DepartmentParameter", userComplete.Department, DbType.String);
            dynamicParameters.Add("@SalaryParameter", userComplete.Salary, DbType.Decimal);
            dynamicParameters.Add("@UserIdParameter", userComplete.UserId, DbType.Int32);

            return _dapper.ExecuteSqlWithParameters(sql, dynamicParameters);
        }
    }
}