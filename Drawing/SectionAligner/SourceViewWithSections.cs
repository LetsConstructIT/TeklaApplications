using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace ViewAligner
{
    public class SourceViewWithSections
    {
        public int SourceViewId { get; private set; }

        private View _sourceView;
        private ViewBoundingBox _sourceBoundingBox;
        private Sections _sections;

        public SourceViewWithSections(View sourceView, int sourceViewId, View sectionView)
        {            
            _sourceView = sourceView;
            SourceViewId = sourceViewId;
            _sections = new Sections(sectionView, _sourceView.ViewCoordinateSystem.AxisX);
        }

        public void AddSection(View sectionView)
        {
            if (!_sections.GetIds().Contains(sectionView.GetId()))
                _sections.Add(sectionView);
        }

        public void Align()
        {
            _sourceBoundingBox = new ViewBoundingBox(_sourceView);

            _sections.SortByLocation(_sourceBoundingBox.CenterPoint);

            double boundaryValue = _sourceBoundingBox.LeftBorder;
            foreach (var viewBoundingBox in _sections.leftVerticalSections)
            {
                if (!Settings.PreserveInitiallyOverlappedViews || 
                    boundaryValue - viewBoundingBox.RightBorder > 0)
                    MoveToRightBorderValue(viewBoundingBox, boundaryValue);

                boundaryValue = viewBoundingBox.LeftBorder;
            }

            boundaryValue = _sourceBoundingBox.RightBorder;
            foreach (var viewBoundingBox in _sections.rightVerticalSections)
            {
                if (!Settings.PreserveInitiallyOverlappedViews || 
                    viewBoundingBox.LeftBorder - boundaryValue > 0)
                    MoveToLeftBorderValue(viewBoundingBox, boundaryValue);

                boundaryValue = viewBoundingBox.RightBorder;
            }

            AlignVertically(_sections.leftVerticalSections);
            AlignVertically(_sections.rightVerticalSections);

            boundaryValue = _sourceBoundingBox.UpperBorder;
            foreach (var viewBoundingBox in _sections.upperHorizontalSections)
            {
                if (!Settings.PreserveInitiallyOverlappedViews || 
                    viewBoundingBox.LowerBorder - boundaryValue > 0)
                    MoveToLowerBorderValue(viewBoundingBox, boundaryValue);

                boundaryValue = viewBoundingBox.UpperBorder;
            }

            boundaryValue = _sourceBoundingBox.LowerBorder;
            foreach (var viewBoundingBox in _sections.lowerHorizontalSections)
            {
                if (!Settings.PreserveInitiallyOverlappedViews || 
                    boundaryValue - viewBoundingBox.UpperBorder > 0)
                    MoveToUpperBorderValue(viewBoundingBox, boundaryValue);

                boundaryValue = viewBoundingBox.LowerBorder;
            }

            AlignHorizontally(_sections.lowerHorizontalSections);
            AlignHorizontally(_sections.upperHorizontalSections);
        }

        private void MoveToRightBorderValue(ViewBoundingBox viewBoundingBox, double boundaryValue)
        {
            View view = viewBoundingBox.GetView();
            view.Origin.X -= viewBoundingBox.RightBorder - boundaryValue;

            view.Modify();
            viewBoundingBox.RefreshBoundingBox();
        }

        private void MoveToLeftBorderValue(ViewBoundingBox viewBoundingBox, double boundaryValue)
        {
            View view = viewBoundingBox.GetView();
            view.Origin.X -= viewBoundingBox.LeftBorder - boundaryValue;

            view.Modify();
            viewBoundingBox.RefreshBoundingBox();
        }

        private void MoveToLowerBorderValue(ViewBoundingBox viewBoundingBox, double boundaryValue)
        {
            View view = viewBoundingBox.GetView();
            view.Origin.Y -= viewBoundingBox.LowerBorder - boundaryValue;

            view.Modify();
            viewBoundingBox.RefreshBoundingBox();
        }

        private void MoveToUpperBorderValue(ViewBoundingBox viewBoundingBox, double boundaryValue)
        {
            View view = viewBoundingBox.GetView();
            view.Origin.Y -= viewBoundingBox.UpperBorder - boundaryValue;

            view.Modify();
            viewBoundingBox.RefreshBoundingBox();
        }

        private void AlignVertically(IEnumerable<ViewBoundingBox> views)
        {
            foreach (var viewBoundingBox in views)
            {
                View sectionView = viewBoundingBox.GetView();
                sectionView.Origin.Y = _sourceBoundingBox.GetView().Origin.Y;
                sectionView.Modify();
            }
        }

        private void AlignHorizontally(IEnumerable<ViewBoundingBox> views)
        {
            foreach (var viewBoundingBox in views)
            {
                View sectionView = viewBoundingBox.GetView();
                sectionView.Origin.X = _sourceBoundingBox.GetView().Origin.X;
                sectionView.Modify();
            }
        }
    }
}
