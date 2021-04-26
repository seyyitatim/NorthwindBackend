using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encyption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        private readonly IConfiguration Configuration;
        private TokenOptions tokenOptions;
        private DateTime _accessTokenExpiration;
        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            tokenOptions = new TokenOptions()
            {
                Audience = Configuration["TokenOptions:Audience"],
                Issuer = Configuration["TokenOptions:Issuer"],
                SecurityKey = Configuration["TokenOptions:SecurityKey"],
                AccessTokenExpiration = Convert.ToInt32(Configuration["TokenOptions:AccessTokenExpiration"])
            };
            _accessTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.AccessTokenExpiration);
        }

        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
        {
            var securityKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey);
            var signinCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(tokenOptions, user, signinCredentials, operationClaims);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials,
            List<OperationClaim> operationClaims)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                claims: SetClaims(user, operationClaims),
                notBefore: DateTime.Now,
                expires: _accessTokenExpiration,
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        public IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
        {
            var cliams = new List<Claim>();
            cliams.AddNameId(user.Id.ToString());
            cliams.AddEmail(user.Email);
            cliams.AddName($"{user.FirstName} {user.LastName}");
            cliams.AddRoles(operationClaims.Select(c => c.Name).ToArray());
            return cliams;
        }
    }
}
