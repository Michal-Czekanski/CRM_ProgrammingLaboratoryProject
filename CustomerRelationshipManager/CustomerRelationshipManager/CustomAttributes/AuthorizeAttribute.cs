﻿using Microsoft.AspNetCore.Mvc;

namespace CustomerRelationshipManager.CustomAttributes
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }

        
    }
}
