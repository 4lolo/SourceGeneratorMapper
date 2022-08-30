using SourceGeneratorMapper.Attributes;
using SourceGeneratorMapper.Tests.Model.Api;
using SourceGeneratorMapper.Tests.Model.Web;

namespace SourceGeneratorMapper.Tests.Mapper;

[Mapper]
[Mapping<User, WebUser>(TwoWay = true)]
public partial class TestMapper : IMapper
{
    
}