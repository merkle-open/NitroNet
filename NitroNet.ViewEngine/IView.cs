using NitroNet.ViewEngine.Context;

namespace NitroNet.ViewEngine
{
    public interface IView : IView<object>
    {
    }

    public interface IView<in T>
	{
		void Render(T model, RenderingContext context);
    }
}