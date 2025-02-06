namespace ExitTheBoard
{
    public class LineNode
    {
        private PointNode startPoint;
        private PointNode endPoint;

        public LineNode[] FromStartLines { get; private set; }
        public LineNode[] FromEndLines { get; private set; }

        public LineNode(PointNode startPoint, PointNode endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public void SetNeighboursStartLines(LineNode[] lines)
        {
            FromStartLines = lines;
        }
        
        public void SetNeighboursEndLines(LineNode[] lines)
        {
            FromEndLines = lines;
        }

        public LineTrack GetTrack()
        {
            return new LineTrack(startPoint.Position, endPoint.Position);
        }
    }
}