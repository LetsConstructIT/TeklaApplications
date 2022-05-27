using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace ViewAligner
{
    public class Sections
    {
        public List<ViewBoundingBox> leftVerticalSections = new List<ViewBoundingBox>();
        public List<ViewBoundingBox> rightVerticalSections = new List<ViewBoundingBox>();

        public List<ViewBoundingBox> lowerHorizontalSections = new List<ViewBoundingBox>();
        public List<ViewBoundingBox> upperHorizontalSections = new List<ViewBoundingBox>();

        private List<View> _allSections = new List<View>();
        private Vector _sourceViewAxisX;

        public Sections(View initialSection, Vector sourceViewAxisX)
        {
            Add(initialSection);
            _sourceViewAxisX = sourceViewAxisX;
        }

        public IEnumerable<int> GetIds()
        {
            foreach (View section in _allSections)
            {
                yield return section.GetId();
            }
        }

        public void Add(View section)
        {
            _allSections.Add(section);
        }

        internal void SortByLocation(Point sourceCenterPt)
        {
            foreach (var section in _allSections)
            {
                ViewBoundingBox viewBoundingBox = new ViewBoundingBox(section);

                if (IsSectionVertical(section))
                {
                    if (viewBoundingBox.CenterPoint.X < sourceCenterPt.X)
                        leftVerticalSections.Add(viewBoundingBox);
                    else
                        rightVerticalSections.Add(viewBoundingBox);
                }
                else
                {
                    if (viewBoundingBox.CenterPoint.Y < sourceCenterPt.Y)
                        lowerHorizontalSections.Add(viewBoundingBox);
                    else
                        upperHorizontalSections.Add(viewBoundingBox);
                }
            }

            leftVerticalSections = leftVerticalSections.OrderByDescending(x => x.CenterPoint.X).ToList();
            rightVerticalSections = rightVerticalSections.OrderBy(x => x.CenterPoint.X).ToList();

            lowerHorizontalSections = lowerHorizontalSections.OrderByDescending(x => x.CenterPoint.Y).ToList();
            upperHorizontalSections = upperHorizontalSections.OrderBy(x => x.CenterPoint.Y).ToList();
        }

        private bool IsSectionVertical(View section)
        {
            Vector sectionDirection = section.ViewCoordinateSystem.AxisX;
            double angleToGlobalX = (180 / Math.PI) * sectionDirection.GetAngleBetween(_sourceViewAxisX);

            return Math.Abs(angleToGlobalX) > 88 && Math.Abs(angleToGlobalX) < 92;
        }
    }
}
