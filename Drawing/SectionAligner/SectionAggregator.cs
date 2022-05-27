using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace ViewAligner
{
    public class SectionAggregator
    {
        public List<SourceViewWithSections> SourceViews { get; private set; }

        private DrawingHandler _dh;
        private Drawing _drawing;

        public SectionAggregator(DrawingHandler dh, Drawing drawing)
        {
            _dh = dh;
            _drawing = drawing;
            SourceViews = new List<SourceViewWithSections>();
        }

        public void GetSectionsFromDrawing()
        {
            SourceViews = GetOnlySelectedViews();
            if (SourceViews.Count == 0)
                SourceViews = GetAllPartViewsWithCorrelatedSections();
        }

        private List<SourceViewWithSections> GetOnlySelectedViews()
        {
            List<SourceViewWithSections> sourceViews = new List<SourceViewWithSections>();

            foreach (var selectedObject in _dh.GetDrawingObjectSelector().GetSelected())
            {
                if (selectedObject is View)
                {
                    View selectedView = selectedObject as View;
                    if (selectedView.ViewType == View.ViewTypes.SectionView)
                    {
                        AddSectionViewWithRelatedSource(selectedView, sourceViews);
                    }
                    else
                    {
                        AddSectionViewsFromMainView(selectedView, sourceViews);
                    }
                }
            }

            return sourceViews;
        }

        private List<SourceViewWithSections> GetAllPartViewsWithCorrelatedSections()
        {
            List<SourceViewWithSections> sourceViews = new List<SourceViewWithSections>();
            foreach (View sectionView in GetAllSectionViews())
            {
                AddSectionViewWithRelatedSource(sectionView, sourceViews);
            }

            return sourceViews;
        }

        private void AddSectionViewWithRelatedSource(View sectionView, List<SourceViewWithSections> sourceViews)
        {
            View sourceView = GetSourceViewFromSection(sectionView);
            if (sourceView == null)
                return;

            int sourceViewId = sourceView.GetId();
            var existingSourceView = sourceViews.Find(x => x.SourceViewId == sourceViewId);
            if (existingSourceView == null)
                sourceViews.Add(new SourceViewWithSections(sourceView, sourceViewId, sectionView));
            else
                existingSourceView.AddSection(sectionView);
        }

        private void AddSectionViewsFromMainView(View mainView, List<SourceViewWithSections> sourceViews)
        {
            int mainViewId = mainView.GetId();

            foreach (View sectionView in GetAllSectionViews())
            {
                View sourceView = GetSourceViewFromSection(sectionView);
                if (sourceView != null)
                {
                    if (mainViewId == sourceView.GetId())
                    {
                        var existingSourceView = sourceViews.Find(x => x.SourceViewId == mainViewId);
                        if (existingSourceView == null)
                            sourceViews.Add(new SourceViewWithSections(sourceView, mainViewId, sectionView));
                        else
                            existingSourceView.AddSection(sectionView);
                    }
                }
            }
        }

        private View GetSourceViewFromSection(View section)
        {
            DrawingObjectEnumerator doe = section.GetRelatedObjects(new Type[] { typeof(SectionMark) });
            while (doe.MoveNext())
            {
                if (doe.Current is SectionMark)
                    return doe.Current.GetView() as View;
            }

            return null;
        }

        private IEnumerable<View> GetAllSectionViews()
        {
            foreach (var item in _drawing.GetSheet().GetAllViews())
            {
                if (item is View)
                {
                    View view = (View)item;
                    if (view.ViewType == View.ViewTypes.SectionView)
                        yield return view;
                }
            }
        }
    }
}
