using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace ViewAligner
{
    public class Aligner
    {
        private DrawingHandler _dh;
        private Drawing _activeDrawing;

        public void Execute()
        {
            if (!InitailizeDrawingHandler())
                return;

            SectionAggregator sectionAggregator = new SectionAggregator(_dh, _activeDrawing);
            sectionAggregator.GetSectionsFromDrawing();
            sectionAggregator.SourceViews.ForEach(x => x.Align());

            _activeDrawing.CommitChanges();
        }

        private bool InitailizeDrawingHandler()
        {
            _dh = new DrawingHandler();
            if (!_dh.GetConnectionStatus())
                return false;

            _activeDrawing = _dh.GetActiveDrawing();
            if (_activeDrawing == null)
                return false;

            return true;
        }
    }
}
