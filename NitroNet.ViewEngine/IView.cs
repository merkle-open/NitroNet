using Veil;

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