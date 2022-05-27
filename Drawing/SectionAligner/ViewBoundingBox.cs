using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace ViewAligner
{
    public class ViewBoundingBox
    {
        public AABB BoundingBox { get; private set; }
        public Point CenterPoint { get { return BoundingBox.GetCenterPoint(); } }
        public double LeftBorder { get { return BoundingBox.MinPoint.X; } }
        public double RightBorder { get { return BoundingBox.MaxPoint.X; } }
        public double UpperBorder { get { return BoundingBox.MaxPoint.Y; } }
        public double LowerBorder { get { return BoundingBox.MinPoint.Y; } }

        private View _view;

        public ViewBoundingBox(View view)
        {
            _view = view;

            RefreshBoundingBox();
        }

        public View GetView()
        {
            return _view;
        }

        public void RefreshBoundingBox()
        {
            _view.Select();

            BoundingBox = new AABB(GetBottomLeftViewFrame(), GetTopRightViewFrame());
        }

        private Point GetBottomMidViewFrame()
        {
            Point point = GetBottomLeftViewFrame();
            Point point2 = GetBottomRightViewFrame();

            return new Point(
                (point.X + point2.X) / 2.0,
                (point.Y + point2.Y) / 2.0,
                (point.Z + point2.Z) / 2.0
                );
        }

        private Point GetTopMidViewFrame()
        {
            Point point = GetTopLeftViewFrame();
            Point point2 = GetTopRightViewFrame();

            return new Point(
                (point.X + point2.X) / 2.0,
                (point.Y + point2.Y) / 2.0,
                (point.Z + point2.Z) / 2.0
                );
        }

        private Point GetBottomLeftViewFrame()
        {
            return _view.Origin + _view.FrameOrigin;
        }

        private Point GetBottomRightViewFrame()
        {
            Point point = GetBottomLeftViewFrame();
            point.X += _view.Width;
            return point;
        }

        private Point GetTopLeftViewFrame()
        {
            Point point = GetBottomLeftViewFrame();
            point.Y += _view.Height;

            return point;
        }

        private Point GetTopRightViewFrame()
        {
            Point point = GetTopLeftViewFrame();
            point.X += _view.Width;
            return point;
        }
    }
}
