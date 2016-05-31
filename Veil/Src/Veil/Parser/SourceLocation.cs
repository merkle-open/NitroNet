namespace Veil.Parser
{
	public class SourceLocation
	{
		public string TemplateId { get; private set; }
		public int Length { get; private set; }
		public int Index { get; private set; }

		public SourceLocation(string templateId, int index, int length)
		{
			TemplateId = templateId;
			Length = length;
			Index = index;
		}

	    public SourceLocation SetLength(int length)
	    {
	        return new SourceLocation(this.TemplateId, this.Index, length);
	    }

	    public SourceLocation MoveIndex(int index)
	    {
	        return new SourceLocation(this.TemplateId, this.Index + index, Length - index);
	    }
	}
}