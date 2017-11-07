using System.Windows.Media;

namespace SmartDots.Helpers
{
    public class StatusIcon
    {
        public string Tooltip { get; }
        public int Rank { get; }
        public Color Color { get; }
        public Brush BorderColor => Color.A == 0 ? Brushes.Transparent : Brushes.Black;

        public StatusIcon(Color color, string tooltip, int rank)
        {
            Color = color;
            Tooltip = tooltip;
            Rank = rank;
        }

        public override string ToString()
        {
            return Tooltip;
        }
    }
}
