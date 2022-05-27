using Tekla.Structures.Drawing;

namespace ViewAligner
{
    public static class DrawingExtensions
    {
        public static int GetId(this DatabaseObject view)
        {
            var identifier = (Tekla.Structures.Identifier)view.GetPropertyValue("Identifier");
            return identifier.ID;
        }
    }
}
