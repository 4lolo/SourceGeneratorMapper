using System;

namespace SourceGeneratorMapper.Tests.Model.Web;

public class WebUser
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Email { get; set; }
}