using System.Linq;

namespace ShoddyChess
{
	public class BoardItem
	{
		public PieceIdentifier Id { get; set; }
		public PieceColor Color { get; set; }
		public BoardItem() : this(PieceIdentifier.Q, PieceColor.White, true){}
		public bool CanBeChanged { get; private set; }

		public BoardItem(PieceIdentifier id, PieceColor color, bool canBeChanged)
		{
			this.Id = id;
			this.Color = color;
			this.CanBeChanged = canBeChanged;
		}
		
		public override string ToString()
		{
			return string.Join("", new[]{Color.ToString().First().ToString(), Id.ToString()});
		}
	}
}