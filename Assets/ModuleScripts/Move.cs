namespace ShoddyChess
{
	public class Move
	{
		public Pair<int,int> From { get; private set; }

		public Pair<int,int> To { get; private set; }
		
		public bool Capture { get; private set; }
		
		public Move(Pair<int,int> from, Pair<int,int> to)
		{
			From = from;
			To = to;
		}
		
		public Move(Pair<int,int> from, Pair<int,int> to, bool capture)
		{
			From = from;
			To = to;
			Capture = capture;
		}
	}
}
