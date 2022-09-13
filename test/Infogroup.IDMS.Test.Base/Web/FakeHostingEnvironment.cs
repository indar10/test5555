using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Infogroup.IDMS.Test.Base.Hosting
{
    public class FakeHostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get => "Development"; set => throw new System.NotImplementedException(); }
        public string ApplicationName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string WebRootPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ContentRootPath { get => @"D:\Development\IDMS_NG\IDMS_NG\src\Infogroup.IDMS.Web.Host\"; set => throw new System.NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
