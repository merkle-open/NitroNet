using Castle.Windsor;

namespace NitroNet.CastleWindsorModules
{
    public interface ICastleWindsorModule
    {
        void Configure(IWindsorContainer container);
    }
}