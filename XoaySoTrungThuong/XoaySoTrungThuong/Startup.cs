using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XoaySoTrungThuong.Startup))]
namespace XoaySoTrungThuong
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
